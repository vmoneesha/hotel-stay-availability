namespace HotelStay.Api.Domain.ProviderModels.PremierStays;

/// <summary>
/// PremierStays hotel model with rich display and rating metadata.
/// </summary>
public sealed record PremierStaysHotel(
    string HotelId,
    string HotelName,
    string Destination,
    int StarRating,
    IReadOnlyCollection<PremierStaysRoom> Rooms);