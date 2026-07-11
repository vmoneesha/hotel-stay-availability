# Hotel Stay Availability Specification

## Functional Requirements

- Search hotel rooms by destination, check-in date, check-out date, and optional room type.
- Normalize availability from multiple offline providers into one API response contract.
- Support room types: `Standard`, `Deluxe`, and `Suite`.
- Show provider badges in the frontend.
- Sort search results by total stay price.
- Display loading, empty, validation, result, reservation, and confirmation states.
- Reserve a selected room with guest and identity document details.
- Retrieve a confirmed reservation by reference.
- Run completely offline with deterministic in-memory data.

## API Contracts

### Search Hotels

`GET /hotels/search?destination=London&checkIn=2026-08-10&checkOut=2026-08-13&roomType=Suite`

Successful response:

```json
{
  "destination": "London",
  "checkIn": "2026-08-10",
  "checkOut": "2026-08-13",
  "rooms": [
    {
      "provider": "PremierStays",
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
      "cancellationPolicy": "Refundable until 96 hours before check-in"
    }
  ]
}
```

Validation response for HTTP 400:

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
  "provider": "PremierStays",
  "hotelId": "PS-LON-010",
  "roomId": "PS-LON-STE",
  "roomType": "Suite",
  "guestName": "Asha Rao",
  "documentType": "Passport",
  "documentNumber": "P1234567",
  "perNightPrice": 310
}
```

Successful response uses HTTP 201 and includes a location header pointing to `/hotels/reservation/{reference}`.

```json
{
  "reference": "HS-000001",
  "destination": "London",
  "checkIn": "2026-08-10",
  "checkOut": "2026-08-13",
  "provider": "PremierStays",
  "hotelId": "PS-LON-010",
  "roomId": "PS-LON-STE",
  "roomType": "Suite",
  "guestName": "Asha Rao",
  "documentType": "Passport",
  "perNightPrice": 310,
  "totalStayPrice": 930,
  "nights": 3,
  "createdAtUtc": "2026-07-11T08:00:00Z"
}
```

Document mismatch response uses HTTP 422.

### Get Reservation

`GET /hotels/reservation/{reference}`

Returns a confirmed reservation or HTTP 404.

## Data Models

### Enums

`RoomType`: `Standard`, `Deluxe`, `Suite`

`DocumentType`: `NationalId`, `Passport`

### HotelRoomDto

| Field | Type | Notes |
| --- | --- | --- |
| `provider` | string | Provider code, such as `PremierStays`. |
| `providerBadge` | string | UI badge text. |
| `hotelId` | string | Normalized hotel identifier. |
| `hotelName` | string | Display name. |
| `destination` | string | Normalized destination. |
| `roomId` | string | Provider room identifier. |
| `roomType` | RoomType | Standard, Deluxe, or Suite. |
| `perNightPrice` | decimal | Provider nightly rate. |
| `totalStayPrice` | decimal | `perNightPrice * nights`. |
| `nights` | integer | Difference between check-out and check-in. |
| `amenities` | string[] | Empty for providers that do not expose amenities. |
| `starRating` | integer? | Null for providers that do not expose rating. |
| `cancellationPolicy` | string | Normalized cancellation text. |

### ReservationDto

| Field | Type | Notes |
| --- | --- | --- |
| `reference` | string | Deterministic reference generated as `HS-000001`, `HS-000002`, etc. |
| `destination` | string | Selected destination. |
| `checkIn` | date | ISO date. |
| `checkOut` | date | ISO date. |
| `provider` | string | Selected provider. |
| `hotelId` | string | Selected hotel. |
| `roomId` | string | Selected room. |
| `roomType` | RoomType | Selected room type. |
| `guestName` | string | Guest name. |
| `documentType` | DocumentType | Submitted document type. |
| `perNightPrice` | decimal | Per-night price at reservation time. |
| `totalStayPrice` | decimal | Calculated total. |
| `nights` | integer | Stay length. |
| `createdAtUtc` | datetime | Reservation creation timestamp. |

## Provider Interfaces

`IHotelProvider` exposes provider-specific availability payloads without requiring a shared provider contract shape.

```text
ProviderCode: string
SearchAsync(criteria, cancellationToken) -> ProviderAvailabilityPayload
```

`IProviderRoomMapper` normalizes a provider payload into API-facing rooms.

```text
CanMap(providerCode) -> bool
Map(payload, criteria) -> IReadOnlyCollection<HotelRoomDto>
```

This keeps the orchestration service open for extension. New providers add a provider and mapper rather than changing the availability service.

## Provider Contracts

PremierStays:

- PascalCase C# contract.
- Rich response with hotels, rooms, amenities, star rating, cancellation policy, and nightly rates.

BudgetNests:

- snake_case JSON contract through `JsonPropertyName` attributes.
- Minimal response with room code, hotel name, destination, room type, nightly rate, and `available` flag.
- Unavailable rooms are filtered by `BudgetNestsRoomMapper`.
- Cancellation policy normalizes to `Flexible cancellation`.

## Validation Rules

HTTP 400:

- Destination missing.
- Check-in missing.
- Check-out missing.
- Check-out date is less than or equal to check-in date.
- Reservation provider, hotel id, room id, guest name, document number, or per-night price is invalid.

HTTP 422:

- Domestic destinations require `NationalId`.
- International destinations require `Passport`.

Frontend validation mirrors the same document and date rules before submitting requests.

## Assumptions

- Reservations are held in memory and are reset when the API process restarts.
- Inventory is deterministic and embedded in provider classes.
- Prices are numeric sample values and do not perform currency conversion.
- Authentication, payment, booking guarantees, databases, and external providers are outside scope.
- Angular communicates with the API at `http://localhost:5000` during local development.

## Extension Points

- Add providers through `IHotelProvider` and `IProviderRoomMapper`.
- Replace the in-memory reservation repository with persistent storage behind `IReservationRepository`.
- Add richer reservation state without changing provider normalization.
- Add integration tests around Minimal API endpoints.
- Add UI tests for search, reservation, and confirmation flows.
- Add configuration-driven destination and document rules.

## High-Level Architecture

```text
hotelstay-ui
  SearchComponent
  ReservationComponent
  ConfirmationComponent
  HotelApiService
  HotelSelectionStore

HotelStay.Api
  Program endpoints
  Validation
  Services
  Providers
  Mapping
  Repositories
  DTOs

HotelStay.Tests
  Availability mapping tests
  Validation tests
```
