using System.Text.Json.Serialization;

namespace HotelStay.Api.Domain.ProviderModels.BudgetNests;

/// <summary>
/// BudgetNests provider response containing minimal snake_case room availability data.
/// </summary>
public sealed record BudgetNestsAvailabilityResponse(
    [property: JsonPropertyName("available_rooms")] IReadOnlyCollection<BudgetNestsRoom> AvailableRooms);