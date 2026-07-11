namespace HotelStay.Api.Domain.Enums;

/// <summary>
/// Represents the identity document types accepted by the reservation domain.
/// </summary>
public enum DocumentType
{
    /// <summary>
    /// National identity document accepted for domestic destinations.
    /// </summary>
    NationalId,

    /// <summary>
    /// Passport document required for international destinations.
    /// </summary>
    Passport
}