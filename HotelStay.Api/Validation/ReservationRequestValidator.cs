using HotelStay.Domain.Dtos;
using HotelStay.Api.Dtos;

namespace HotelStay.Api.Validation;

public sealed class ReservationRequestValidator
{
    public IReadOnlyCollection<ValidationError> Validate(ReservationRequest request)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(request.Destination))
        {
            errors.Add(new ValidationError("destination", "Destination is required."));
        }

        if (string.IsNullOrWhiteSpace(request.ProviderCode))
        {
            errors.Add(new ValidationError("providerCode", "Provider code is required."));
        }

        if (string.IsNullOrWhiteSpace(request.HotelId))
        {
            errors.Add(new ValidationError("hotelId", "Hotel id is required."));
        }

        if (string.IsNullOrWhiteSpace(request.RoomId))
        {
            errors.Add(new ValidationError("roomId", "Room id is required."));
        }

        if (string.IsNullOrWhiteSpace(request.GuestName))
        {
            errors.Add(new ValidationError("guestName", "Guest name is required."));
        }

        if (string.IsNullOrWhiteSpace(request.DocumentNumber))
        {
            errors.Add(new ValidationError("documentNumber", "Document number is required."));
        }

        if (request.PerNightPrice <= 0)
        {
            errors.Add(new ValidationError("perNightPrice", "Per-night price must be greater than zero."));
        }

        if (request.CheckIn == DateOnly.MinValue)
        {
            errors.Add(new ValidationError("checkIn", "Check-in date is required."));
        }

        if (request.CheckOut == DateOnly.MinValue)
        {
            errors.Add(new ValidationError("checkOut", "Check-out date is required."));
        }

        if (request.CheckIn != DateOnly.MinValue && request.CheckOut != DateOnly.MinValue && request.CheckOut <= request.CheckIn)
        {
            errors.Add(new ValidationError("checkOut", "Check-out date must be after check-in date."));
        }

        return errors;
    }
}