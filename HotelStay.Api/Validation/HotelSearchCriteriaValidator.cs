using HotelStay.Api.Dtos;

namespace HotelStay.Api.Validation;

public sealed class HotelSearchCriteriaValidator
{
    public HotelSearchValidationResult Validate(string? destination, string? checkIn, string? checkOut)
    {
        var errors = new List<ValidationError>();
        var parsedCheckIn = DateOnly.MinValue;
        var parsedCheckOut = DateOnly.MinValue;

        if (string.IsNullOrWhiteSpace(destination))
        {
            errors.Add(new ValidationError("destination", "Destination is required."));
        }

        if (string.IsNullOrWhiteSpace(checkIn))
        {
            errors.Add(new ValidationError("checkIn", "Check-in date is required."));
        }
        else if (!DateOnly.TryParse(checkIn, out parsedCheckIn))
        {
            errors.Add(new ValidationError("checkIn", "Check-in date must be a valid ISO date."));
        }

        if (string.IsNullOrWhiteSpace(checkOut))
        {
            errors.Add(new ValidationError("checkOut", "Check-out date is required."));
        }
        else if (!DateOnly.TryParse(checkOut, out parsedCheckOut))
        {
            errors.Add(new ValidationError("checkOut", "Check-out date must be a valid ISO date."));
        }

        if (parsedCheckIn != DateOnly.MinValue && parsedCheckOut != DateOnly.MinValue && parsedCheckOut <= parsedCheckIn)
        {
            errors.Add(new ValidationError("checkOut", "Check-out date must be after check-in date."));
        }

        return new HotelSearchValidationResult(errors, parsedCheckIn, parsedCheckOut);
    }
}

public sealed record HotelSearchValidationResult(
    IReadOnlyCollection<ValidationError> Errors,
    DateOnly CheckIn,
    DateOnly CheckOut)
{
    public bool IsValid => Errors.Count == 0;
}