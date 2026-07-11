using System.Text.Json.Serialization;

namespace HotelStay.Api.Domain.ProviderModels.BudgetNests;

/// <summary>
/// BudgetNests room model with minimal room, rate, and availability details.
/// </summary>
public sealed record BudgetNestsRoom(
    [property: JsonPropertyName("room_code")] string RoomCode,
    [property: JsonPropertyName("hotel_name")] string HotelName,
    [property: JsonPropertyName("destination")] string Destination,
    [property: JsonPropertyName("room_type")] string RoomType,
    [property: JsonPropertyName("nightly_rate")] decimal NightlyRate,
    [property: JsonPropertyName("available")] bool Available);