namespace reservation_service.Custom_Exceptions;

public class ReservationConflictException : Exception
{
    public ReservationConflictException(string message) : base(message)
    {
    }
}
