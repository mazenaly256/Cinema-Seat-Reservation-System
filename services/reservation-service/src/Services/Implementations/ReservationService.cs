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

public class ReservationService(ApplicationDbContext context, IConfiguration configuration) : IReservationService
{
    public async Task AddNewReservationAsync(CreateReservationRequestDto reservationDtoFromRequest, CancellationToken ct)
    {
        if (await context.Reservations.AnyAsync(r => r.ShowtimeId == reservationDtoFromRequest.ShowtimeId && r.SeatNumber == reservationDtoFromRequest.SeatNumber))
        {
            throw new InvalidOperationException("The seat is already successfully reserved with a confirmed payment."); ;
        }

        if (!Regex.IsMatch(reservationDtoFromRequest.SeatNumber, @"^[A-D][1-4]$"))
        {
            throw new ArgumentException("The seat number format must be two characters, first is a letter from A to D and second is a digit from 1 to 4");
        }

        using (HttpClient client = new HttpClient())
        {
            string url = $"{configuration["movieServiceBaseUrl"]}/api/showtimes/{reservationDtoFromRequest.ShowtimeId}";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, url);

            HttpResponseMessage response = await client.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new ArgumentException("No upcoming showtime with the sent ID");
                }

                else
                {
                    throw new InvalidOperationException("Failed to retrieve the showtime data.");
                }
            }
        }



        try
        {
            context.SeatHolds.RemoveRange(context.SeatHolds.Where(sh => sh.HeldUntil <= DateTime.Now));
            await context.SaveChangesAsync(ct);

            var seatTemporaryLock = new SeatHold
            {
                ShowtimeId = reservationDtoFromRequest.ShowtimeId,
                SeatNumber = reservationDtoFromRequest.SeatNumber,
                HeldUntil = DateTime.Now.AddMinutes(10)
            };


            // checks if the seat has been reserved by another thread (another request)
            if (await context.Reservations.AnyAsync(r => r.ShowtimeId == reservationDtoFromRequest.ShowtimeId && r.SeatNumber == reservationDtoFromRequest.SeatNumber))
            {
                throw new InvalidOperationException("The seat is already successfully reserved with a confirmed payment."); ;
            }

            await context.SeatHolds.AddAsync(seatTemporaryLock, ct);
            await context.SaveChangesAsync(ct);






            #region Mock Payment
            /* the logic of the payment processing will be here, now for the sake of simplicity,
            * suppose there is always a successful payment with new GUID */

            ShowtimeIntegrationDto? showtime;

            using (HttpClient client = new HttpClient())
            {
                string url = $"{configuration["movieServiceBaseUrl"]}/api/showtimes/{reservationDtoFromRequest.ShowtimeId}";

                var response = await client.GetAsync(url, ct);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        throw new ArgumentException("No upcoming showtime with the sent ID");
                    }

                    else
                    {
                        throw new InvalidOperationException("Failed to retrieve the showtime data.");
                    }
                }

                showtime = await response.Content.ReadFromJsonAsync<ShowtimeIntegrationDto>(ct);

                if (showtime is null)
                {
                    throw new InvalidOperationException("Failed to retrieve the showtime data.");
                }
            }

            var mockPayment = new Payment { PaidAmount = showtime!.Price, PaidAt = DateTime.Now };
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
        }

        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("PK_SeatHolds") == true)    // Race condition handling
        {
            // the message should be logged here

            
            throw new InvalidOperationException($"The seat is locked, try again after a while or choose another seat.");
        }

        catch (InvalidOperationException)
        {
            throw;
        }

        catch (Exception ex)
        {
            // the message should be logged here

            throw;
        }
    }
}
