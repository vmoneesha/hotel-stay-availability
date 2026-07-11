namespace HotelStay.Api.Dtos;

public sealed record ReservationDto(
    string Reference,
    string Destination,
    DateOnly CheckIn,
    DateOnly CheckOut,
    string Provider,
    string HotelId,
    string RoomId,
    RoomType RoomType,
    string GuestName,
    DocumentType DocumentType,
    decimal PerNightPrice,
    decimal TotalStayPrice,
    int Nights,
    DateTimeOffset CreatedAtUtc);