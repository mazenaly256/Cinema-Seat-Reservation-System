using movie_service.RequestDTOs;
using movie_service.ResponseDTOs;

namespace movie_service.Services.Interfaces;

public interface IShowtimeService
{
    IEnumerable<ShowtimeResponseDto> GetShowtimes(Guid? movieId, DateTime? from, DateTime? to,  string? status);
    
    Task<bool> ExistsByIdAsync(Guid showtimeId, CancellationToken ct);

    Task<ShowtimeResponseDto?> GetShowtimeByIdAsync(Guid showtimeId, CancellationToken ct);

    Task<Guid?> AddNewShowtimeAsync(CreateShowtimeRequestDto showtimeDtoFromRequest, CancellationToken ct);

    Task UpdateShowtimeAsync(Guid showtimeId, UpdateShowtimeRequestDto showtimeDtoFromRequest, CancellationToken ct);

    Task DeleteShowtimeAsync(Guid showtimeId, CancellationToken ct);
}
