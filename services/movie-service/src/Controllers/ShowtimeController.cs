using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using movie_service.RequestDTOs;
using movie_service.ResponseDTOs;
using movie_service.Services.Implementations;
using movie_service.Services.Interfaces;
using movie_service.Validators;

namespace movie_service.Controllers;

[ApiController]
[Route("api/showtimes")]
public class ShowtimeController(IShowtimeService showtimeService, IValidator<CreateShowtimeRequestDto> createShowtimeRequestDtoValidator) : ControllerBase
{
    [HttpGet]
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
    public async Task<IActionResult> CheckExistenceOfUpcomingShowtimeAsync(Guid showtimeId, CancellationToken ct)
    {
        return (await showtimeService.CheckIfShowtimeIsUpcomingByIdAsync(showtimeId, ct)) ? Ok() : NotFound();
    }


    [HttpPost]
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


    [HttpPut("{showtimeId:guid}")]
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


    [HttpDelete("{showtimeId:guid}")]
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
