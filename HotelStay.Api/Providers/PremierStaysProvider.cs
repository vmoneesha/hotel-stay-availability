using HotelStay.Api.Dtos;

namespace HotelStay.Api.Providers;

public sealed class PremierStaysProvider : IHotelProvider
{
    public string ProviderCode => "PremierStays";

    public Task<ProviderAvailabilityPayload> SearchAsync(HotelSearchCriteria criteria, CancellationToken cancellationToken)
    {
        var hotels = new List<PremierStaysHotel>
        {
            new("PS-HYD-001", "Premier Hyderabad Skyline", "Hyderabad", 5, new[]
            {
                new PremierStaysRoom("PS-HYD-STD", "Standard", 6200m, new[] { "Breakfast", "Wi-Fi" }, "Free cancellation until 24 hours before check-in"),
                new PremierStaysRoom("PS-HYD-DLX", "Deluxe", 8400m, new[] { "Breakfast", "Wi-Fi", "Airport pickup" }, "Free cancellation until 48 hours before check-in")
            }),
            new("PS-LON-010", "Premier London Regent", "London", 5, new[]
            {
                new PremierStaysRoom("PS-LON-DLX", "Deluxe", 185m, new[] { "Breakfast", "Tube access", "Workspace" }, "Refundable until 72 hours before check-in"),
                new PremierStaysRoom("PS-LON-STE", "Suite", 310m, new[] { "Lounge", "Concierge", "Breakfast" }, "Refundable until 96 hours before check-in")
            }),
            new("PS-DXB-020", "Premier Dubai Marina", "Dubai", 5, new[]
            {
                new PremierStaysRoom("PS-DXB-STE", "Suite", 920m, new[] { "Marina view", "Breakfast", "Spa access" }, "Refundable until 72 hours before check-in")
            }),
            new("PS-SIN-030", "Premier Singapore Quay", "Singapore", 4, new[]
            {
                new PremierStaysRoom("PS-SIN-STD", "Standard", 240m, new[] { "Wi-Fi", "Breakfast" }, "Free cancellation until 24 hours before check-in")
            })
        };

        var response = new PremierStaysAvailabilityResponse(hotels);
        return Task.FromResult(new ProviderAvailabilityPayload(ProviderCode, response));
    }
}

public sealed record PremierStaysAvailabilityResponse(IReadOnlyCollection<PremierStaysHotel> Hotels);

public sealed record PremierStaysHotel(
    string HotelId,
    string HotelName,
    string Destination,
    int StarRating,
    IReadOnlyCollection<PremierStaysRoom> Rooms);

public sealed record PremierStaysRoom(
    string RoomId,
    string RoomType,
    decimal NightlyRate,
    IReadOnlyCollection<string> Amenities,
    string CancellationPolicy);