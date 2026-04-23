using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using reservation_service.RequestDTOs;
using reservation_service.Services.Interfaces;

namespace reservation_service.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationController(IReservationService reservationService) : ControllerBase
{
    [HttpPost]
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

        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }

        catch (Exception ex)
        {
            return StatusCode(500);
        }
    }
}
