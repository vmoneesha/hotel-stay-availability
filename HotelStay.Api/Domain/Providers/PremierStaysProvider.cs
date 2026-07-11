using HotelStay.Api.Domain.Dtos;
using HotelStay.Api.Domain.ProviderContracts;
using HotelStay.Api.Domain.ProviderModels.PremierStays;

namespace HotelStay.Api.Domain.Providers;

/// <summary>
/// Deterministic PremierStays provider that returns a rich PascalCase availability response.
/// </summary>
public sealed class PremierStaysProvider : IHotelProvider
{
    private static readonly IReadOnlyCollection<PremierStaysHotel> Hotels = new[]
    {
        new PremierStaysHotel("PS-HYD-001", "Premier Hyderabad Skyline", "Hyderabad", 5, new[]
        {
            new PremierStaysRoom("PS-HYD-STD", "Standard", 6200m, new[] { "Breakfast", "Wi-Fi" }, "Free cancellation until 24 hours before check-in"),
            new PremierStaysRoom("PS-HYD-DLX", "Deluxe", 8400m, new[] { "Breakfast", "Wi-Fi", "Airport pickup" }, "Free cancellation until 48 hours before check-in")
        }),
        new PremierStaysHotel("PS-BLR-001", "Premier Bangalore Garden", "Bangalore", 4, new[]
        {
            new PremierStaysRoom("PS-BLR-STD", "Standard", 5800m, new[] { "Wi-Fi", "Workspace" }, "Free cancellation until 24 hours before check-in"),
            new PremierStaysRoom("PS-BLR-STE", "Suite", 11800m, new[] { "Breakfast", "Club lounge", "Late checkout" }, "Refundable until 72 hours before check-in")
        }),
        new PremierStaysHotel("PS-MUM-001", "Premier Mumbai Bay", "Mumbai", 5, new[]
        {
            new PremierStaysRoom("PS-MUM-DLX", "Deluxe", 9800m, new[] { "Breakfast", "Sea view", "Wi-Fi" }, "Free cancellation until 48 hours before check-in")
        }),
        new PremierStaysHotel("PS-LON-010", "Premier London Regent", "London", 5, new[]
        {
            new PremierStaysRoom("PS-LON-DLX", "Deluxe", 185m, new[] { "Breakfast", "Tube access", "Workspace" }, "Refundable until 72 hours before check-in"),
            new PremierStaysRoom("PS-LON-STE", "Suite", 310m, new[] { "Lounge", "Concierge", "Breakfast" }, "Refundable until 96 hours before check-in")
        }),
        new PremierStaysHotel("PS-DXB-020", "Premier Dubai Marina", "Dubai", 5, new[]
        {
            new PremierStaysRoom("PS-DXB-STE", "Suite", 920m, new[] { "Marina view", "Breakfast", "Spa access" }, "Refundable until 72 hours before check-in")
        }),
        new PremierStaysHotel("PS-SIN-030", "Premier Singapore Quay", "Singapore", 4, new[]
        {
            new PremierStaysRoom("PS-SIN-STD", "Standard", 240m, new[] { "Wi-Fi", "Breakfast" }, "Free cancellation until 24 hours before check-in")
        })
    };

    /// <inheritdoc />
    public string ProviderCode => "PremierStays";

    /// <inheritdoc />
    public Task<ProviderAvailabilityResult> SearchAsync(HotelSearchRequest request, CancellationToken cancellationToken)
    {
        var matchingHotels = Hotels
            .Where(hotel => string.Equals(hotel.Destination, request.Destination, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var response = new PremierStaysAvailabilityResponse(matchingHotels);
        return Task.FromResult(new ProviderAvailabilityResult(ProviderCode, response));
    }
}