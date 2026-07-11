using HotelStay.Api.Dtos;
using HotelStay.Api.Mapping;
using HotelStay.Api.Providers;

namespace HotelStay.Api.Services;

public sealed class HotelAvailabilityService(
    IEnumerable<IHotelProvider> providers,
    IEnumerable<IProviderRoomMapper> mappers) : IHotelAvailabilityService
{
    public async Task<IReadOnlyCollection<HotelRoomDto>> SearchAsync(HotelSearchCriteria criteria, CancellationToken cancellationToken)
    {
        var rooms = new List<HotelRoomDto>();

        foreach (var provider in providers)
        {
            var payload = await provider.SearchAsync(criteria, cancellationToken);
            var mapper = mappers.Single(candidate => candidate.CanMap(payload.ProviderCode));
            rooms.AddRange(mapper.Map(payload, criteria));
        }

        return rooms.OrderBy(room => room.TotalStayPrice).ThenBy(room => room.Provider).ToList();
    }
}