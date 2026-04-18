namespace reservation_service.Models;

public class Payment
{
    public Guid Id { get; set; }

    public decimal PaidAmount { get; set; }

    public DateTime PaidAt { get; set; }

    public Reservation? Reservation { get; set; }
}
