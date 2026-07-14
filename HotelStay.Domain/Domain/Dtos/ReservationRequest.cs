using HotelStay.Domain.Enums;

namespace HotelStay.Domain.Dtos;

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