using HotelStay.Api.Dtos;

namespace HotelStay.Api.Validation;

public interface IHotelSearchValidator
{
    SearchValidationResult Validate(HotelSearchRequest request);
}

public sealed record SearchValidationResult(HotelSearchCriteria Criteria, IReadOnlyCollection<ValidationError> Errors)
{
    public bool IsValid => Errors.Count == 0;
}