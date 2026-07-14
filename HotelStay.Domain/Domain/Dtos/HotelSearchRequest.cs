using HotelStay.Domain.Enums;

namespace HotelStay.Domain.Dtos;

/// <summary>
/// Request contract for searching hotel room availability.
/// </summary>
public sealed record HotelSearchRequest(
    string Destination,
    DateOnly CheckIn,
    DateOnly CheckOut,
    RoomType? RoomType);