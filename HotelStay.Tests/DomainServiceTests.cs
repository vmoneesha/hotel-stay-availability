using HotelStay.Api.Domain.Dtos;
using HotelStay.Api.Domain.Enums;
using HotelStay.Api.Domain.Normalization;
using HotelStay.Api.Domain.ProviderContracts;
using HotelStay.Api.Domain.Providers;
using HotelStay.Api.Domain.Services;

namespace HotelStay.Tests;

public sealed class DomainServiceTests
{
    [Theory]
    [InlineData("London")]
    [InlineData("Dubai")]
    [InlineData("Singapore")]
    public void DocumentValidationService_AcceptsPassportForInternationalDestinations(string destination)
    {
        // Arrange
        var service = new DocumentValidationService();

        // Act
        var isValid = service.IsValidForDestination(destination, DocumentType.Passport);

        // Assert
        Assert.True(isValid);
    }

    [Theory]
    [InlineData("London")]
    [InlineData("Dubai")]
    [InlineData("Singapore")]
    public void DocumentValidationService_RejectsNationalIdForInternationalDestinations(string destination)
    {
        // Arrange
        var service = new DocumentValidationService();

        // Act
        var isValid = service.IsValidForDestination(destination, DocumentType.NationalId);
        var message = service.GetValidationMessage(destination);

        // Assert
        Assert.False(isValid);
        Assert.Contains("Passport", message);
    }

    [Theory]
    [InlineData("Hyderabad", DocumentType.NationalId)]
    [InlineData("Hyderabad", DocumentType.Passport)]
    [InlineData("Bangalore", DocumentType.NationalId)]
    [InlineData("Mumbai", DocumentType.Passport)]
    public void DocumentValidationService_AcceptsNationalIdAndPassportForDomesticDestinations(
        string destination,
        DocumentType documentType)
    {
        // Arrange
        var service = new DocumentValidationService();

        // Act
        var isValid = service.IsValidForDestination(destination, documentType);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void DocumentValidationService_ThrowsForUnsupportedDestination()
    {
        // Arrange
        var service = new DocumentValidationService();

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            service.IsValidForDestination("Atlantis", DocumentType.Passport));

        // Assert
        Assert.Contains("Unsupported destination", exception.Message);
    }

    [Fact]
    public async Task HotelSearchService_NormalizesPremierStaysRoomsWithRichProviderData()
    {
        // Arrange
        var service = CreateHotelSearchService();
        var request = SearchRequest("London", new DateOnly(2026, 8, 10), new DateOnly(2026, 8, 13), RoomType.Suite);

        // Act
        var rooms = await service.SearchAsync(request, CancellationToken.None);

        // Assert
        var room = Assert.Single(rooms);
        Assert.Equal("PremierStays", room.ProviderCode);
        Assert.Equal("Premier", room.ProviderBadge);
        Assert.Equal("Premier London Regent", room.HotelName);
        Assert.Equal(RoomType.Suite, room.RoomType);
        Assert.Equal(310m, room.PerNightPrice);
        Assert.Equal(930m, room.TotalStayPrice);
        Assert.Equal(3, room.Nights);
        Assert.Equal(5, room.StarRating);
        Assert.Equal(CancellationPolicy.Refundable, room.CancellationPolicy);
        Assert.Contains("Concierge", room.Amenities);
    }

    [Fact]
    public async Task HotelSearchService_NormalizesBudgetNestsRoomsAndFiltersUnavailableRooms()
    {
        // Arrange
        var service = CreateHotelSearchService();
        var request = SearchRequest("Hyderabad", new DateOnly(2026, 8, 10), new DateOnly(2026, 8, 12), null);

        // Act
        var rooms = await service.SearchAsync(request, CancellationToken.None);

        // Assert
        Assert.Contains(rooms, room => room.RoomId == "BN-HYD-STD" && room.ProviderCode == "BudgetNests");
        Assert.DoesNotContain(rooms, room => room.RoomId == "BN-HYD-DLX");

        var budgetRoom = Assert.Single(rooms, room => room.ProviderCode == "BudgetNests");
        Assert.Equal("Budget", budgetRoom.ProviderBadge);
        Assert.Equal("BN-HYDERABAD", budgetRoom.HotelId);
        Assert.Equal(RoomType.Standard, budgetRoom.RoomType);
        Assert.Equal(CancellationPolicy.Flexible, budgetRoom.CancellationPolicy);
        Assert.Equal("Flexible cancellation", budgetRoom.CancellationPolicyDescription);
        Assert.Empty(budgetRoom.Amenities);
        Assert.Null(budgetRoom.StarRating);
    }

    [Fact]
    public async Task HotelSearchService_FiltersByRequestedRoomType()
    {
        // Arrange
        var service = CreateHotelSearchService();
        var request = SearchRequest("Hyderabad", new DateOnly(2026, 8, 10), new DateOnly(2026, 8, 12), RoomType.Deluxe);

        // Act
        var rooms = await service.SearchAsync(request, CancellationToken.None);

        // Assert
        var room = Assert.Single(rooms);
        Assert.Equal("PS-HYD-DLX", room.RoomId);
        Assert.Equal(RoomType.Deluxe, room.RoomType);
    }

    [Fact]
    public async Task HotelSearchService_OrdersAvailableRoomsByTotalPrice()
    {
        // Arrange
        var service = CreateHotelSearchService();
        var request = SearchRequest("Hyderabad", new DateOnly(2026, 8, 10), new DateOnly(2026, 8, 12), null);

        // Act
        var rooms = await service.SearchAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(new[] { "BN-HYD-STD", "PS-HYD-STD", "PS-HYD-DLX" }, rooms.Select(room => room.RoomId));
    }

    [Fact]
    public async Task HotelSearchService_CalculatesTotalStayPriceForSingleNightBoundary()
    {
        // Arrange
        var service = CreateHotelSearchService();
        var request = SearchRequest("Bangalore", new DateOnly(2026, 8, 10), new DateOnly(2026, 8, 11), RoomType.Standard);

        // Act
        var rooms = await service.SearchAsync(request, CancellationToken.None);

        // Assert
        Assert.All(rooms, room => Assert.Equal(1, room.Nights));
        Assert.Contains(rooms, room => room.RoomId == "BN-BLR-STD" && room.TotalStayPrice == 3100m);
        Assert.Contains(rooms, room => room.RoomId == "PS-BLR-STD" && room.TotalStayPrice == 5800m);
    }

    [Fact]
    public async Task HotelSearchService_ReturnsEmptyCollectionWhenNoProviderHasMatchingDestination()
    {
        // Arrange
        var service = CreateHotelSearchService();
        var request = SearchRequest("Goa", new DateOnly(2026, 8, 10), new DateOnly(2026, 8, 12), null);

        // Act
        var rooms = await service.SearchAsync(request, CancellationToken.None);

        // Assert
        Assert.Empty(rooms);
    }

    [Fact]
    public void ReservationService_ReturnsConfirmationWithDeterministicReferenceAndPrice()
    {
        // Arrange
        var service = new ReservationService(new DocumentValidationService());
        var request = ReservationRequest("Hyderabad", new DateOnly(2026, 8, 10), new DateOnly(2026, 8, 13), DocumentType.Passport, 2900m);

        // Act
        var reservation = service.Reserve(request);

        // Assert
        Assert.Equal("HS-000001", reservation.Reference);
        Assert.Equal("Hyderabad", reservation.Destination);
        Assert.Equal(DocumentType.Passport, reservation.DocumentType);
        Assert.Equal(3, reservation.Nights);
        Assert.Equal(8700m, reservation.TotalStayPrice);
        Assert.Equal(new DateTimeOffset(2026, 7, 11, 0, 0, 0, TimeSpan.Zero), reservation.CreatedAtUtc);
    }

    [Fact]
    public void ReservationService_IncrementsReservationReferencesSequentially()
    {
        // Arrange
        var service = new ReservationService(new DocumentValidationService());

        // Act
        var first = service.Reserve(ReservationRequest("Bangalore", new DateOnly(2026, 8, 10), new DateOnly(2026, 8, 11), DocumentType.NationalId, 3100m));
        var second = service.Reserve(ReservationRequest("Bangalore", new DateOnly(2026, 8, 11), new DateOnly(2026, 8, 12), DocumentType.Passport, 3100m));

        // Assert
        Assert.Equal("HS-000001", first.Reference);
        Assert.Equal("HS-000002", second.Reference);
    }

    [Fact]
    public void ReservationService_RejectsInvalidInternationalDocument()
    {
        // Arrange
        var service = new ReservationService(new DocumentValidationService());
        var request = ReservationRequest("London", new DateOnly(2026, 8, 10), new DateOnly(2026, 8, 12), DocumentType.NationalId, 185m);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => service.Reserve(request));

        // Assert
        Assert.Contains("London requires a valid Passport", exception.Message);
    }

    [Fact]
    public void ReservationService_CalculatesSingleNightBoundaryPrice()
    {
        // Arrange
        var service = new ReservationService(new DocumentValidationService());
        var request = ReservationRequest("Singapore", new DateOnly(2026, 8, 10), new DateOnly(2026, 8, 11), DocumentType.Passport, 240m);

        // Act
        var reservation = service.Reserve(request);

        // Assert
        Assert.Equal(1, reservation.Nights);
        Assert.Equal(240m, reservation.TotalStayPrice);
    }

    private static HotelSearchService CreateHotelSearchService() =>
        new(
            new IHotelProvider[] { new PremierStaysProvider(), new BudgetNestsProvider() },
            new IProviderRoomNormalizer[] { new PremierStaysRoomNormalizer(), new BudgetNestsRoomNormalizer() });

    private static HotelSearchRequest SearchRequest(string destination, DateOnly checkIn, DateOnly checkOut, RoomType? roomType) =>
        new(destination, checkIn, checkOut, roomType);

    private static ReservationRequest ReservationRequest(
        string destination,
        DateOnly checkIn,
        DateOnly checkOut,
        DocumentType documentType,
        decimal perNightPrice) =>
        new(
            destination,
            checkIn,
            checkOut,
            "PremierStays",
            "HOTEL-1",
            "ROOM-1",
            RoomType.Standard,
            "Asha Rao",
            documentType,
            documentType == DocumentType.Passport ? "P1234567" : "N1234567",
            perNightPrice);
}