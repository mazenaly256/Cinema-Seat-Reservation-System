using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using movie_service.Filters;
using movie_service.RequestDTOs;
using movie_service.ResponseDTOs;
using movie_service.Services.Interfaces;

namespace movie_service.Controllers;

[ApiController]
[Route("api/movies")]
[Tags("Movies")]
public class MovieController(IMovieService movieService, IValidator<CreateMovieRequestDto> createMovieRequestDtoValidator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("Get All Movies")]
    [EndpointDescription("Fetch all movies filtered according to specific criteria.")]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("Get Movie By Id")]
    [EndpointDescription("Searches for a movie with the given ID")]
    public async Task<ActionResult<MovieResponseDto>> GetMovieByIdAsync(Guid movieId, CancellationToken ct)
    {
        var movie = await movieService.GetMovieByIdAsync(movieId, ct);

        if (movie is null)
        {
            return NotFound($"Movie with ID: {movieId} is not found.");
        }

        return Ok(movie);
    }



    [AdminOnly]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointName("Add New Movie")]
    [EndpointDescription("Adds a new movie to the cinema's available movies")]
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



    [AdminOnly]
    [HttpPut("{movieId:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointName("Update Movie")]
    [EndpointDescription("Edits movie's data")]
    public async Task<IActionResult> UpdateMovieAsync(Guid movieId, UpdateMovieRequestDto movieFromRequest, CancellationToken ct)
    {
        try
        {
            await movieService.UpdateMovieAsync(movieId, movieFromRequest, ct);

            return NoContent();
        }

        catch(KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }

        catch
        {
            return StatusCode(500, "Unexpected error while updating the movie.");
        }

    }



    [AdminOnly]
    [HttpDelete("{movieId:guid}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointName("Delete Movie")]
    [EndpointDescription("Deletes a movie from the cinema's database")]
    public async Task<IActionResult> DeleteMovieAsync(Guid movieId, CancellationToken ct)
    {
        try
        {
            await movieService.DeleteMovieAsync(movieId, ct);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error while deleting the movie. {ex.Message}");
        }
    }
}
