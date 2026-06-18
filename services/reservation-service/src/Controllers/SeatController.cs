using Microsoft.AspNetCore.Mvc;
using reservation_service.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace reservation_service.Controllers;

[ApiController]
[Route("api/seats")]
[Tags("Seats")]
public class SeatController(ISeatService seatService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("Get Seats")]
    [EndpointDescription("Gets available or non available seats")]
    public async Task<ActionResult<IEnumerable<string>>> GetSeatsAsync([Required] Guid showtimeId, [Required] bool available, CancellationToken ct)
    {
        try
        {
            if (available)
            {
                var availableSeats = await seatService.GetAvailableSeatsAsync(showtimeId, ct);

                return Ok(availableSeats);
            }

            else
            {
                var unavailableSeats = await seatService.GetReservedAndLockedSeatsAsync(showtimeId, ct);

                return Ok(unavailableSeats);
            }
        }

        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }

        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
