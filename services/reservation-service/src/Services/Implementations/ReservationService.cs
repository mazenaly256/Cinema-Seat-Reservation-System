using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using reservation_service.Data;
using reservation_service.IntegrationDTOs.MovieService;
using reservation_service.Models;
using reservation_service.RequestDTOs;
using reservation_service.Services.Interfaces;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace reservation_service.Services.Implementations;

public class ReservationService(ApplicationDbContext context, ILogger<ReservationService> logger, IHttpClientFactory httpClientFactory) : IReservationService
{
    public async Task AddNewReservationAsync(CreateReservationRequestDto reservationDtoFromRequest, CancellationToken ct)
    {
        if (await context.Reservations.AnyAsync(r => r.ShowtimeId == reservationDtoFromRequest.ShowtimeId && r.SeatNumber == reservationDtoFromRequest.SeatNumber))
        {
            throw new InvalidOperationException("Reservation Conflict. The seat is already successfully reserved with a confirmed payment."); ;
        }

        if (!Regex.IsMatch(reservationDtoFromRequest.SeatNumber, @"^[A-D][1-4]$"))
        {
            throw new ArgumentException("The seat number format must be two characters, first is a letter from A to D and second is a digit from 1 to 4");
        }

        string showtimeUrl = $"api/showtimes/{reservationDtoFromRequest.ShowtimeId}";
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, showtimeUrl);
        HttpResponseMessage response;

        var movieServiceHttpClient = httpClientFactory.CreateClient("movie-service");

        try
        {
            response = await movieServiceHttpClient.SendAsync(request, ct);
        }

        catch (Exception ex)
        {
            logger.LogError(ex, "Error while trying to connect to Movie Service to check movie existence with HTTP HEAD request.");
            
            throw;
        }

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new ArgumentException($"No upcoming showtime with the ID: {reservationDtoFromRequest.ShowtimeId}");
            }

            else
            {
                logger.LogWarning("Error in checking showtime existence through HTTP HEAD Request to Movie Service. Movie Service Returns {MovieServiceResponseStatusCode}", response.StatusCode);
                throw new Exception("Failed to retrieve the showtime data.");
            }
        }



        try
        {
            context.SeatHolds.RemoveRange(context.SeatHolds.Where(sh => sh.HeldUntil <= DateTime.UtcNow));
            await context.SaveChangesAsync(ct);

            var seatTemporaryLock = new SeatHold
            {
                ShowtimeId = reservationDtoFromRequest.ShowtimeId,
                SeatNumber = reservationDtoFromRequest.SeatNumber,
                HeldUntil = DateTime.UtcNow.AddMinutes(10)
            };


            // checks if the seat has been reserved by another thread (another request)
            if (await context.Reservations.AnyAsync(r => r.ShowtimeId == reservationDtoFromRequest.ShowtimeId && r.SeatNumber == reservationDtoFromRequest.SeatNumber))
            {
                throw new InvalidOperationException("Reservation Conflict. The seat is already successfully reserved with a confirmed payment."); ;
            }

            await context.SeatHolds.AddAsync(seatTemporaryLock, ct);
            await context.SaveChangesAsync(ct);






            #region Mock Payment
            /* the logic of the payment processing will be here, now for the sake of simplicity,
            * suppose there is always a successful payment with new GUID */

            ShowtimeIntegrationDto? showtime;

            try
            {
                response = await movieServiceHttpClient.GetAsync(showtimeUrl, ct);
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "Error while trying to connect to Movie Service to get showtime's price.");

                throw;
            }

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new ArgumentException("No upcoming showtime with the sent ID");
                }

                else
                {
                    logger.LogError("Unable to retrieve showtime data from Movie Service. Movie Service responds with {MovieServiceResponseStatusCode} Status Code", response.StatusCode);
                    throw new InvalidOperationException("Failed to retrieve the showtime data.");
                }
            }

            showtime = await response.Content.ReadFromJsonAsync<ShowtimeIntegrationDto>(ct);

            if (showtime is null)
            {
                logger.LogError("Entity mapping between services failed, Retrieved showtime data from Movie Service is incompatible with the ShowtimeIntegrationDto defined in Reservation Service");

                throw new InvalidOperationException("Error while deserializing retrieved showtime data.");
            }

            var mockPayment = new Payment { PaidAmount = showtime!.Price, PaidAt = DateTime.UtcNow };
            await context.Payments.AddAsync(mockPayment, ct);
            await context.SaveChangesAsync(ct);
            #endregion







            var confirmedReservation = new Reservation
            {
                ShowtimeId = reservationDtoFromRequest.ShowtimeId,
                SeatNumber = reservationDtoFromRequest.SeatNumber,
                PaymentId = mockPayment.Id
            };

            await context.Reservations.AddAsync(confirmedReservation, ct);
            await context.SaveChangesAsync(ct);

            logger.LogInformation("Reservation for Seat Number: {ReservedSeatNumber} for showtime: {ShowtimeId}, has been confirmed successfully.", reservationDtoFromRequest.SeatNumber, reservationDtoFromRequest.ShowtimeId);
        }

        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("PK_SeatHolds") == true)    // Race condition handling
        {
            logger.LogInformation(ex, "Race condition has been resolved successfully. Concurrent reservation has been handled correctly");
            
            throw new InvalidOperationException($"Reservation Conflict. The seat is locked right now, try again after a while or choose another seat.");
        }

        catch (InvalidOperationException)
        {
            throw;
        }

        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected Error while try to reserve a seat.");

            throw;
        }
    }
}
