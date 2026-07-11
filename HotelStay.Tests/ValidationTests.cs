using HotelStay.Api.Dtos;
using HotelStay.Api.Services;
using HotelStay.Api.Validation;

namespace HotelStay.Tests;

public sealed class ValidationTests
{
    [Fact]
    public void SearchValidator_ReturnsBadRequestErrorsForMissingRequiredFields()
    {
        var validator = new HotelSearchValidator();

        var result = validator.Validate(new HotelSearchRequest(null, null, null, null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Field == "destination");
        Assert.Contains(result.Errors, error => error.Field == "checkIn");
        Assert.Contains(result.Errors, error => error.Field == "checkOut");
    }

    [Fact]
    public void SearchValidator_RejectsCheckOutOnOrBeforeCheckIn()
    {
        var validator = new HotelSearchValidator();

        var result = validator.Validate(new HotelSearchRequest("Mumbai", "2026-08-10", "2026-08-10", null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Field == "checkOut");
    }

    [Fact]
    public void ReservationValidator_ReturnsUnprocessableEntityWhenInternationalDestinationUsesNationalId()
    {
        var validator = new ReservationValidator();
        var request = ValidReservation("London") with { DocumentType = DocumentType.NationalId };

        var result = validator.Validate(request);

        Assert.False(result.HasBadRequestErrors);
        var error = Assert.Single(result.UnprocessableEntityErrors);
        Assert.Equal("documentType", error.Field);
        Assert.Contains("London requires a valid Passport", error.Message);
    }

    [Theory]
    [InlineData(DocumentType.NationalId)]
    [InlineData(DocumentType.Passport)]
    public void ReservationValidator_AcceptsDomesticNationalIdAndPassport(DocumentType documentType)
    {
        var validator = new ReservationValidator();

        var result = validator.Validate(ValidReservation("Bangalore") with { DocumentType = documentType });

        Assert.False(result.HasBadRequestErrors);
        Assert.False(result.HasUnprocessableEntityErrors);
    }

    [Fact]
    public void ReservationReferenceGenerator_ProducesDeterministicSequentialReferences()
    {
        var generator = new SequentialReservationReferenceGenerator();

        Assert.Equal("HS-000001", generator.Next());
        Assert.Equal("HS-000002", generator.Next());
    }

    private static ReserveRoomRequest ValidReservation(string destination) => new(
        destination,
        "2026-08-10",
        "2026-08-12",
        "PremierStays",
        "PS-HYD-001",
        "PS-HYD-STD",
        RoomType.Standard,
        "Asha Rao",
        DocumentType.NationalId,
        "ID12345",
        6200m);
}