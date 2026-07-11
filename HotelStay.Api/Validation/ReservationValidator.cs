using HotelStay.Api.Dtos;
using HotelStay.Api.Extensions;

namespace HotelStay.Api.Validation;

public sealed class ReservationValidator : IReservationValidator
{
    public ReservationValidationResult Validate(ReserveRoomRequest request)
    {
        var badRequestErrors = new List<ValidationError>();
        var unprocessableEntityErrors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(request.Destination))
        {
            badRequestErrors.Add(new ValidationError("destination", "Destination is required."));
        }

        var checkIn = ParseDate(request.CheckIn, "checkIn", "Check-in date is required.", badRequestErrors);
        var checkOut = ParseDate(request.CheckOut, "checkOut", "Check-out date is required.", badRequestErrors);

        if (checkIn.HasValue && checkOut.HasValue && checkOut.Value <= checkIn.Value)
        {
            badRequestErrors.Add(new ValidationError("checkOut", "Check-out date must be after check-in date."));
        }

        AddRequired(request.Provider, "provider", "Provider is required.", badRequestErrors);
        AddRequired(request.HotelId, "hotelId", "Hotel id is required.", badRequestErrors);
        AddRequired(request.RoomId, "roomId", "Room id is required.", badRequestErrors);
        AddRequired(request.GuestName, "guestName", "Guest name is required.", badRequestErrors);
        AddRequired(request.DocumentNumber, "documentNumber", "Document number is required.", badRequestErrors);

        if (request.PerNightPrice <= 0)
        {
            badRequestErrors.Add(new ValidationError("perNightPrice", "Per-night price must be greater than zero."));
        }

        var destination = request.Destination?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(destination) && request.DocumentType != destination.RequiredDocument())
        {
            var expectedDocument = destination.RequiredDocument();
            unprocessableEntityErrors.Add(new ValidationError("documentType", $"{destination} requires {expectedDocument}."));
        }

        var draft = new ReservationDraft(
            destination,
            checkIn ?? DateOnly.MinValue,
            checkOut ?? DateOnly.MinValue,
            request.Provider?.Trim() ?? string.Empty,
            request.HotelId?.Trim() ?? string.Empty,
            request.RoomId?.Trim() ?? string.Empty,
            request.RoomType,
            request.GuestName?.Trim() ?? string.Empty,
            request.DocumentType,
            request.DocumentNumber?.Trim() ?? string.Empty,
            request.PerNightPrice);

        return new ReservationValidationResult(draft, badRequestErrors, unprocessableEntityErrors);
    }

    private static DateOnly? ParseDate(string? value, string field, string missingMessage, List<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(new ValidationError(field, missingMessage));
            return null;
        }

        if (!DateOnly.TryParse(value, out var parsed))
        {
            errors.Add(new ValidationError(field, "Date must be a valid ISO date."));
            return null;
        }

        return parsed;
    }

    private static void AddRequired(string? value, string field, string message, List<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(new ValidationError(field, message));
        }
    }
}