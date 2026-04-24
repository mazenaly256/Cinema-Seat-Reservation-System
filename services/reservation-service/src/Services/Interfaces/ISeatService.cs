namespace reservation_service.Services.Interfaces;

public interface ISeatService
{
    Task<HashSet<string>> GetReservedAndLockedSeatsAsync(Guid showtimeId, CancellationToken ct);

    Task<List<string>> GetAvailableSeatsAsync(Guid showtimeId, CancellationToken ct);
}
