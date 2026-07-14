using HotelStay.Domain.Dtos;

namespace HotelStay.Domain.ProviderContracts;

/// <summary>
/// Contract implemented by deterministic hotel availability providers.
/// </summary>
public interface IHotelProvider
{
    /// <summary>
    /// Gets the stable provider code used for mapping and diagnostics.
    /// </summary>
    string ProviderCode { get; }

    /// <summary>
    /// Searches provider-specific availability data for the supplied criteria.
    /// </summary>
    Task<ProviderAvailabilityResult> SearchAsync(HotelSearchRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Provider availability result that preserves the provider-specific response payload behind a stable provider code.
/// </summary>
public sealed record ProviderAvailabilityResult(string ProviderCode, object Payload);