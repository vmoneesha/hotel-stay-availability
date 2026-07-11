using System.Collections.Concurrent;
using HotelStay.Api.Extensions;
using System.Text.Json.Serialization;
using HotelStay.Api.Domain.Dtos;
using HotelStay.Api.Domain.Enums;
using HotelStay.Api.Domain.ProviderContracts;
using HotelStay.Api.Domain.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using ApiValidationError = HotelStay.Api.Dtos.ValidationError;
using ApiValidationProblemResponse = HotelStay.Api.Dtos.ValidationProblemResponse;
using DomainBudgetNestsProvider = HotelStay.Api.Domain.Providers.BudgetNestsProvider;
using DomainPremierStaysProvider = HotelStay.Api.Domain.Providers.PremierStaysProvider;
using DomainReservationService = HotelStay.Api.Domain.Services.ReservationService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHotelStayServices();
builder.Services.AddSingleton<IHotelProvider, DomainPremierStaysProvider>();
builder.Services.AddSingleton<IHotelProvider, DomainBudgetNestsProvider>();
builder.Services.AddSingleton<HotelSearchService>();
builder.Services.AddSingleton<DocumentValidationService>();
builder.Services.AddSingleton<DomainReservationService>();
builder.Services.AddSingleton<ConcurrentDictionary<string, ReservationResponse>>();
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

app.MapGet("/", () => TypedResults.Ok(new
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

app.MapGet("/hotels/search", async Task<Results<Ok<IReadOnlyCollection<HotelRoomDto>>, BadRequest<ApiValidationProblemResponse>>> (
	string? destination,
	string? checkIn,
	string? checkOut,
	RoomType? roomType,
	HotelSearchService hotelSearchService,
	CancellationToken cancellationToken) =>
{
	var validationErrors = ValidateStayCriteria(destination, checkIn, checkOut, out var parsedCheckIn, out var parsedCheckOut);
	if (validationErrors.Count > 0)
	{
		return TypedResults.BadRequest(ApiValidationProblemResponse.From(validationErrors));
	}

	var request = new HotelSearchRequest(destination!.Trim(), parsedCheckIn, parsedCheckOut, roomType);
	var rooms = await hotelSearchService.SearchAsync(request, cancellationToken);
	return TypedResults.Ok(rooms);
});

app.MapPost("/hotels/reserve", Results<Created<ReservationResponse>, BadRequest<ApiValidationProblemResponse>, UnprocessableEntity<ApiValidationProblemResponse>> (
	ReservationRequest request,
	DocumentValidationService documentValidationService,
	DomainReservationService reservationService,
	ConcurrentDictionary<string, ReservationResponse> reservations) =>
{
	var validationErrors = ValidateReservationRequest(request);
	if (validationErrors.Count > 0)
	{
		return TypedResults.BadRequest(ApiValidationProblemResponse.From(validationErrors));
	}

	if (!documentValidationService.IsValidForDestination(request.Destination, request.DocumentType))
	{
		var requiredDocument = documentValidationService.GetRequiredDocumentType(request.Destination);
		var errors = new[]
		{
			new ApiValidationError("documentType", $"{request.Destination} requires {requiredDocument}.")
		};
		return TypedResults.UnprocessableEntity(ApiValidationProblemResponse.From(errors));
	}

	var reservation = reservationService.Reserve(request);
	reservations[reservation.Reference] = reservation;
	return TypedResults.Created($"/hotels/reservation/{reservation.Reference}", reservation);
});

app.MapGet("/hotels/reservation/{reference}", Results<Ok<ReservationResponse>, NotFound> (
	string reference,
	ConcurrentDictionary<string, ReservationResponse> reservations) =>
{
	return reservations.TryGetValue(reference, out var reservation)
		? TypedResults.Ok(reservation)
		: TypedResults.NotFound();
});

app.Run();

static List<ApiValidationError> ValidateStayCriteria(
	string? destination,
	string? checkIn,
	string? checkOut,
	out DateOnly parsedCheckIn,
	out DateOnly parsedCheckOut)
{
	var errors = new List<ApiValidationError>();
	parsedCheckIn = DateOnly.MinValue;
	parsedCheckOut = DateOnly.MinValue;

	if (string.IsNullOrWhiteSpace(destination))
	{
		errors.Add(new ApiValidationError("destination", "Destination is required."));
	}

	if (string.IsNullOrWhiteSpace(checkIn))
	{
		errors.Add(new ApiValidationError("checkIn", "Check-in date is required."));
	}
	else if (!DateOnly.TryParse(checkIn, out parsedCheckIn))
	{
		errors.Add(new ApiValidationError("checkIn", "Check-in date must be a valid ISO date."));
	}

	if (string.IsNullOrWhiteSpace(checkOut))
	{
		errors.Add(new ApiValidationError("checkOut", "Check-out date is required."));
	}
	else if (!DateOnly.TryParse(checkOut, out parsedCheckOut))
	{
		errors.Add(new ApiValidationError("checkOut", "Check-out date must be a valid ISO date."));
	}

	if (parsedCheckIn != DateOnly.MinValue && parsedCheckOut != DateOnly.MinValue && parsedCheckOut <= parsedCheckIn)
	{
		errors.Add(new ApiValidationError("checkOut", "Check-out date must be after check-in date."));
	}

	return errors;
}

static List<ApiValidationError> ValidateReservationRequest(ReservationRequest request)
{
	var errors = new List<ApiValidationError>();

	if (string.IsNullOrWhiteSpace(request.Destination))
	{
		errors.Add(new ApiValidationError("destination", "Destination is required."));
	}

	if (request.CheckIn == DateOnly.MinValue)
	{
		errors.Add(new ApiValidationError("checkIn", "Check-in date is required."));
	}

	if (request.CheckOut == DateOnly.MinValue)
	{
		errors.Add(new ApiValidationError("checkOut", "Check-out date is required."));
	}

	if (request.CheckIn != DateOnly.MinValue && request.CheckOut != DateOnly.MinValue && request.CheckOut <= request.CheckIn)
	{
		errors.Add(new ApiValidationError("checkOut", "Check-out date must be after check-in date."));
	}

	return errors;
}

public partial class Program;
