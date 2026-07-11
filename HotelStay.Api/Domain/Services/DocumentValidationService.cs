using HotelStay.Api.Domain.Enums;

namespace HotelStay.Api.Domain.Services;

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
    /// Gets the required document type for a supported destination.
    /// </summary>
    public DocumentType GetRequiredDocumentType(string destination)
    {
        if (InternationalDestinations.Contains(destination))
        {
            return DocumentType.Passport;
        }

        if (DomesticDestinations.Contains(destination))
        {
            return DocumentType.NationalId;
        }

        throw new ArgumentException($"Unsupported destination '{destination}'.", nameof(destination));
    }

    /// <summary>
    /// Returns true when the supplied document type satisfies the destination document rule.
    /// </summary>
    public bool IsValidForDestination(string destination, DocumentType documentType) =>
        GetRequiredDocumentType(destination) == documentType;
}