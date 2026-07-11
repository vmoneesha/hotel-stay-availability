using HotelStay.Api.Dtos;

namespace HotelStay.Api.Providers;

public interface IHotelProvider
{
    string ProviderCode { get; }

    Task<ProviderAvailabilityPayload> SearchAsync(HotelSearchCriteria criteria, CancellationToken cancellationToken);
}

public sealed record ProviderAvailabilityPayload(string ProviderCode, object Contract);