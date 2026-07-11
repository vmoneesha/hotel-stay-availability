using HotelStay.Api.Dtos;

namespace HotelStay.Api.Repositories;

public interface IReservationRepository
{
    Task SaveAsync(ReservationDto reservation, CancellationToken cancellationToken);

    Task<ReservationDto?> GetAsync(string reference, CancellationToken cancellationToken);
}