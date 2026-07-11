using HotelStay.Api.Dtos;
using HotelStay.Api.Providers;

namespace HotelStay.Api.Mapping;

public interface IProviderRoomMapper
{
    bool CanMap(string providerCode);

    IReadOnlyCollection<HotelRoomDto> Map(ProviderAvailabilityPayload payload, HotelSearchCriteria criteria);
}