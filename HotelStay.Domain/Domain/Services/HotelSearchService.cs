using HotelStay.Domain.Dtos;
using HotelStay.Domain.Normalization;
using HotelStay.Domain.ProviderContracts;

namespace HotelStay.Domain.Services;

/// <summary>
/// Searches all hotel providers and normalizes provider-specific responses into hotel room DTOs.
/// </summary>
public sealed class HotelSearchService(
    IEnumerable<IHotelProvider> providers,
    IEnumerable<IProviderRoomNormalizer> normalizers)
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
            var normalizer = normalizers.FirstOrDefault(candidate => candidate.CanNormalize(result));
            if (normalizer is null)
            {
                throw new InvalidOperationException($"No room normalizer is registered for provider '{result.ProviderCode}'.");
            }

            rooms.AddRange(normalizer.Normalize(result, request));
        }

        return rooms
            .Where(room => request.RoomType is null || room.RoomType == request.RoomType)
            .OrderBy(room => room.TotalStayPrice)
            .ThenBy(room => room.ProviderCode, StringComparer.OrdinalIgnoreCase)
            .ThenBy(room => room.HotelName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}