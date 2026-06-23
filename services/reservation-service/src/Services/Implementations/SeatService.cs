using Microsoft.EntityFrameworkCore;
using reservation_service.Data;
using reservation_service.Services.Interfaces;

namespace reservation_service.Services.Implementations;

public class SeatService(ApplicationDbContext context, ILogger<SeatService> logger, IHttpClientFactory httpClientFactory) : ISeatService
{
    public async Task<HashSet<string>> GetReservedAndLockedSeatsAsync(Guid showtimeId, CancellationToken ct)
    {
        string showtimeUrl = $"api/showtimes/{showtimeId}";
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, showtimeUrl);

        var movieServiceHttpClient = httpClientFactory.CreateClient("movie-service");
        HttpResponseMessage response;

        try
        {
            response = await movieServiceHttpClient.SendAsync(request, ct);
        }

        catch (Exception ex)
        {
            logger.LogError(ex, "Error while trying to connect to Movie Service to check showtime existence (HTTP HEAD request).");

            throw;
        }

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($"No showtime with the ID: {showtimeId}");
            }

            logger.LogWarning("Error in retrieving showtime data from Movie Service. Movie Service returns {MovieServiceResponseStatusCode}", response.StatusCode);
            throw new Exception();
        }

        var reservedSeats = context.Reservations.Where(r => r.ShowtimeId == showtimeId).Select(r => r.SeatNumber);
        var heldSeats = context.SeatHolds.Where(sh => sh.ShowtimeId == showtimeId && sh.HeldUntil > DateTime.UtcNow).Select(sh => sh.SeatNumber);

        return await reservedSeats.Union(heldSeats).ToHashSetAsync(ct);
    }

    public async Task<List<string>> GetAvailableSeatsAsync(Guid showtimeId, CancellationToken ct)
    {
        var unavailableSeats = await GetReservedAndLockedSeatsAsync(showtimeId, ct);

        var allSeats = new HashSet<string>
        {
            "A1", "A2", "A3", "A4",
            "B1", "B2", "B3", "B4",
            "C1", "C2", "C3", "C4",
            "D1", "D2", "D3", "D4"
        };

        return allSeats.Except(unavailableSeats).ToList();
    }
}
