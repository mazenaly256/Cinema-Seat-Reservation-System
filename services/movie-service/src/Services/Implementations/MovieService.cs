using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using movie_service.Data;
using movie_service.Models;
using movie_service.RequestDTOs;
using movie_service.ResponseDTOs;
using movie_service.Services.Interfaces;

namespace movie_service.Services.Implementations;

public class MovieService(ApplicationDbContext context, ILogger<MovieService> logger) : IMovieService
{
    public IEnumerable<MovieResponseDto> GetAllMovies(CancellationToken ct)
    {
        var movies = context.Movies.Include(m => m.Genres).ThenInclude(mg => mg.Genre).Select(MovieResponseDto.FromModel);

        return movies.ToList();
    }

    public IEnumerable<MovieResponseDto> GetMoviesWithUpcomingShowtimes(CancellationToken ct)
    {
        var movies = context.Movies.Where(m => m.Showtimes.Any(st => st.StartTime > DateTime.UtcNow)).Include(m => m.Genres).ThenInclude(mg => mg.Genre).Select(MovieResponseDto.FromModel);

        return movies.ToList();
    }

    public async Task<MovieResponseDto?> GetMovieByIdAsync(Guid movieId, CancellationToken ct)
    {
        var movie = await context.Movies.Include(m => m.Genres).ThenInclude(mg => mg.Genre).SingleOrDefaultAsync(m => m.Id == movieId, ct);

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
        try
        {
            var movie = new Movie
            {
                Name = dtoFromRequest.MovieName,
                DurationMinutes = (int)dtoFromRequest.DurationMinutes!,
                Genres = dtoFromRequest.GenresIds!.Select(g => new MovieGenre { GenreId = g }).ToList()
            };

            await context.Movies.AddAsync(movie, ct);

            if (await context.SaveChangesAsync(ct) > 0)
            {
                logger.LogInformation("New Movie with ID: {NewMovieId} has been saved successfully on DB.", movie.Id);

                return movie.Id;
            }

            logger.LogWarning("No movie has been added when tried to add new movie with name: {NewMovieName}.", movie.Name);
            return null;
        }
        catch (OperationCanceledException ex)
        {
            logger.LogInformation(ex, "Adding new movie is canceled with cancellation token.");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while adding new movie to DB.");
            throw;
        }
    }

    public async Task UpdateMovieAsync(Guid movieId, UpdateMovieRequestDto dtoFromRequest, CancellationToken ct)
    {
        var movieFromDB = await context.Movies.Include(m => m.Genres).SingleOrDefaultAsync(m => m.Id == movieId, ct);

        if (movieFromDB is null)
        {
            throw new KeyNotFoundException($"Movie with ID: {movieId} does NOT exist.");
        }

        movieFromDB.Genres = dtoFromRequest.GenresIds?.Select(g => new MovieGenre { MovieId = movieId, GenreId = g }).ToList();
        movieFromDB.DurationMinutes = dtoFromRequest.DurationMinutes;
        movieFromDB.Name = dtoFromRequest.MovieName;

        if (await context.SaveChangesAsync(ct) > 0)
        {
            logger.LogInformation("Movie with ID: {MovieId} has been updated successfully", movieId);
        }

        else
        {
            logger.LogWarning("Failed updating operation for movie with ID: {MovieId}", movieId);
        }
    }

    public async Task DeleteMovieAsync(Guid movieId, CancellationToken ct)
    {
        logger.LogInformation("Attemting to delete movie with ID: {MovieId}.", movieId);

        if (!await this.ExistsByIdAsync(movieId, ct))
        {
            throw new KeyNotFoundException($"Movie with ID: {movieId} does NOT exist.");
        }

        context.Remove(context.Movies.Find(movieId)!);

        if (await context.SaveChangesAsync(ct) > 0)
        {
            logger.LogInformation("Movie with ID: {MovieId} has been successfully deleted from DB.", movieId);
        }
    }
}
