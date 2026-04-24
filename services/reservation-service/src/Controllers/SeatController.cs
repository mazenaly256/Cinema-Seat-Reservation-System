using Microsoft.AspNetCore.Mvc;
using reservation_service.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace reservation_service.Controllers;

[ApiController]
[Route("api/seats")]
public class SeatController(ISeatService seatService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSeatsAsync([Required] Guid showtimeId, [Required] bool available, CancellationToken ct)
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

        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
