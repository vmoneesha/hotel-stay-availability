using HotelStay.Domain.Dtos;
using HotelStay.Domain.Stores;

namespace HotelStay.Domain.Services;

/// <summary>
/// Creates deterministic reservation confirmations for validated reservation requests.
/// </summary>
public sealed class ReservationService(
    DocumentValidationService documentValidationService,
    IReservationStore reservationStore,
    TimeProvider timeProvider)
{
    private int currentReferenceNumber;

    /// <summary>
    /// Generates a reservation reference and returns a confirmation response.
    /// </summary>
    public ReservationResponse Reserve(ReservationRequest request)
    {
        if (!documentValidationService.IsValidForDestination(request.Destination, request.DocumentType))
        {
            throw new ReservationDocumentValidationException(
                "documentType",
                documentValidationService.GetValidationMessage(request.Destination));
        }

        var nights = request.CheckOut.DayNumber - request.CheckIn.DayNumber;
        var reference = $"HS-{Interlocked.Increment(ref currentReferenceNumber):000000}";

        var reservation = new ReservationResponse(
            reference,
            request.Destination,
            request.CheckIn,
            request.CheckOut,
            request.ProviderCode,
            request.HotelId,
            request.RoomId,
            request.RoomType,
            request.GuestName,
            request.DocumentType,
            request.PerNightPrice,
            decimal.Round(request.PerNightPrice * nights, 2),
            nights,
            timeProvider.GetUtcNow());

        reservationStore.Save(reservation);
        return reservation;
    }

    public ReservationResponse? FindByReference(string reference) => reservationStore.FindByReference(reference);
}