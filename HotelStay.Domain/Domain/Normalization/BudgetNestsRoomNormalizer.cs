using HotelStay.Domain.Dtos;
using HotelStay.Domain.ProviderContracts;
using HotelStay.Domain.ProviderModels.BudgetNests;

namespace HotelStay.Domain.Normalization;

/// <summary>
/// Normalizes BudgetNests minimal availability payloads into public room DTOs.
/// </summary>
public sealed class BudgetNestsRoomNormalizer : IProviderRoomNormalizer
{
    public bool CanNormalize(ProviderAvailabilityResult result) => result.Payload is BudgetNestsAvailabilityResponse;

    public IReadOnlyCollection<HotelRoomDto> Normalize(ProviderAvailabilityResult result, HotelSearchRequest request)
    {
        var response = (BudgetNestsAvailabilityResponse)result.Payload;

        return response.AvailableRooms
            .Where(room => room.Available)
            .Select(room => new HotelRoomDto(
                result.ProviderCode,
                "Budget",
                $"BN-{room.Destination.ToUpperInvariant()}",
                room.HotelName,
                room.Destination,
                room.RoomCode,
                ProviderRoomNormalization.ParseRoomType(room.RoomType),
                room.NightlyRate,
                ProviderRoomNormalization.CalculateTotalStayPrice(room.NightlyRate, request),
                ProviderRoomNormalization.CalculateNights(request),
                Array.Empty<string>(),
                null,
                ProviderRoomNormalization.ParseCancellationPolicy(room.Cancellation),
                ProviderRoomNormalization.NormalizeCancellationDescription(room.Cancellation)))
            .ToList();
    }
}