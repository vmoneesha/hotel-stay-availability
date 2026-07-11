namespace HotelStay.Api.Dtos;

public sealed record HotelSearchRequest(
    string? Destination,
    string? CheckIn,
    string? CheckOut,
    RoomType? RoomType);