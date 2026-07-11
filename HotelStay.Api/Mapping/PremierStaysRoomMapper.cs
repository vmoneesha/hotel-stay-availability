using HotelStay.Api.Dtos;
using HotelStay.Api.Extensions;
using HotelStay.Api.Providers;

namespace HotelStay.Api.Mapping;

public sealed class PremierStaysRoomMapper : IProviderRoomMapper
{
    public bool CanMap(string providerCode) => string.Equals(providerCode, "PremierStays", StringComparison.OrdinalIgnoreCase);

    public IReadOnlyCollection<HotelRoomDto> Map(ProviderAvailabilityPayload payload, HotelSearchCriteria criteria)
    {
        var response = (PremierStaysAvailabilityResponse)payload.Contract;

        return response.Hotels
            .Where(hotel => string.Equals(hotel.Destination, criteria.Destination, StringComparison.OrdinalIgnoreCase))
            .SelectMany(hotel => hotel.Rooms.Select(room => MapRoom(hotel, room, criteria)))
            .Where(room => criteria.RoomType is null || room.RoomType == criteria.RoomType)
            .ToList();
    }

    private static HotelRoomDto MapRoom(PremierStaysHotel hotel, PremierStaysRoom room, HotelSearchCriteria criteria)
    {
        var roomType = Enum.Parse<RoomType>(room.RoomType, ignoreCase: true);
        return new HotelRoomDto(
            "PremierStays",
            "Premier",
            hotel.HotelId,
            hotel.HotelName,
            hotel.Destination,
            room.RoomId,
            roomType,
            room.NightlyRate,
            room.NightlyRate.TotalForStay(criteria.Nights),
            criteria.Nights,
            room.Amenities,
            hotel.StarRating,
            room.CancellationPolicy);
    }
}