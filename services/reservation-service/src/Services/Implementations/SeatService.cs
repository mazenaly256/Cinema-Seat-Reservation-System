using Microsoft.EntityFrameworkCore;
using reservation_service.Data;
using reservation_service.Services.Interfaces;

namespace reservation_service.Services.Implementations;

public class SeatService(ApplicationDbContext context, IConfiguration configuration) : ISeatService
{
    public async Task<HashSet<string>> GetReservedAndLockedSeatsAsync(Guid showtimeId, CancellationToken ct)
    {
        using (HttpClient client = new HttpClient())
        {
            string url = $"{configuration["movieServiceBaseUrl"]}/api/showtimes/{showtimeId}";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, url);

            HttpResponseMessage response = await client.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                throw new ArgumentException("No upcoming showtime with the sent ID");
            }
        }

        var reservedSeats = context.Reservations.Where(r => r.ShowtimeId == showtimeId).Select(r => r.SeatNumber);
        var heldSeats = context.SeatHolds.Where(sh => sh.ShowtimeId == showtimeId && sh.HeldUntil > DateTime.Now).Select(sh => sh.SeatNumber);

        return await reservedSeats.Union(heldSeats).ToHashSetAsync();
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
