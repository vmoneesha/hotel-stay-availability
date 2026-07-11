using System.Text.Json.Serialization;
using HotelStay.Api.Dtos;

namespace HotelStay.Api.Providers;

public sealed class BudgetNestsProvider : IHotelProvider
{
    public string ProviderCode => "BudgetNests";

    public Task<ProviderAvailabilityPayload> SearchAsync(HotelSearchCriteria criteria, CancellationToken cancellationToken)
    {
        var rooms = new List<BudgetNestsRoom>
        {
            new("BN-BLR-STD", "Budget Bangalore Central", "Bangalore", "standard", 3100m, true),
            new("BN-BLR-DLX", "Budget Bangalore Central", "Bangalore", "deluxe", 4200m, false),
            new("BN-MUM-STD", "Budget Mumbai East", "Mumbai", "standard", 5200m, true),
            new("BN-LON-STD", "Budget London Docklands", "London", "standard", 115m, true),
            new("BN-DXB-DLX", "Budget Dubai Creek", "Dubai", "deluxe", 430m, false),
            new("BN-SIN-STE", "Budget Singapore Orchard", "Singapore", "suite", 390m, true)
        };

        var response = new BudgetNestsAvailabilityResponse(rooms);
        return Task.FromResult(new ProviderAvailabilityPayload(ProviderCode, response));
    }
}

public sealed record BudgetNestsAvailabilityResponse(
    [property: JsonPropertyName("available_rooms")] IReadOnlyCollection<BudgetNestsRoom> AvailableRooms);

public sealed record BudgetNestsRoom(
    [property: JsonPropertyName("room_code")] string RoomCode,
    [property: JsonPropertyName("hotel_name")] string HotelName,
    [property: JsonPropertyName("destination")] string Destination,
    [property: JsonPropertyName("room_type")] string RoomType,
    [property: JsonPropertyName("nightly_rate")] decimal NightlyRate,
    [property: JsonPropertyName("available")] bool Available);