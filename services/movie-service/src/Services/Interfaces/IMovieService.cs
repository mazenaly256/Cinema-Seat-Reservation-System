using movie_service.RequestDTOs;
using movie_service.ResponseDTOs;

namespace movie_service.Services.Interfaces;

public interface IMovieService
{
    IEnumerable<MovieResponseDto> GetAllMovies(CancellationToken ct);

    Task<MovieResponseDto?> GetMovieByIdAsync(Guid movieId, CancellationToken ct);

    IEnumerable<MovieResponseDto> GetMoviesWithUpcomingShowtimes(CancellationToken ct);

    Task<bool> ExistsByNameAsync(string movieName, CancellationToken ct);

    Task<Guid?> AddNewMovieAsync(CreateMovieRequestDto dtoFromRequest, CancellationToken ct);

    Task<bool> ExistsByIdAsync(Guid movieId, CancellationToken ct);

    Task UpdateMovieAsync(Guid movieId, UpdateMovieRequestDto dtoFromRequest, CancellationToken ct);

    Task DeleteMovieAsync(Guid movieId, CancellationToken ct);
}
