namespace HotelStay.Domain.ProviderModels.PremierStays;

/// <summary>
/// PremierStays room model with amenities, rate, and cancellation policy details.
/// </summary>
public sealed record PremierStaysRoom(
    string RoomId,
    string RoomType,
    decimal Rate,
    IReadOnlyCollection<string> Amenities,
    string CancellationPolicy);