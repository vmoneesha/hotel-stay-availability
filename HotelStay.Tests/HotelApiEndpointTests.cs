using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HotelStay.Tests;

public sealed class HotelApiEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public HotelApiEndpointTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Search_ReturnsNormalizedAvailableRoomsOrderedByTotalPrice()
    {
        var response = await client.GetAsync("/hotels/search?destination=Hyderabad&checkIn=2026-08-10&checkOut=2026-08-12");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var rooms = document.RootElement.EnumerateArray().ToArray();

        Assert.Equal(new[] { "BN-HYD-STD", "PS-HYD-STD", "PS-HYD-DLX" }, rooms.Select(room => room.GetProperty("roomId").GetString()));
        Assert.All(rooms, room => Assert.True(room.GetProperty("totalStayPrice").GetDecimal() > 0));
        Assert.Contains(rooms, room => room.GetProperty("providerCode").GetString() == "PremierStays");
        Assert.Contains(rooms, room => room.GetProperty("providerCode").GetString() == "BudgetNests");
    }

    [Fact]
    public async Task Search_ReturnsBadRequestForMissingRequiredCriteria()
    {
        var response = await client.GetAsync("/hotels/search");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var fields = document.RootElement.GetProperty("details")
            .EnumerateArray()
            .Select(error => error.GetProperty("field").GetString())
            .ToArray();

        Assert.Contains("destination", fields);
        Assert.Contains("checkIn", fields);
        Assert.Contains("checkOut", fields);
    }

    [Fact]
    public async Task Search_ReturnsBadRequestWhenCheckOutIsNotAfterCheckIn()
    {
        var response = await client.GetAsync("/hotels/search?destination=London&checkIn=2026-08-10&checkOut=2026-08-10");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        Assert.Contains(document.RootElement.GetProperty("details").EnumerateArray(), error =>
            error.GetProperty("field").GetString() == "checkOut");
    }

    [Fact]
    public async Task Reserve_CreatesReservationAndLookupReturnsConfirmation()
    {
        var response = await client.PostAsJsonAsync("/hotels/reserve", ReservationRequest("London", "Passport"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        using var createdDocument = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var reference = createdDocument.RootElement.GetProperty("reference").GetString();

        Assert.StartsWith("HS-", reference);
        Assert.Equal("London", createdDocument.RootElement.GetProperty("destination").GetString());
        Assert.Equal(930m, createdDocument.RootElement.GetProperty("totalStayPrice").GetDecimal());

        var lookupResponse = await client.GetAsync($"/hotels/reservation/{reference}");

        Assert.Equal(HttpStatusCode.OK, lookupResponse.StatusCode);
        using var lookupDocument = await JsonDocument.ParseAsync(await lookupResponse.Content.ReadAsStreamAsync());
        Assert.Equal(reference, lookupDocument.RootElement.GetProperty("reference").GetString());
    }

    [Fact]
    public async Task Reserve_ReturnsUnprocessableEntityForInternationalNationalId()
    {
        var response = await client.PostAsJsonAsync("/hotels/reserve", ReservationRequest("London", "NationalId"));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var error = Assert.Single(document.RootElement.GetProperty("details").EnumerateArray());
        Assert.Equal("documentType", error.GetProperty("field").GetString());
        Assert.Contains("Passport", error.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Reserve_ReturnsBadRequestForMissingGuestAndRoomDetails()
    {
        var response = await client.PostAsJsonAsync("/hotels/reserve", new
        {
            destination = "Hyderabad",
            checkIn = "2026-08-10",
            checkOut = "2026-08-12",
            providerCode = "",
            hotelId = "",
            roomId = "",
            roomType = "Standard",
            guestName = "",
            documentType = "NationalId",
            documentNumber = "",
            perNightPrice = 0m
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var fields = document.RootElement.GetProperty("details")
            .EnumerateArray()
            .Select(error => error.GetProperty("field").GetString())
            .ToArray();

        Assert.Contains("providerCode", fields);
        Assert.Contains("hotelId", fields);
        Assert.Contains("roomId", fields);
        Assert.Contains("guestName", fields);
        Assert.Contains("documentNumber", fields);
        Assert.Contains("perNightPrice", fields);
    }

    [Fact]
    public async Task ReservationLookup_ReturnsNotFoundForUnknownReference()
    {
        var response = await client.GetAsync("/hotels/reservation/HS-999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static object ReservationRequest(string destination, string documentType) => new
    {
        destination,
        checkIn = "2026-08-10",
        checkOut = "2026-08-13",
        providerCode = "PremierStays",
        hotelId = "PS-LON-010",
        roomId = "PS-LON-STE",
        roomType = "Suite",
        guestName = "Asha Rao",
        documentType,
        documentNumber = documentType == "Passport" ? "P1234567" : "ID12345",
        perNightPrice = 310m
    };
}