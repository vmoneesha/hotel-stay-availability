using HotelStay.Api.Domain.Enums;

namespace HotelStay.Api.Domain.Dtos;

/// <summary>
/// Response contract returned after a reservation is confirmed.
/// </summary>
public sealed record ReservationResponse(
    string Reference,
    string Destination,
    DateOnly CheckIn,
    DateOnly CheckOut,
    string ProviderCode,
    string HotelId,
    string RoomId,
    RoomType RoomType,
    string GuestName,
    DocumentType DocumentType,
    decimal PerNightPrice,
    decimal TotalStayPrice,
    int Nights,
    DateTimeOffset CreatedAtUtc);