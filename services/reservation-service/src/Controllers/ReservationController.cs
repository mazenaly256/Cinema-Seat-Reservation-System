using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using reservation_service.RequestDTOs;
using reservation_service.Services.Interfaces;

namespace reservation_service.Controllers;

[ApiController]
[Route("api/reservations")]
[Tags("Reservations")]
public class ReservationController(IReservationService reservationService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointName("Reserve Seat")]
    [EndpointDescription("Reserve a seat for a specific showtime")]
    public async Task<IActionResult> ReserveSeatAsync(CreateReservationRequestDto reservationDtoFromRequest, CancellationToken ct)
    {
        try
        {
            await reservationService.AddNewReservationAsync(reservationDtoFromRequest, ct);

            return Created();
        }

        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        catch (InvalidOperationException ex) when (ex.Message.Contains("Reservation Conflict."))
        {
            return Conflict(ex.Message);
        }

        catch (InvalidOperationException ex)
        {
            return StatusCode(500, ex.Message);
        }

        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
