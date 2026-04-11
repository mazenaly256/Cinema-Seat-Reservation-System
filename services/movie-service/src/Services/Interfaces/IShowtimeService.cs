using movie_service.ResponseDTOs;

namespace movie_service.Services.Interfaces;

public interface IShowtimeService
{
    IEnumerable<ShowtimeResponseDto> GetAllUpcomingShowtimes();

    IEnumerable<ShowtimeResponseDto> GetUpcomingShowtimesByMovie(Guid movieId);
}
