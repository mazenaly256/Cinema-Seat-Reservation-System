using reservation_service.RequestDTOs;

namespace reservation_service.Services.Interfaces;

public interface IReservationService
{
    public Task AddNewReservationAsync(CreateReservationRequestDto reservationDtoFromRequest, CancellationToken ct);
}
