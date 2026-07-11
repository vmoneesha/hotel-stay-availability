using HotelStay.Api.Dtos;

namespace HotelStay.Api.Extensions;

public static class DestinationExtensions
{
    private static readonly HashSet<string> DomesticDestinations = new(StringComparer.OrdinalIgnoreCase)
    {
        "Hyderabad",
        "Bangalore",
        "Mumbai"
    };

    private static readonly HashSet<string> InternationalDestinations = new(StringComparer.OrdinalIgnoreCase)
    {
        "London",
        "Dubai",
        "Singapore"
    };

    public static bool IsKnownDestination(this string destination) =>
        DomesticDestinations.Contains(destination) || InternationalDestinations.Contains(destination);

    public static bool IsInternational(this string destination) => InternationalDestinations.Contains(destination);

    public static DocumentType RequiredDocument(this string destination) =>
        destination.IsInternational() ? DocumentType.Passport : DocumentType.NationalId;
}