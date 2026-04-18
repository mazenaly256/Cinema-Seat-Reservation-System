using System.ComponentModel.DataAnnotations.Schema;

namespace reservation_service.Models;

public class Reservation
{
    public Guid ShowtimeId { get; set; }

    public string SeatNumber { get; set; } = null!;

    [ForeignKey("Payment")]
    public Guid PaymentId { get; set; }

    public Payment? Payment { get; set; }
}
