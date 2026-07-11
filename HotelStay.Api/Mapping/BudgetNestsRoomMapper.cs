using HotelStay.Api.Dtos;
using HotelStay.Api.Extensions;
using HotelStay.Api.Providers;

namespace HotelStay.Api.Mapping;

public sealed class BudgetNestsRoomMapper : IProviderRoomMapper
{
    public bool CanMap(string providerCode) => string.Equals(providerCode, "BudgetNests", StringComparison.OrdinalIgnoreCase);

    public IReadOnlyCollection<HotelRoomDto> Map(ProviderAvailabilityPayload payload, HotelSearchCriteria criteria)
    {
        var response = (BudgetNestsAvailabilityResponse)payload.Contract;

        return response.AvailableRooms
            .Where(room => room.Available)
            .Where(room => string.Equals(room.Destination, criteria.Destination, StringComparison.OrdinalIgnoreCase))
            .Select(room => MapRoom(room, criteria))
            .Where(room => criteria.RoomType is null || room.RoomType == criteria.RoomType)
            .ToList();
    }

    private static HotelRoomDto MapRoom(BudgetNestsRoom room, HotelSearchCriteria criteria)
    {
        var roomType = Enum.Parse<RoomType>(room.RoomType, ignoreCase: true);
        return new HotelRoomDto(
            "BudgetNests",
            "Budget",
            $"BN-{room.Destination.ToUpperInvariant()}",
            room.HotelName,
            room.Destination,
            room.RoomCode,
            roomType,
            room.NightlyRate,
            room.NightlyRate.TotalForStay(criteria.Nights),
            criteria.Nights,
            Array.Empty<string>(),
            null,
            "Flexible cancellation");
    }
}