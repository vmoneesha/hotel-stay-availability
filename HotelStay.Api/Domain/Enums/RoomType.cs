namespace HotelStay.Api.Domain.Enums;

/// <summary>
/// Represents the normalized room categories supported by the hotel availability domain.
/// </summary>
public enum RoomType
{
    /// <summary>
    /// A standard room intended for baseline stay availability.
    /// </summary>
    Standard,

    /// <summary>
    /// A deluxe room with upgraded stay features.
    /// </summary>
    Deluxe,

    /// <summary>
    /// A suite room with premium stay features.
    /// </summary>
    Suite
}