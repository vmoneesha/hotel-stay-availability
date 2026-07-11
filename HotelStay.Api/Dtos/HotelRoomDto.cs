namespace HotelStay.Api.Dtos;

public sealed record HotelRoomDto(
    string Provider,
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
    string CancellationPolicy);