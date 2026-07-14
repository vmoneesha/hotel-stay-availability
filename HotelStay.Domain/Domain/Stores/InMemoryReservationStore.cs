using System.Collections.Concurrent;
using HotelStay.Domain.Dtos;

namespace HotelStay.Domain.Stores;

/// <summary>
/// In-memory reservation store for the offline case-study runtime.
/// </summary>
public sealed class InMemoryReservationStore : IReservationStore
{
    private readonly ConcurrentDictionary<string, ReservationResponse> reservations = new(StringComparer.OrdinalIgnoreCase);

    public void Save(ReservationResponse reservation) => reservations[reservation.Reference] = reservation;

    public ReservationResponse? FindByReference(string reference) =>
        reservations.TryGetValue(reference, out var reservation) ? reservation : null;
}