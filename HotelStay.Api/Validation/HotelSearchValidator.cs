using HotelStay.Api.Dtos;

namespace HotelStay.Api.Validation;

public sealed class HotelSearchValidator : IHotelSearchValidator
{
    public SearchValidationResult Validate(HotelSearchRequest request)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(request.Destination))
        {
            errors.Add(new ValidationError("destination", "Destination is required."));
        }

        var checkIn = ParseDate(request.CheckIn, "checkIn", "Check-in date is required.", errors);
        var checkOut = ParseDate(request.CheckOut, "checkOut", "Check-out date is required.", errors);

        if (checkIn.HasValue && checkOut.HasValue && checkOut.Value <= checkIn.Value)
        {
            errors.Add(new ValidationError("checkOut", "Check-out date must be after check-in date."));
        }

        var criteria = new HotelSearchCriteria(
            request.Destination?.Trim() ?? string.Empty,
            checkIn ?? DateOnly.MinValue,
            checkOut ?? DateOnly.MinValue,
            request.RoomType);

        return new SearchValidationResult(criteria, errors);
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
}