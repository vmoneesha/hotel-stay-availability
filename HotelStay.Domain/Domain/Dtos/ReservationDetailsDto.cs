using HotelStay.Domain.Enums;

namespace HotelStay.Domain.Dtos;

/// <summary>
/// Detailed reservation contract returned when looking up an existing reservation.
/// </summary>
public sealed record ReservationDetailsDto(
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
    DateTimeOffset CreatedAtUtc,
    string Status);