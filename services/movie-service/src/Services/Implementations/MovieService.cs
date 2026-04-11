using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using movie_service.Data;
using movie_service.Models;
using movie_service.RequestDTOs;
using movie_service.ResponseDTOs;
using movie_service.Services.Interfaces;

namespace movie_service.Services.Implementations;

public class MovieService(ApplicationDbContext context) : IMovieService
{
    public IEnumerable<MovieResponseDto> GetAllMovies(CancellationToken ct)
    {
        var movies = context.Movies.Include(m => m.Genres).ThenInclude(mg => mg.Genre).Include(m => m.Showtimes).Select(MovieResponseDto.FromModel);

        return movies.ToList();
    }

    public IEnumerable<MovieResponseDto> GetMoviesWithUpcomingShowtimes(CancellationToken ct)
    {
        var movies = context.Movies.Where(m => m.Showtimes.Any(st => st.StartTime > DateTime.Now)).Include(m => m.Genres).ThenInclude(mg => mg.Genre).Include(m => m.Showtimes.Where(st => st.StartTime > DateTime.Now)).Select(MovieResponseDto.FromModel);

        return movies.ToList();
    }

    public async Task<MovieResponseDto?> GetMovieByIdAsync(Guid movieId, CancellationToken ct)
    {
        var movie = await context.Movies.Include(m => m.Genres).ThenInclude(mg => mg.Genre).Include(m => m.Showtimes).SingleOrDefaultAsync(m => m.Id == movieId, ct);

        return MovieResponseDto.FromModel(movie);
    }

    public async Task<bool> ExistsByNameAsync(string movieName, CancellationToken ct)
    {
        return await context.Movies.AnyAsync(m => m.Name.ToLower() == movieName.ToLower(), ct);
    }

    public async Task<bool> ExistsByIdAsync(Guid movieId, CancellationToken ct)
    {
        return await context.Movies.AnyAsync(m => m.Id == movieId, ct);
    }

    public async Task<Guid?> AddNewMovieAsync(CreateMovieRequestDto dtoFromRequest, CancellationToken ct)
    {
        var movie = new Movie
        {
            Name = dtoFromRequest.MovieName,
            DurationMinutes = dtoFromRequest.DurationMinutes,
            Genres = dtoFromRequest.GenresIds!.Select(g => new MovieGenre { GenreId = g }).ToList()
        };

        await context.Movies.AddAsync(movie, ct);

        return (await context.SaveChangesAsync(ct) > 0 ? movie.Id : null);
    }

    public async Task UpdateMovieAsync(Guid movieId, UpdateMovieRequestDto dtoFromRequest, CancellationToken ct)
    {
        var movieFromDB = await context.Movies.Include(m => m.Genres).SingleOrDefaultAsync(m => m.Id == movieId, ct);

        if (movieFromDB is null)
        {
            throw new InvalidOperationException($"Movie with ID: {movieId} does NOT exist.");
        }

        movieFromDB.Genres = dtoFromRequest.GenresIds?.Select(g => new MovieGenre { MovieId = movieId, GenreId = g }).ToList();
        movieFromDB.DurationMinutes = dtoFromRequest.DurationMinutes;
        movieFromDB.Name = dtoFromRequest.MovieName;

        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteMovieAsync(Guid movieId, CancellationToken ct)
    {
        if (!await this.ExistsByIdAsync(movieId, ct))
        {
            throw new InvalidOperationException($"Movie with ID: {movieId} does NOT exist.");
        }

        context.Remove(context.Movies.Find(movieId)!);

        await context.SaveChangesAsync(ct);
    }
}
