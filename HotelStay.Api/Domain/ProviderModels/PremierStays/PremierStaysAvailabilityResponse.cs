namespace HotelStay.Api.Domain.ProviderModels.PremierStays;

/// <summary>
/// PremierStays provider response containing rich hotel availability data.
/// </summary>
public sealed record PremierStaysAvailabilityResponse(IReadOnlyCollection<PremierStaysHotel> Hotels);