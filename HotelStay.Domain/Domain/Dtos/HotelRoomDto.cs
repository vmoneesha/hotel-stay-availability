using HotelStay.Domain.Enums;

namespace HotelStay.Domain.Dtos;

/// <summary>
/// Normalized hotel room contract returned to application clients.
/// </summary>
public sealed record HotelRoomDto(
    string ProviderCode,
    string ProviderBadge,
    string HotelId,
    string HotelName,
    string Destination,
    string RoomId,
    RoomType RoomType,
    decimal PerNightPrice,
    decimal TotalStayPrice,
    int Nights,
    IReadOnlyCollection<string> Amenities,
    int? StarRating,
    CancellationPolicy CancellationPolicy,
    string CancellationPolicyDescription);