# Hotel Stay Availability Specification

## Scope

Build an offline hotel availability and reservation workflow for the SkyRoute case study. The application must normalize availability from multiple deterministic hotel provider stubs, support destination-specific document validation, and provide a usable frontend flow from search through confirmation.

Out of scope: real provider APIs, credentials, authentication, payment, durable persistence, cloud deployment, and nondeterministic inventory.

## Functional Requirements

- Search by destination, check-in, check-out, and optional room type.
- Support normalized room types: `Standard`, `Deluxe`, `Suite`.
- Query PremierStays and BudgetNests provider stubs for every search.
- Filter provider rooms where availability is false.
- Normalize provider-specific contracts into one public room DTO.
- Calculate `nights` from checkout minus check-in.
- Calculate `totalStayPrice` from `perNightPrice * nights`.
- Sort returned rooms by total stay price, provider code, then hotel name.
- Reserve a selected room with guest and document details.
- Validate document rules on both frontend and backend.
- Store confirmed reservations in memory for the API process lifetime.
- Retrieve a reservation by reference.

## API Contracts

### Search Hotels

`GET /hotels/search?destination=London&checkIn=2026-08-10&checkOut=2026-08-13&roomType=Suite`

Query parameters:

| Name | Required | Notes |
| --- | --- | --- |
| `destination` | Yes | Supported deterministic city. |
| `checkIn` | Yes | ISO date. |
| `checkOut` | Yes | ISO date after check-in. |
| `roomType` | No | `Standard`, `Deluxe`, or `Suite`. |

Successful response: HTTP 200 with a JSON array.

```json
[
  {
    "providerCode": "PremierStays",
    "providerBadge": "Premier",
    "hotelId": "PS-LON-010",
    "hotelName": "Premier London Regent",
    "destination": "London",
    "roomId": "PS-LON-STE",
    "roomType": "Suite",
    "perNightPrice": 310,
    "totalStayPrice": 930,
    "nights": 3,
    "amenities": ["Lounge", "Concierge", "Breakfast"],
    "starRating": 5,
    "cancellationPolicy": "Refundable",
    "cancellationPolicyDescription": "Refundable until 96 hours before check-in"
  }
]
```

HTTP 400 validation response:

```json
{
  "error": "ValidationFailed",
  "details": [
    {
      "field": "checkOut",
      "message": "Check-out date must be after check-in date."
    }
  ]
}
```

### Reserve Room

`POST /hotels/reserve`

Request body:

```json
{
  "destination": "London",
  "checkIn": "2026-08-10",
  "checkOut": "2026-08-13",
  "providerCode": "PremierStays",
  "hotelId": "PS-LON-010",
  "roomId": "PS-LON-STE",
  "roomType": "Suite",
  "guestName": "Asha Rao",
  "documentType": "Passport",
  "documentNumber": "P1234567",
  "perNightPrice": 310
}
```

Successful response: HTTP 201 with `Location: /hotels/reservation/{reference}`.

```json
{
  "reference": "HS-000001",
  "destination": "London",
  "checkIn": "2026-08-10",
  "checkOut": "2026-08-13",
  "providerCode": "PremierStays",
  "hotelId": "PS-LON-010",
  "roomId": "PS-LON-STE",
  "roomType": "Suite",
  "guestName": "Asha Rao",
  "documentType": "Passport",
  "perNightPrice": 310,
  "totalStayPrice": 930,
  "nights": 3,
  "createdAtUtc": "2026-07-11T00:00:00+00:00"
}
```

HTTP 400 is returned for:

- Missing destination.
- Missing provider code, hotel id, room id, guest name, or document number.
- Missing check-in or check-out.
- Check-out date on or before check-in.
- Non-positive per-night price.

HTTP 422 is returned for a valid request whose document type violates the destination rule.

### Get Reservation

`GET /hotels/reservation/{reference}`

Returns HTTP 200 with the reservation response when found. Returns HTTP 404 when the reference is unknown.

## Data Models

### Enums

| Enum | Values |
| --- | --- |
| `RoomType` | `Standard`, `Deluxe`, `Suite` |
| `DocumentType` | `NationalId`, `Passport` |
| `CancellationPolicy` | `Flexible`, `FreeCancellation`, `Refundable`, `NonRefundable` |

### `HotelRoomDto`

| Field | Type | Notes |
| --- | --- | --- |
| `providerCode` | string | Stable provider code, for example `PremierStays`. |
| `providerBadge` | string | Short UI badge text. |
| `hotelId` | string | Normalized hotel identifier. |
| `hotelName` | string | Display name. |
| `destination` | string | City. |
| `roomId` | string | Provider room identifier. |
| `roomType` | enum | `Standard`, `Deluxe`, or `Suite`. |
| `perNightPrice` | decimal | Nightly rate. |
| `totalStayPrice` | decimal | Rounded total for the stay. |
| `nights` | integer | Checkout day number minus check-in day number. |
| `amenities` | string[] | Empty when provider does not expose amenities. |
| `starRating` | integer? | Null when provider does not expose rating. |
| `cancellationPolicy` | enum | Normalized cancellation policy. |
| `cancellationPolicyDescription` | string | User-facing cancellation text. |

### `ReservationRequest`

| Field | Type | Notes |
| --- | --- | --- |
| `destination` | string | Selected room destination. |
| `checkIn` | date | ISO date. |
| `checkOut` | date | ISO date after check-in. |
| `providerCode` | string | Selected provider. |
| `hotelId` | string | Selected hotel. |
| `roomId` | string | Selected room. |
| `roomType` | enum | Selected room type. |
| `guestName` | string | Required guest name. |
| `documentType` | enum | Submitted document type. |
| `documentNumber` | string | Required document number. |
| `perNightPrice` | decimal | Must be greater than zero. |

## Provider Contracts

### PremierStays

- Rich source model using PascalCase C# records.
- Always returns available rooms for supported destinations.
- Includes hotel id, hotel name, destination, amenities, star rating, room id, room type, nightly rate, and cancellation policy text.

### BudgetNests

- Minimal source model with snake_case JSON property names.
- Includes room code, hotel name, destination, room type, nightly rate, cancellation text, and `available` flag.
- Rooms with `available: false` are excluded from normalized search results.

## Document Rules

| Destination Type | Cities | Accepted Documents |
| --- | --- | --- |
| Domestic | Hyderabad, Bangalore, Mumbai | `NationalId` or `Passport` |
| International | London, Dubai, Singapore | `Passport` only |

The frontend defaults international reservations to Passport and blocks invalid document submissions. The backend remains authoritative and returns HTTP 422 for document mismatches.

## Frontend Requirements

- Search page with destination, dates, optional room type, loading state, validation state, empty state, and results list.
- Results include provider badge, hotel name, room type, per-night price, total stay price, nights, amenities where available, and cancellation policy.
- Results can be sorted by total price or room type.
- Reservation page collects guest name, document type, and document number.
- Confirmation page displays reference, guest, provider, destination, dates, nights, total price, cancellation policy, and creation timestamp.

## Testing Requirements

- Backend xUnit unit tests for domain services and provider normalization.
- Backend Minimal API integration tests for route binding, JSON contracts, validation status codes, reservation creation, and lookup.
- Angular unit tests for services, state store, pages, and shared components.
- Playwright e2e tests for search to reservation to confirmation, invalid dates, and international document mismatch.

## Extension Points

- Add a provider-specific source model under `Domain/ProviderModels`.
- Implement `IHotelProvider` for the provider.
- Implement `IProviderRoomNormalizer` for that provider's payload.
- Register the provider and normalizer with dependency injection.
- Add domain and endpoint tests for the new provider's behavior.

## Assumptions

- Reservation storage is intentionally in memory and resets when the API process restarts.
- Prices are sample values and do not perform currency conversion.
- The local Angular app calls `http://localhost:5000`.
- Swagger is enabled only in development.