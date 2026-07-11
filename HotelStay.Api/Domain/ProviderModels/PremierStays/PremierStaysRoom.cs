namespace HotelStay.Api.Domain.ProviderModels.PremierStays;

/// <summary>
/// PremierStays room model with amenities, nightly rate, and cancellation policy details.
/// </summary>
public sealed record PremierStaysRoom(
    string RoomId,
    string RoomType,
    decimal NightlyRate,
    IReadOnlyCollection<string> Amenities,
    string CancellationPolicy);