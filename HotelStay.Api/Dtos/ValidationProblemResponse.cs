namespace HotelStay.Api.Dtos;

public sealed record ValidationProblemResponse(string Error, IReadOnlyCollection<ValidationError> Details)
{
    public static ValidationProblemResponse From(IReadOnlyCollection<ValidationError> errors) => new("ValidationFailed", errors);
}

public sealed record ValidationError(string Field, string Message);