using HotelStay.Api.Dtos;
using HotelStay.Api.Extensions;
using HotelStay.Api.Services;
using HotelStay.Api.Validation;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHotelStayServices();
builder.Services.ConfigureHttpJsonOptions(options =>
{
	options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddCors(options =>
{
	options.AddPolicy("HotelStayUi", policy =>
	{
		policy.WithOrigins("http://localhost:4200")
			.AllowAnyHeader()
			.AllowAnyMethod();
	});
});

var app = builder.Build();

app.UseCors("HotelStayUi");

app.MapGet("/", () => Results.Ok(new
{
	name = "Hotel Stay Availability API",
	status = "Running",
	endpoints = new[]
	{
		"GET /hotels/search",
		"POST /hotels/reserve",
		"GET /hotels/reservation/{reference}"
	}
}));

app.MapGet("/hotels/search", async (
	string? destination,
	string? checkIn,
	string? checkOut,
	RoomType? roomType,
	IHotelSearchValidator validator,
	IHotelAvailabilityService availabilityService,
	CancellationToken cancellationToken) =>
{
	var request = new HotelSearchRequest(destination, checkIn, checkOut, roomType);
	var validation = validator.Validate(request);

	if (!validation.IsValid)
	{
		return Results.BadRequest(ValidationProblemResponse.From(validation.Errors));
	}

	var rooms = await availabilityService.SearchAsync(validation.Criteria, cancellationToken);
	return Results.Ok(new HotelSearchResponse(validation.Criteria.Destination, validation.Criteria.CheckIn, validation.Criteria.CheckOut, rooms));
});

app.MapPost("/hotels/reserve", async (
	ReserveRoomRequest request,
	IReservationValidator validator,
	IReservationService reservationService,
	CancellationToken cancellationToken) =>
{
	var validation = validator.Validate(request);

	if (validation.HasBadRequestErrors)
	{
		return Results.BadRequest(ValidationProblemResponse.From(validation.BadRequestErrors));
	}

	if (validation.HasUnprocessableEntityErrors)
	{
		return Results.UnprocessableEntity(ValidationProblemResponse.From(validation.UnprocessableEntityErrors));
	}

	var reservation = await reservationService.ReserveAsync(validation.Reservation, cancellationToken);
	return Results.Created($"/hotels/reservation/{reservation.Reference}", reservation);
});

app.MapGet("/hotels/reservation/{reference}", async (
	string reference,
	IReservationService reservationService,
	CancellationToken cancellationToken) =>
{
	var reservation = await reservationService.GetAsync(reference, cancellationToken);
	return reservation is null ? Results.NotFound() : Results.Ok(reservation);
});

app.Run();

public partial class Program;
