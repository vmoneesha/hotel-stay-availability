using HotelStay.Api.Domain.Dtos;
using HotelStay.Api.Domain.Enums;
using HotelStay.Api.Domain.ProviderContracts;
using HotelStay.Api.Domain.ProviderModels.BudgetNests;
using HotelStay.Api.Domain.ProviderModels.PremierStays;

namespace HotelStay.Api.Domain.Services;

/// <summary>
/// Searches all hotel providers and normalizes provider-specific responses into hotel room DTOs.
/// </summary>
public sealed class HotelSearchService(IEnumerable<IHotelProvider> providers)
{
    /// <summary>
    /// Queries all configured providers and returns normalized, available rooms ordered by total stay price.
    /// </summary>
    public async Task<IReadOnlyCollection<HotelRoomDto>> SearchAsync(HotelSearchRequest request, CancellationToken cancellationToken)
    {
        var rooms = new List<HotelRoomDto>();

        foreach (var provider in providers)
        {
            var result = await provider.SearchAsync(request, cancellationToken);
            rooms.AddRange(Normalize(result, request));
        }

        return rooms
            .Where(room => request.RoomType is null || room.RoomType == request.RoomType)
            .OrderBy(room => room.TotalStayPrice)
            .ThenBy(room => room.ProviderCode, StringComparer.OrdinalIgnoreCase)
            .ThenBy(room => room.HotelName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static IReadOnlyCollection<HotelRoomDto> Normalize(ProviderAvailabilityResult result, HotelSearchRequest request) =>
        result.Payload switch
        {
            PremierStaysAvailabilityResponse premierStays => NormalizePremierStays(result.ProviderCode, premierStays, request),
            BudgetNestsAvailabilityResponse budgetNests => NormalizeBudgetNests(result.ProviderCode, budgetNests, request),
            _ => Array.Empty<HotelRoomDto>()
        };

    private static IReadOnlyCollection<HotelRoomDto> NormalizePremierStays(
        string providerCode,
        PremierStaysAvailabilityResponse response,
        HotelSearchRequest request)
    {
        return response.Hotels
            .SelectMany(hotel => hotel.Rooms.Select(room => new HotelRoomDto(
                providerCode,
                "Premier",
                hotel.HotelId,
                hotel.HotelName,
                hotel.Destination,
                room.RoomId,
                ParseRoomType(room.RoomType),
                room.Rate,
                CalculateTotalStayPrice(room.Rate, request),
                CalculateNights(request),
                room.Amenities,
                hotel.StarRating,
                ParseCancellationPolicy(room.CancellationPolicy),
                room.CancellationPolicy)))
            .ToList();
    }

    private static IReadOnlyCollection<HotelRoomDto> NormalizeBudgetNests(
        string providerCode,
        BudgetNestsAvailabilityResponse response,
        HotelSearchRequest request)
    {
        return response.AvailableRooms
            .Where(room => room.Available)
            .Select(room => new HotelRoomDto(
                providerCode,
                "Budget",
                $"BN-{room.Destination.ToUpperInvariant()}",
                room.HotelName,
                room.Destination,
                room.RoomCode,
                ParseRoomType(room.RoomType),
                room.NightlyRate,
                CalculateTotalStayPrice(room.NightlyRate, request),
                CalculateNights(request),
                Array.Empty<string>(),
                null,
                ParseCancellationPolicy(room.Cancellation),
                NormalizeCancellationDescription(room.Cancellation)))
            .ToList();
    }

    private static RoomType ParseRoomType(string value) => Enum.Parse<RoomType>(value, ignoreCase: true);

    private static CancellationPolicy ParseCancellationPolicy(string value)
    {
        if (value.Contains("non_refundable", StringComparison.OrdinalIgnoreCase) ||
            value.Contains("non-refundable", StringComparison.OrdinalIgnoreCase))
        {
            return CancellationPolicy.NonRefundable;
        }

        if (value.Contains("free", StringComparison.OrdinalIgnoreCase))
        {
            return CancellationPolicy.FreeCancellation;
        }

        if (value.Contains("refund", StringComparison.OrdinalIgnoreCase))
        {
            return CancellationPolicy.Refundable;
        }

        return CancellationPolicy.Flexible;
    }

    private static string NormalizeCancellationDescription(string value) =>
        ParseCancellationPolicy(value) switch
        {
            CancellationPolicy.NonRefundable => "Non-refundable",
            CancellationPolicy.FreeCancellation => "Free cancellation",
            CancellationPolicy.Refundable => "Refundable",
            _ => "Flexible cancellation"
        };

    private static decimal CalculateTotalStayPrice(decimal perNightPrice, HotelSearchRequest request) =>
        decimal.Round(perNightPrice * CalculateNights(request), 2);

    private static int CalculateNights(HotelSearchRequest request) => request.CheckOut.DayNumber - request.CheckIn.DayNumber;
}