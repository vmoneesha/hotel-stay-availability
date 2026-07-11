namespace HotelStay.Api.Dtos;

public sealed record ReserveRoomRequest(
    string? Destination,
    string? CheckIn,
    string? CheckOut,
    string? Provider,
    string? HotelId,
    string? RoomId,
    RoomType RoomType,
    string? GuestName,
    DocumentType DocumentType,
    string? DocumentNumber,
    decimal PerNightPrice);