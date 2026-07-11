using HotelStay.Api.Domain.Enums;

namespace HotelStay.Api.Domain.Dtos;

/// <summary>
/// Request contract for reserving a selected hotel room.
/// </summary>
public sealed record ReservationRequest(
    string Destination,
    DateOnly CheckIn,
    DateOnly CheckOut,
    string ProviderCode,
    string HotelId,
    string RoomId,
    RoomType RoomType,
    string GuestName,
    DocumentType DocumentType,
    string DocumentNumber,
    decimal PerNightPrice);