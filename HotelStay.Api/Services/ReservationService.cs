using HotelStay.Api.Dtos;
using HotelStay.Api.Repositories;

namespace HotelStay.Api.Services;

public sealed class ReservationService(
    IReservationRepository repository,
    IReservationReferenceGenerator referenceGenerator) : IReservationService
{
    public async Task<ReservationDto> ReserveAsync(ReservationDraft reservation, CancellationToken cancellationToken)
    {
        var confirmed = new ReservationDto(
            referenceGenerator.Next(),
            reservation.Destination,
            reservation.CheckIn,
            reservation.CheckOut,
            reservation.Provider,
            reservation.HotelId,
            reservation.RoomId,
            reservation.RoomType,
            reservation.GuestName,
            reservation.DocumentType,
            reservation.PerNightPrice,
            reservation.TotalStayPrice,
            reservation.Nights,
            DateTimeOffset.UtcNow);

        await repository.SaveAsync(confirmed, cancellationToken);
        return confirmed;
    }

    public Task<ReservationDto?> GetAsync(string reference, CancellationToken cancellationToken) => repository.GetAsync(reference, cancellationToken);
}

public interface IReservationReferenceGenerator
{
    string Next();
}

public sealed class SequentialReservationReferenceGenerator : IReservationReferenceGenerator
{
    private int current;

    public string Next()
    {
        var next = Interlocked.Increment(ref current);
        return $"HS-{next:000000}";
    }
}