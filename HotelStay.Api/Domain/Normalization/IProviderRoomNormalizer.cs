using HotelStay.Api.Domain.Dtos;
using HotelStay.Api.Domain.ProviderContracts;

namespace HotelStay.Api.Domain.Normalization;

/// <summary>
/// Normalizes one provider-specific availability payload into public hotel room DTOs.
/// </summary>
public interface IProviderRoomNormalizer
{
    /// <summary>
    /// Returns true when this normalizer can handle the provider result payload.
    /// </summary>
    bool CanNormalize(ProviderAvailabilityResult result);

    /// <summary>
    /// Converts provider-specific availability into normalized room DTOs.
    /// </summary>
    IReadOnlyCollection<HotelRoomDto> Normalize(ProviderAvailabilityResult result, HotelSearchRequest request);
}