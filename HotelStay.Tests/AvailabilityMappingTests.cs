using HotelStay.Api.Dtos;
using HotelStay.Api.Mapping;
using HotelStay.Api.Providers;

namespace HotelStay.Tests;

public sealed class AvailabilityMappingTests
{
    [Fact]
    public async Task PremierStaysMapper_NormalizesRichContractAndCalculatesTotalPrice()
    {
        var provider = new PremierStaysProvider();
        var mapper = new PremierStaysRoomMapper();
        var criteria = new HotelSearchCriteria("London", new DateOnly(2026, 8, 10), new DateOnly(2026, 8, 13), RoomType.Suite);

        var payload = await provider.SearchAsync(criteria, CancellationToken.None);
        var rooms = mapper.Map(payload, criteria);

        var room = Assert.Single(rooms);
        Assert.Equal("PremierStays", room.Provider);
        Assert.Equal(RoomType.Suite, room.RoomType);
        Assert.Equal(3, room.Nights);
        Assert.Equal(310m, room.PerNightPrice);
        Assert.Equal(930m, room.TotalStayPrice);
        Assert.Contains("Concierge", room.Amenities);
        Assert.Equal(5, room.StarRating);
    }

    [Fact]
    public async Task BudgetNestsMapper_FiltersUnavailableRoomsAndUsesFlexibleCancellation()
    {
        var provider = new BudgetNestsProvider();
        var mapper = new BudgetNestsRoomMapper();
        var criteria = new HotelSearchCriteria("Bangalore", new DateOnly(2026, 8, 10), new DateOnly(2026, 8, 12), null);

        var payload = await provider.SearchAsync(criteria, CancellationToken.None);
        var rooms = mapper.Map(payload, criteria);

        var room = Assert.Single(rooms);
        Assert.Equal("BN-BLR-STD", room.RoomId);
        Assert.Equal(RoomType.Standard, room.RoomType);
        Assert.Equal("Flexible cancellation", room.CancellationPolicy);
        Assert.Equal(6200m, room.TotalStayPrice);
    }

    [Fact]
    public async Task AvailabilityService_ComposesProvidersWithoutProviderSpecificBranching()
    {
        var service = new HotelStay.Api.Services.HotelAvailabilityService(
            new IHotelProvider[] { new PremierStaysProvider(), new BudgetNestsProvider() },
            new IProviderRoomMapper[] { new PremierStaysRoomMapper(), new BudgetNestsRoomMapper() });
        var criteria = new HotelSearchCriteria("London", new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 3), null);

        var rooms = await service.SearchAsync(criteria, CancellationToken.None);

        Assert.Contains(rooms, room => room.Provider == "PremierStays");
        Assert.Contains(rooms, room => room.Provider == "BudgetNests");
        Assert.Equal(rooms.OrderBy(room => room.TotalStayPrice).Select(room => room.RoomId), rooms.Select(room => room.RoomId));
    }
}