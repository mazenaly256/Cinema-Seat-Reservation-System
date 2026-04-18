namespace reservation_service.Models;

public class SeatHold
{
    public Guid ShowtimeId { get; set; }

    public string SeatNumber { get; set; } = null!;

    public DateTime HeldUntil { get; set; }
}
