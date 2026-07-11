using HotelStay.Api.Domain.Dtos;
using HotelStay.Api.Domain.ProviderContracts;
using HotelStay.Api.Domain.ProviderModels.BudgetNests;

namespace HotelStay.Api.Domain.Providers;

/// <summary>
/// Deterministic BudgetNests provider that returns a minimal snake_case availability response.
/// </summary>
public sealed class BudgetNestsProvider : IHotelProvider
{
    private static readonly IReadOnlyCollection<BudgetNestsRoom> Rooms = new[]
    {
        new BudgetNestsRoom("BN-HYD-STD", "Budget Hyderabad Central", "Hyderabad", "standard", 2900m, "flexible", true),
        new BudgetNestsRoom("BN-HYD-DLX", "Budget Hyderabad Central", "Hyderabad", "deluxe", 3600m, "flexible", false),
        new BudgetNestsRoom("BN-BLR-STD", "Budget Bangalore Central", "Bangalore", "standard", 3100m, "flexible", true),
        new BudgetNestsRoom("BN-BLR-DLX", "Budget Bangalore Central", "Bangalore", "deluxe", 4200m, "non_refundable", false),
        new BudgetNestsRoom("BN-MUM-STD", "Budget Mumbai East", "Mumbai", "standard", 5200m, "flexible", true),
        new BudgetNestsRoom("BN-LON-STD", "Budget London Docklands", "London", "standard", 115m, "flexible", true),
        new BudgetNestsRoom("BN-DXB-DLX", "Budget Dubai Creek", "Dubai", "deluxe", 430m, "non_refundable", false),
        new BudgetNestsRoom("BN-SIN-STE", "Budget Singapore Orchard", "Singapore", "suite", 390m, "flexible", true)
    };

    /// <inheritdoc />
    public string ProviderCode => "BudgetNests";

    /// <inheritdoc />
    public Task<ProviderAvailabilityResult> SearchAsync(HotelSearchRequest request, CancellationToken cancellationToken)
    {
        var matchingRooms = Rooms
            .Where(room => string.Equals(room.Destination, request.Destination, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var response = new BudgetNestsAvailabilityResponse(matchingRooms);
        return Task.FromResult(new ProviderAvailabilityResult(ProviderCode, response));
    }
}