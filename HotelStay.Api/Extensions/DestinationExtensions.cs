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

    public static bool AcceptsDocument(this string destination, DocumentType documentType) =>
        destination.IsInternational()
            ? documentType == DocumentType.Passport
            : documentType is DocumentType.NationalId or DocumentType.Passport;

    public static string DocumentValidationMessage(this string destination) =>
        destination.IsInternational()
            ? $"{destination} requires a valid Passport for reservation."
            : $"{destination} accepts either National ID or Passport for reservation.";
}