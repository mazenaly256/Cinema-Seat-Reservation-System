using Microsoft.EntityFrameworkCore;
using movie_service.Data;
using movie_service.Models;
using movie_service.RequestDTOs;
using movie_service.ResponseDTOs;
using movie_service.Services.Interfaces;

namespace movie_service.Services.Implementations;

public class ShowtimeService(ApplicationDbContext context, ILogger<ShowtimeService> logger) : IShowtimeService
{
    public IEnumerable<ShowtimeResponseDto> GetShowtimes(Guid? movieId, DateTime? from, DateTime? to, string? status)
    {
        IQueryable<Showtime> showtimes = context.Showtimes.Include(st => st.Movie);

        if (movieId.HasValue)
        {
            showtimes = showtimes.Where(st => st.MovieId == movieId);
        }

        if (status is not null)
        {
            showtimes = showtimes.Where(st => st.StartTime > DateTime.UtcNow);
        }

        if (from.HasValue)
        {
            showtimes = showtimes.Where(st => st.StartTime >= from);
        }

        if (to.HasValue)
        {
            showtimes = showtimes.Where(st => st.StartTime <= to);
        }

        return showtimes.Select(ShowtimeResponseDto.FromModel);
    }

    public async Task<ShowtimeResponseDto?> GetShowtimeByIdAsync(Guid showtimeId, CancellationToken ct)
    {
        var showtime = await context.Showtimes.Include(st => st.Movie).SingleOrDefaultAsync(st => st.Id == showtimeId, ct);

        return ShowtimeResponseDto.FromModel(showtime!);
    }

    public async Task<Guid?> AddNewShowtimeAsync(CreateShowtimeRequestDto showtimeDtoFromRequest, CancellationToken ct)
    {
        var movie = await context.Movies.SingleOrDefaultAsync(m => m.Id == showtimeDtoFromRequest.ShowingMovieId, ct);

        if (movie is null)
        {
            throw new KeyNotFoundException($"Movie with ID: {showtimeDtoFromRequest.ShowingMovieId} does not exist. Showtime can not be for inexistant movie.");
        }

        if (showtimeDtoFromRequest.StartTime is null || showtimeDtoFromRequest.EndTime is null)
        {
            throw new ArgumentException($"Showtime must have start time and end time.");
        }

        if (showtimeDtoFromRequest.Price is null)
        {
            throw new ArgumentException($"Showtime must have a price.");
        }

        if (showtimeDtoFromRequest.StartTime <= DateTime.UtcNow)
        {
            throw new ArgumentException($"Can not make showtime's start date in the past.");
        }

        if ((showtimeDtoFromRequest.EndTime.Value - showtimeDtoFromRequest.StartTime.Value).TotalMinutes < movie.DurationMinutes)
        {
            throw new ArgumentException($"The showtime's duration ({(showtimeDtoFromRequest.EndTime.Value - showtimeDtoFromRequest.StartTime.Value).TotalMinutes} minutes) is less than the showing movie duration ({movie.DurationMinutes} minutes).");
        }

        var showtime = new Showtime
        {
            MovieId = showtimeDtoFromRequest.ShowingMovieId,
            StartTime = (DateTime)showtimeDtoFromRequest.StartTime,
            EndTime = (DateTime)showtimeDtoFromRequest.EndTime,
            Price = (decimal)showtimeDtoFromRequest.Price
        };

        await context.Showtimes.AddAsync(showtime, ct);

        if (await context.SaveChangesAsync(ct) > 0)
        {
            logger.LogInformation("New Showtime with ID: {NewShowtimeId} has been saved successfully on DB.", showtime.Id);

            return showtime.Id;
        }

        logger.LogWarning("No showtime has been added when tried to add new showtime for movie with ID: {ShowedMovieId}.", showtime.MovieId);
        return null;
    }

    public async Task UpdateShowtimeAsync(Guid showtimeId, UpdateShowtimeRequestDto showtimeDtoFromRequest, CancellationToken ct)
    {
        var showtimeFromDB = await context.Showtimes.Include(st => st.Movie).SingleOrDefaultAsync(st => st.Id == showtimeId, ct);

        if (showtimeFromDB is null)
        {
            throw new InvalidOperationException($"Showtime with ID: {showtimeId} does NOT exist.");
        }

        if (showtimeDtoFromRequest.StartTime is not null && showtimeDtoFromRequest.EndTime is not null)
        {
            if ((Convert.ToDateTime(showtimeDtoFromRequest.EndTime) - Convert.ToDateTime(showtimeDtoFromRequest.StartTime)).TotalMinutes < showtimeFromDB.Movie.DurationMinutes)
            {
                throw new InvalidOperationException($"The showtime's duration ({(Convert.ToDateTime(showtimeDtoFromRequest.EndTime) - Convert.ToDateTime(showtimeDtoFromRequest.StartTime)).TotalMinutes} minutes) is less than the showing movie duration ({showtimeFromDB.Movie.DurationMinutes} minutes).");
            }
        }

        if (showtimeDtoFromRequest.StartTime is not null && showtimeDtoFromRequest.StartTime <= DateTime.UtcNow)
        {
            throw new InvalidOperationException($"Can not make showtime's start date in the past.");
        }


        // using null-checking operator as when the value is not sent in request, it stays as the old value in DB
        showtimeFromDB.StartTime = showtimeDtoFromRequest.StartTime ?? showtimeFromDB.StartTime;
        showtimeFromDB.EndTime = showtimeDtoFromRequest.EndTime ?? showtimeFromDB.EndTime;
        showtimeFromDB.Price = showtimeDtoFromRequest.Price ?? showtimeFromDB.Price;

        if (await context.SaveChangesAsync(ct) > 0)
        {
            logger.LogInformation("Showtime with ID: {Showtime} has been updated successfully", showtimeId);
        }

        else
        {
            logger.LogWarning("Failed updating operation for showtime with ID: {ShowtimeId}", showtimeId);
        }
    }

    public async Task DeleteShowtimeAsync(Guid showtimeId, CancellationToken ct)
    {
        logger.LogInformation("Attemting to delete showtime with ID: {ShowtimeId}.", showtimeId);

        try
        {
            if (!await this.ExistsByIdAsync(showtimeId, ct))
            {
                throw new KeyNotFoundException($"Showtime with ID: {showtimeId} does NOT exist.");
            }

            context.Remove((await context.Showtimes.SingleOrDefaultAsync(st => st.Id == showtimeId, ct))!);

            await context.SaveChangesAsync(ct);

            logger.LogInformation("Showtime with ID: {ShowtimeId} has been deleted successfully", showtimeId);
        }
        catch
        {
            logger.LogError("Error while deleting showtime with ID: {TriedToBeDeletedShowtimeId}", showtimeId);

            throw;
        }
    }

    public async Task<bool> ExistsByIdAsync(Guid showtimeId, CancellationToken ct)
    {
        return await context.Showtimes.AnyAsync(st => st.Id == showtimeId, ct);
    }

    public async Task<bool> CheckIfShowtimeIsUpcomingByIdAsync(Guid showtimeId, CancellationToken ct)
    {
        return await context.Showtimes.AnyAsync(st => st.Id == showtimeId && st.StartTime > DateTime.UtcNow, ct);
    }
}
