using HotelStay.Api.Domain.Dtos;
using HotelStay.Api.Domain.ProviderContracts;
using HotelStay.Api.Domain.ProviderModels.PremierStays;

namespace HotelStay.Api.Domain.Normalization;

/// <summary>
/// Normalizes PremierStays rich availability payloads into public room DTOs.
/// </summary>
public sealed class PremierStaysRoomNormalizer : IProviderRoomNormalizer
{
    public bool CanNormalize(ProviderAvailabilityResult result) => result.Payload is PremierStaysAvailabilityResponse;

    public IReadOnlyCollection<HotelRoomDto> Normalize(ProviderAvailabilityResult result, HotelSearchRequest request)
    {
        var response = (PremierStaysAvailabilityResponse)result.Payload;

        return response.Hotels
            .SelectMany(hotel => hotel.Rooms.Select(room => new HotelRoomDto(
                result.ProviderCode,
                "Premier",
                hotel.HotelId,
                hotel.HotelName,
                hotel.Destination,
                room.RoomId,
                ProviderRoomNormalization.ParseRoomType(room.RoomType),
                room.Rate,
                ProviderRoomNormalization.CalculateTotalStayPrice(room.Rate, request),
                ProviderRoomNormalization.CalculateNights(request),
                room.Amenities,
                hotel.StarRating,
                ProviderRoomNormalization.ParseCancellationPolicy(room.CancellationPolicy),
                room.CancellationPolicy)))
            .ToList();
    }
}