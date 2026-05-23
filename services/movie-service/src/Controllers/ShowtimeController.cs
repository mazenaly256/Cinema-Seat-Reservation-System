using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using movie_service.Filters;
using movie_service.RequestDTOs;
using movie_service.ResponseDTOs;
using movie_service.Services.Implementations;
using movie_service.Services.Interfaces;
using movie_service.Validators;

namespace movie_service.Controllers;

[ApiController]
[Route("api/showtimes")]
[Tags("Showtimes")]
public class ShowtimeController(IShowtimeService showtimeService, IValidator<CreateShowtimeRequestDto> createShowtimeRequestDtoValidator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("Get All Showtimes")]
    [EndpointDescription("Fetch all showtimes filtered according to specific criteria.")]
    public IActionResult GetShowtimes(Guid? movieId, DateTime? from, DateTime? to, string? status)
    {
        if (status is not null && status != "upcoming")
        {
            return BadRequest("Unsupported status for showtimes retrieval. Allowed values are: 'upcoming'.");
        }

        var showtimes = showtimeService.GetShowtimes(movieId, from, to, status);

        return Ok(showtimes);
    }


    [HttpGet("{showtimeId:guid}", Name = "GetShowtimeById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("Get Showtime By ID")]
    [EndpointDescription("Searches for a showtime with the given ID")]
    public async Task<ActionResult<ShowtimeResponseDto>> GetShowtimeByIdAsync(Guid showtimeId, CancellationToken ct)
    {
        var showtime = await showtimeService.GetShowtimeByIdAsync(showtimeId, ct);

        if (showtime is null)
        {
            return NotFound($"Showtime with ID: {showtimeId} is not found.");
        }

        return Ok(showtime);
    }


    [HttpHead("{showtimeId:guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [EndpointName("Check Showtime's Existence")]
    [EndpointDescription("Checks if a showtime with specific ID exists")]
    public async Task<IActionResult> CheckExistenceOfUpcomingShowtimeAsync(Guid showtimeId, CancellationToken ct)
    {
        return (await showtimeService.CheckIfShowtimeIsUpcomingByIdAsync(showtimeId, ct)) ? Ok() : NotFound();
    }



    [AdminOnly]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointName("Schedule New Showtime")]
    [EndpointDescription("Adds a new showtime to the cinema's schedule")]
    public async Task<IActionResult> CreateShowtimeAsync(CreateShowtimeRequestDto showtimeDtoFromRequest, CancellationToken ct)
    {
        var validationResult = await createShowtimeRequestDtoValidator.ValidateAsync(showtimeDtoFromRequest, ct);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        try
        {
            var newShowtimeId = await showtimeService.AddNewShowtimeAsync(showtimeDtoFromRequest, ct);

            if (newShowtimeId is null)
            {
                return StatusCode(500, "error while creating new showtime");
            }

            return CreatedAtRoute("GetShowtimeById", new {showtimeId = newShowtimeId}, await showtimeService.GetShowtimeByIdAsync((Guid)newShowtimeId, ct));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest($"Invalid data. {ex.Message}");
        }
    }



    [AdminOnly]
    [HttpPut("{showtimeId:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointName("Update Showtime")]
    [EndpointDescription("Edits showtime's data")]
    public async Task<IActionResult> UpdateShowtimeAsync(Guid showtimeId, UpdateShowtimeRequestDto showtimeDtoFromRequest, CancellationToken ct)
    {
        try
        {
            if (!await showtimeService.ExistsByIdAsync(showtimeId, ct))
            {
                return NotFound($"Movie with ID: {showtimeId} does NOT exist.");
            }

            await showtimeService.UpdateShowtimeAsync(showtimeId, showtimeDtoFromRequest, ct);

            return NoContent();
        }
        catch(InvalidOperationException ex)
        {
            return StatusCode(500, $"Unexpected error while updating the movie. {ex.Message}");
        }

    }



    [AdminOnly]
    [HttpDelete("{showtimeId:guid}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointName("Delete Showtime")]
    [EndpointDescription("Deletes a showtime from the cinema's schedule")]
    public async Task<IActionResult> DeleteShowtimeAsync(Guid showtimeId, CancellationToken ct)
    {
        if (!await showtimeService.ExistsByIdAsync(showtimeId, ct))
        {
            return BadRequest($"Showtime with ID: {showtimeId} does NOT exist.");
        }

        try
        {
            await showtimeService.DeleteShowtimeAsync(showtimeId, ct);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, $"Error while deleting the movie. {ex.Message}");
        }
    }
}
