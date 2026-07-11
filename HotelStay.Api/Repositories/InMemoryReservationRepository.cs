using System.Collections.Concurrent;
using HotelStay.Api.Dtos;

namespace HotelStay.Api.Repositories;

public sealed class InMemoryReservationRepository : IReservationRepository
{
    private readonly ConcurrentDictionary<string, ReservationDto> reservations = new(StringComparer.OrdinalIgnoreCase);

    public Task SaveAsync(ReservationDto reservation, CancellationToken cancellationToken)
    {
        reservations[reservation.Reference] = reservation;
        return Task.CompletedTask;
    }

    public Task<ReservationDto?> GetAsync(string reference, CancellationToken cancellationToken)
    {
        reservations.TryGetValue(reference, out var reservation);
        return Task.FromResult(reservation);
    }
}