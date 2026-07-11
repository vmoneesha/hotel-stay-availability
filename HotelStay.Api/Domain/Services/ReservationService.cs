using HotelStay.Api.Domain.Dtos;

namespace HotelStay.Api.Domain.Services;

/// <summary>
/// Creates deterministic reservation confirmations for validated reservation requests.
/// </summary>
public sealed class ReservationService(DocumentValidationService documentValidationService)
{
    private static readonly DateTimeOffset DeterministicCreatedAtUtc = new(2026, 7, 11, 0, 0, 0, TimeSpan.Zero);
    private int currentReferenceNumber;

    /// <summary>
    /// Generates a reservation reference and returns a confirmation response.
    /// </summary>
    public ReservationResponse Reserve(ReservationRequest request)
    {
        if (!documentValidationService.IsValidForDestination(request.Destination, request.DocumentType))
        {
            throw new InvalidOperationException(documentValidationService.GetValidationMessage(request.Destination));
        }

        var nights = request.CheckOut.DayNumber - request.CheckIn.DayNumber;
        var reference = $"HS-{Interlocked.Increment(ref currentReferenceNumber):000000}";

        return new ReservationResponse(
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
            DeterministicCreatedAtUtc);
    }
}