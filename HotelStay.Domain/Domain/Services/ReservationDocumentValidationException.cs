namespace HotelStay.Domain.Services;

public sealed class ReservationDocumentValidationException(string field, string message) : InvalidOperationException(message)
{
    public string Field { get; } = field;
}