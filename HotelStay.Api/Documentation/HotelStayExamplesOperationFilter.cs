using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HotelStay.Api.Documentation;

internal sealed class HotelStayExamplesOperationFilter : IOperationFilter
{
	private const string SearchPath = "hotels/search";
	private const string ReservePath = "hotels/reserve";
	private const string ReservationLookupPath = "hotels/reservation/{reference}";

	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		var path = context.ApiDescription.RelativePath?.Split('?')[0];
		var method = context.ApiDescription.HttpMethod;

		if (method == "GET" && path == string.Empty)
		{
			AddJsonExample(operation, StatusCodes.Status200OK, ApiStatusExample());
		}

		if (method == "GET" && path == SearchPath)
		{
			AddJsonExample(operation, StatusCodes.Status200OK, SearchResponseExample());
			AddJsonExample(operation, StatusCodes.Status400BadRequest, ValidationExample("checkOut", "Check-out date must be after check-in date."));
		}

		if (method == "POST" && path == ReservePath)
		{
			operation.RequestBody.Content["application/json"].Example = ReservationRequestExample();
			AddJsonExample(operation, StatusCodes.Status201Created, ReservationResponseExample());
			AddJsonExample(operation, StatusCodes.Status400BadRequest, ValidationExample("guestName", "Guest name is required."));
			AddJsonExample(operation, StatusCodes.Status422UnprocessableEntity, ValidationExample("documentType", "London requires a valid Passport for reservation."));
		}

		if (method == "GET" && path == ReservationLookupPath)
		{
			AddJsonExample(operation, StatusCodes.Status200OK, ReservationResponseExample());
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

	private static OpenApiObject ApiStatusExample() => new()
	{
		["name"] = new OpenApiString("Hotel Stay Availability API"),
		["status"] = new OpenApiString("Running"),
		["endpoints"] = new OpenApiArray
		{
			new OpenApiString("GET /hotels/search"),
			new OpenApiString("POST /hotels/reserve"),
			new OpenApiString("GET /hotels/reservation/{reference}")
		}
	};

	private static OpenApiArray SearchResponseExample() => new()
	{
		RoomExample()
	};

	private static OpenApiObject RoomExample() => new()
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
	};

	private static OpenApiObject ReservationRequestExample() => new()
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

	private static OpenApiObject ReservationResponseExample() => new()
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
	};

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