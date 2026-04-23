using reservation_service.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace reservation_service.RequestDTOs;

public class CreateReservationRequestDto
{
    public Guid ShowtimeId { get; set; }

    public string SeatNumber { get; set; } = null!;
}
