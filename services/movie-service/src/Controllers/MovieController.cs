using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using movie_service.RequestDTOs;
using movie_service.ResponseDTOs;
using movie_service.Services.Interfaces;

namespace movie_service.Controllers;

[ApiController]
[Route("api/movies")]
public class MovieController(IMovieService movieService, IValidator<CreateMovieRequestDto> createMovieRequestDtoValidator) : ControllerBase
{
    [HttpGet]
    public IActionResult GetMovies(string? status, CancellationToken ct)
    {
        if (status  is null)
        {
            // retrieve all the movies recorded in the system
            var movies = movieService.GetAllMovies(ct);
            return Ok(movies);
        }

        else if (status == "now-showing")
        {
            // retrieve movies that have current or future showtimes
            var movies = movieService.GetMoviesWithUpcomingShowtimes(ct);
            return Ok(movies);
        }

        return BadRequest("Unsupported status for movies retrieval. Allowed values are: 'now-showing'.");
    }


    [HttpGet("{movieId:guid}", Name = "GetMovieByIdAsync")]
    public async Task<IActionResult> GetMovieByIdAsync(Guid movieId, CancellationToken ct)
    {
        var movie = await movieService.GetMovieByIdAsync(movieId, ct);

        if (movie is null)
        {
            return NotFound($"Movie with ID: {movieId} is not found.");
        }

        return Ok(movie);
    }


    [HttpPost]
    public async Task<IActionResult> CreateMovieAsync(CreateMovieRequestDto movieFromRequest, CancellationToken ct)
    {
        var validationResult = await createMovieRequestDtoValidator.ValidateAsync(movieFromRequest, ct);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        if (await movieService.ExistsByNameAsync(movieFromRequest.MovieName, ct))
        {
            return Conflict($"Movie with name '{movieFromRequest.MovieName}' already exists.");
        }

        var newMovieId = await movieService.AddNewMovieAsync(movieFromRequest, ct);

        if (newMovieId is null)
        {
            return StatusCode(500, "Error while saving movie.");
        }

        return CreatedAtRoute(routeName: nameof(GetMovieByIdAsync), routeValues: new { movieId = newMovieId }, value: await movieService.GetMovieByIdAsync((Guid)newMovieId, ct));
    }


    [HttpPut("{movieId:guid}")]
    public async Task<IActionResult> UpdateMovieAsync(Guid movieId, UpdateMovieRequestDto movieFromRequest, CancellationToken ct)
    {
        try
        {
            if (!await movieService.ExistsByIdAsync(movieId, ct))
            {
                return NotFound($"Movie with ID: {movieId} does NOT exist.");
            }

            await movieService.UpdateMovieAsync(movieId, movieFromRequest, ct);

            return NoContent();
        }

        catch
        {
            return StatusCode(500, "Unexpected error while updating the movie.");
        }

    }



    [HttpDelete("{movieId:guid}")]
    public async Task<IActionResult> DeleteMovieAsync(Guid movieId, CancellationToken ct)
    {
        try
        {
            await movieService.DeleteMovieAsync(movieId, ct);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest($"{ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error while deleting the movie. {ex.Message}");
        }
    }
}
