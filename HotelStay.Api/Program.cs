using System.Collections.Concurrent;
using HotelStay.Api.Extensions;
using System.Text.Json.Serialization;
using HotelStay.Api.Domain.Dtos;
using HotelStay.Api.Domain.Enums;
using HotelStay.Api.Domain.ProviderContracts;
using HotelStay.Api.Domain.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using ApiValidationError = HotelStay.Api.Dtos.ValidationError;
using ApiValidationProblemResponse = HotelStay.Api.Dtos.ValidationProblemResponse;
using DomainBudgetNestsProvider = HotelStay.Api.Domain.Providers.BudgetNestsProvider;
using DomainPremierStaysProvider = HotelStay.Api.Domain.Providers.PremierStaysProvider;
using DomainReservationService = HotelStay.Api.Domain.Services.ReservationService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHotelStayServices();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "Hotel Stay Availability API",
		Version = "v1",
		Description = "Offline deterministic hotel availability and reservation API for the Hotel Stay Availability case study."
	});
	options.OperationFilter<HotelStayExamplesOperationFilter>();
});
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

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		options.DocumentTitle = "Hotel Stay Availability API Docs";
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Stay Availability API v1");
		options.RoutePrefix = "swagger";
	});
}

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
}))
	.WithTags("Hotels")
	.WithSummary("Get API status")
	.WithDescription("Returns a lightweight status document and lists the hotel availability and reservation endpoints exposed by the API.")
	.Produces(StatusCodes.Status200OK)
	.WithOpenApi();

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
})
	.WithTags("Hotels")
	.WithSummary("Search available hotel rooms")
	.WithDescription("Searches deterministic PremierStays and BudgetNests inventory, normalizes provider-specific room data, filters unavailable rooms, and returns total stay prices for the requested dates.")
	.Produces<IReadOnlyCollection<HotelRoomDto>>(StatusCodes.Status200OK)
	.Produces<ApiValidationProblemResponse>(StatusCodes.Status400BadRequest)
	.WithOpenApi(operation =>
	{
		operation.Parameters[0].Description = "Destination city. Supported examples include Hyderabad, Bangalore, Mumbai, London, Dubai, and Singapore.";
		operation.Parameters[1].Description = "Check-in date in ISO format, for example 2026-08-10.";
		operation.Parameters[2].Description = "Check-out date in ISO format. Must be after check-in.";
		operation.Parameters[3].Description = "Optional normalized room type filter: Standard, Deluxe, or Suite.";
		return operation;
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
		var errors = new[]
		{
			new ApiValidationError("documentType", documentValidationService.GetValidationMessage(request.Destination))
		};
		return TypedResults.UnprocessableEntity(ApiValidationProblemResponse.From(errors));
	}

	var reservation = reservationService.Reserve(request);
	reservations[reservation.Reference] = reservation;
	return TypedResults.Created($"/hotels/reservation/{reservation.Reference}", reservation);
})
	.WithTags("Hotels")
	.WithSummary("Reserve a selected hotel room")
	.WithDescription("Creates a deterministic in-memory reservation for a selected room after validating stay dates and destination-specific document rules.")
	.Accepts<ReservationRequest>("application/json")
	.Produces<ReservationResponse>(StatusCodes.Status201Created)
	.Produces<ApiValidationProblemResponse>(StatusCodes.Status400BadRequest)
	.Produces<ApiValidationProblemResponse>(StatusCodes.Status422UnprocessableEntity)
	.WithOpenApi();

app.MapGet("/hotels/reservation/{reference}", Results<Ok<ReservationResponse>, NotFound> (
	string reference,
	ConcurrentDictionary<string, ReservationResponse> reservations) =>
{
	return reservations.TryGetValue(reference, out var reservation)
		? TypedResults.Ok(reservation)
		: TypedResults.NotFound();
})
	.WithTags("Hotels")
	.WithSummary("Get reservation by reference")
	.WithDescription("Returns an in-memory reservation confirmation by reference while the API process is running. Missing references return 404 Not Found.")
	.Produces<ReservationResponse>(StatusCodes.Status200OK)
	.Produces(StatusCodes.Status404NotFound)
	.WithOpenApi(operation =>
	{
		operation.Parameters[0].Description = "Reservation reference returned by POST /hotels/reserve, for example HS-000001.";
		return operation;
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

internal sealed class HotelStayExamplesOperationFilter : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		var path = context.ApiDescription.RelativePath?.Split('?')[0];
		var method = context.ApiDescription.HttpMethod;

		if (method == "GET" && path == string.Empty)
		{
			AddJsonExample(operation, StatusCodes.Status200OK, new OpenApiObject
			{
				["name"] = new OpenApiString("Hotel Stay Availability API"),
				["status"] = new OpenApiString("Running"),
				["endpoints"] = new OpenApiArray
				{
					new OpenApiString("GET /hotels/search"),
					new OpenApiString("POST /hotels/reserve"),
					new OpenApiString("GET /hotels/reservation/{reference}")
				}
			});
		}

		if (method == "GET" && path == "hotels/search")
		{
			AddJsonExample(operation, StatusCodes.Status200OK, new OpenApiArray
			{
				new OpenApiObject
				{
					["providerCode"] = new OpenApiString("BudgetNests"),
					["providerBadge"] = new OpenApiString("Budget"),
					["hotelId"] = new OpenApiString("BN-HYDERABAD"),
					["hotelName"] = new OpenApiString("Budget Hyderabad Central"),
					["destination"] = new OpenApiString("Hyderabad"),
					["roomId"] = new OpenApiString("BN-HYD-STD"),
					["roomType"] = new OpenApiString("Standard"),
					["perNightPrice"] = new OpenApiDouble(2900),
					["totalStayPrice"] = new OpenApiDouble(5800),
					["nights"] = new OpenApiInteger(2),
					["amenities"] = new OpenApiArray(),
					["starRating"] = new OpenApiNull(),
					["cancellationPolicy"] = new OpenApiString("Flexible"),
					["cancellationPolicyDescription"] = new OpenApiString("Flexible cancellation")
				}
			});
			AddJsonExample(operation, StatusCodes.Status400BadRequest, ValidationExample("checkOut", "Check-out date must be after check-in date."));
		}

		if (method == "POST" && path == "hotels/reserve")
		{
			operation.RequestBody.Content["application/json"].Example = new OpenApiObject
			{
				["destination"] = new OpenApiString("Hyderabad"),
				["checkIn"] = new OpenApiString("2026-08-10"),
				["checkOut"] = new OpenApiString("2026-08-12"),
				["providerCode"] = new OpenApiString("BudgetNests"),
				["hotelId"] = new OpenApiString("BN-HYDERABAD"),
				["roomId"] = new OpenApiString("BN-HYD-STD"),
				["roomType"] = new OpenApiString("Standard"),
				["guestName"] = new OpenApiString("Asha Rao"),
				["documentType"] = new OpenApiString("Passport"),
				["documentNumber"] = new OpenApiString("P1234567"),
				["perNightPrice"] = new OpenApiDouble(2900)
			};
			AddJsonExample(operation, StatusCodes.Status201Created, new OpenApiObject
			{
				["reference"] = new OpenApiString("HS-000001"),
				["destination"] = new OpenApiString("Hyderabad"),
				["checkIn"] = new OpenApiString("2026-08-10"),
				["checkOut"] = new OpenApiString("2026-08-12"),
				["providerCode"] = new OpenApiString("BudgetNests"),
				["hotelId"] = new OpenApiString("BN-HYDERABAD"),
				["roomId"] = new OpenApiString("BN-HYD-STD"),
				["roomType"] = new OpenApiString("Standard"),
				["guestName"] = new OpenApiString("Asha Rao"),
				["documentType"] = new OpenApiString("Passport"),
				["perNightPrice"] = new OpenApiDouble(2900),
				["totalStayPrice"] = new OpenApiDouble(5800),
				["nights"] = new OpenApiInteger(2),
				["createdAtUtc"] = new OpenApiString("2026-07-11T00:00:00+00:00")
			});
			AddJsonExample(operation, StatusCodes.Status400BadRequest, ValidationExample("guestName", "Guest name is required."));
			AddJsonExample(operation, StatusCodes.Status422UnprocessableEntity, ValidationExample("documentType", "London requires a valid Passport for reservation."));
		}

		if (method == "GET" && path == "hotels/reservation/{reference}")
		{
			AddJsonExample(operation, StatusCodes.Status200OK, new OpenApiObject
			{
				["reference"] = new OpenApiString("HS-000001"),
				["destination"] = new OpenApiString("Hyderabad"),
				["checkIn"] = new OpenApiString("2026-08-10"),
				["checkOut"] = new OpenApiString("2026-08-12"),
				["providerCode"] = new OpenApiString("BudgetNests"),
				["hotelId"] = new OpenApiString("BN-HYDERABAD"),
				["roomId"] = new OpenApiString("BN-HYD-STD"),
				["roomType"] = new OpenApiString("Standard"),
				["guestName"] = new OpenApiString("Asha Rao"),
				["documentType"] = new OpenApiString("Passport"),
				["perNightPrice"] = new OpenApiDouble(2900),
				["totalStayPrice"] = new OpenApiDouble(5800),
				["nights"] = new OpenApiInteger(2),
				["createdAtUtc"] = new OpenApiString("2026-07-11T00:00:00+00:00")
			});
		}
	}

	private static void AddJsonExample(OpenApiOperation operation, int statusCode, IOpenApiAny example)
	{
		var responseKey = statusCode.ToString();
		if (operation.Responses.TryGetValue(responseKey, out var response) && response.Content.TryGetValue("application/json", out var mediaType))
		{
			mediaType.Example = example;
		}
	}

	private static OpenApiObject ValidationExample(string field, string message) => new()
	{
		["error"] = new OpenApiString("ValidationFailed"),
		["details"] = new OpenApiArray
		{
			new OpenApiObject
			{
				["field"] = new OpenApiString(field),
				["message"] = new OpenApiString(message)
			}
		}
	};
}
