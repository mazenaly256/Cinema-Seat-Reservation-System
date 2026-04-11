using movie_service.ResponseDTOs;
using movie_service.Services.Interfaces;

namespace movie_service.Services.Implementations;

public class ShowtimeService : IShowtimeService
{
    public IEnumerable<ShowtimeResponseDto> GetAllUpcomingShowtimes()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ShowtimeResponseDto> GetUpcomingShowtimesByMovie(Guid movieId)
    {
        throw new NotImplementedException();
    }
}
