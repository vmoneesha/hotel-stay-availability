using HotelStay.Domain.Dtos;

namespace HotelStay.Domain.Stores;

/// <summary>
/// Stores confirmed reservations for lookup by reference.
/// </summary>
public interface IReservationStore
{
    void Save(ReservationResponse reservation);

    ReservationResponse? FindByReference(string reference);
}