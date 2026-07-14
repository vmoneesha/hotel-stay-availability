using HotelStay.Domain.Enums;

namespace HotelStay.Domain.Services;

/// <summary>
/// Validates destination-specific identity document rules for reservations.
/// </summary>
public sealed class DocumentValidationService
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

    /// <summary>
    /// Returns true when the supplied document type satisfies the destination document rule.
    /// </summary>
    public bool IsValidForDestination(string destination, DocumentType documentType)
    {
        if (InternationalDestinations.Contains(destination))
        {
            return documentType == DocumentType.Passport;
        }

        if (DomesticDestinations.Contains(destination))
        {
            return documentType is DocumentType.NationalId or DocumentType.Passport;
        }

        throw new ArgumentException($"Unsupported destination '{destination}'.", nameof(destination));
    }

    /// <summary>
    /// Returns a user-friendly validation message for a document mismatch.
    /// </summary>
    public string GetValidationMessage(string destination) =>
        InternationalDestinations.Contains(destination)
            ? $"{destination} requires a valid Passport for reservation."
            : $"{destination} accepts either National ID or Passport for reservation.";
}