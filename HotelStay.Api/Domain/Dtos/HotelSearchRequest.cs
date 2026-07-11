using HotelStay.Api.Domain.Enums;

namespace HotelStay.Api.Domain.Dtos;

/// <summary>
/// Request contract for searching hotel room availability.
/// </summary>
public sealed record HotelSearchRequest(
    string Destination,
    DateOnly CheckIn,
    DateOnly CheckOut,
    RoomType? RoomType);