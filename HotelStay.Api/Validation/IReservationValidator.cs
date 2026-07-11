using HotelStay.Api.Dtos;

namespace HotelStay.Api.Validation;

public interface IReservationValidator
{
    ReservationValidationResult Validate(ReserveRoomRequest request);
}

public sealed record ReservationValidationResult(
    ReservationDraft Reservation,
    IReadOnlyCollection<ValidationError> BadRequestErrors,
    IReadOnlyCollection<ValidationError> UnprocessableEntityErrors)
{
    public bool HasBadRequestErrors => BadRequestErrors.Count > 0;

    public bool HasUnprocessableEntityErrors => UnprocessableEntityErrors.Count > 0;
}