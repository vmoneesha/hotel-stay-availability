using HotelStay.Api.Dtos;

namespace HotelStay.Api.Services;

public interface IReservationService
{
    Task<ReservationDto> ReserveAsync(ReservationDraft reservation, CancellationToken cancellationToken);

    Task<ReservationDto?> GetAsync(string reference, CancellationToken cancellationToken);
}