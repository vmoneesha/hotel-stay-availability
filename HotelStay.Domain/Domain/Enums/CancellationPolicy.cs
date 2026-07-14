namespace HotelStay.Domain.Enums;

/// <summary>
/// Represents the normalized cancellation policies exposed to application clients.
/// </summary>
public enum CancellationPolicy
{
    /// <summary>
    /// The room can be cancelled flexibly under provider-defined rules.
    /// </summary>
    Flexible,

    /// <summary>
    /// The room can be cancelled for free before the provider-defined deadline.
    /// </summary>
    FreeCancellation,

    /// <summary>
    /// The room is refundable before the provider-defined deadline.
    /// </summary>
    Refundable,

    /// <summary>
    /// The room cannot be refunded after reservation.
    /// </summary>
    NonRefundable
}