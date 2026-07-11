using HotelStay.Api.Dtos;

namespace HotelStay.Api.Services;

public interface IHotelAvailabilityService
{
    Task<IReadOnlyCollection<HotelRoomDto>> SearchAsync(HotelSearchCriteria criteria, CancellationToken cancellationToken);
}