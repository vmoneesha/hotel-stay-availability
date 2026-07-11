# Hotel Stay Availability Specification

## Functional Requirements

### Availability Search

- Users can search for hotel stay availability by destination, check-in date, check-out date, guest count, and optional room preferences.
- The system validates search criteria before querying availability providers.
- The system returns normalized stay options that include hotel details, room details, rate information, cancellation information, and availability status.
- The system supports zero-result responses with clear status and messaging.
- The system can be extended to query multiple inventory providers.

### Hotel and Room Results

- Search results include hotel identity, location summary, rating where available, and available room options.
- Room options include room type, capacity, nightly rate, total price, currency, inventory count where available, and provider reference.
- Results can be sorted by price, rating, or recommended order in a future implementation.
- Results can be filtered by room type, price range, occupancy, and amenities in a future implementation.

### Provider Integration

- The first implementation may use an in-memory or static sample provider to validate the application flow.
- Provider implementations must be replaceable without changing API contracts.
- Provider responses must be normalized into internal availability result models.
- Provider failures must be handled without exposing provider-specific internals to API clients.

### User Interface

- The Angular client will provide a search form for stay criteria.
- The client will display loading, validation, empty, error, and successful result states.
- The client will render returned availability options in a scannable list or card layout.
- Booking completion is outside the initial scope unless explicitly added later.

## API Contracts

### Search Availability

`POST /api/availability/search`

#### Request Body

```json
{
  "destination": "Seattle, WA",
  "checkInDate": "2026-08-15",
  "checkOutDate": "2026-08-18",
  "adults": 2,
  "children": 0,
  "rooms": 1,
  "roomType": "King",
  "maxPricePerNight": 250,
  "currency": "USD"
}
```

#### Successful Response

```json
{
  "searchId": "av_01JZEXAMPLE",
  "criteria": {
    "destination": "Seattle, WA",
    "checkInDate": "2026-08-15",
    "checkOutDate": "2026-08-18",
    "adults": 2,
    "children": 0,
    "rooms": 1,
    "roomType": "King",
    "maxPricePerNight": 250,
    "currency": "USD"
  },
  "results": [
    {
      "hotelId": "hotel-1001",
      "hotelName": "Example Harbor Hotel",
      "location": "Seattle, WA",
      "rating": 4.5,
      "rooms": [
        {
          "roomId": "room-king-1",
          "roomType": "King",
          "capacity": 2,
          "availableRooms": 3,
          "nightlyRate": 219.00,
          "totalPrice": 657.00,
          "currency": "USD",
          "cancellationPolicy": "Free cancellation until 48 hours before check-in",
          "providerCode": "sample-provider"
        }
      ]
    }
  ],
  "warnings": []
}
```

#### Empty Response

```json
{
  "searchId": "av_01JZEMPTY",
  "criteria": {
    "destination": "Seattle, WA",
    "checkInDate": "2026-08-15",
    "checkOutDate": "2026-08-18",
    "adults": 2,
    "children": 0,
    "rooms": 1,
    "currency": "USD"
  },
  "results": [],
  "warnings": ["No availability matched the search criteria."]
}
```

#### Validation Error Response

```json
{
  "error": "ValidationFailed",
  "message": "One or more validation errors occurred.",
  "details": [
    {
      "field": "checkOutDate",
      "message": "Check-out date must be after check-in date."
    }
  ]
}
```

### Optional Health Check

`GET /api/health`

```json
{
  "status": "Healthy",
  "timestampUtc": "2026-07-11T00:00:00Z"
}
```

## Data Models

### AvailabilitySearchRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| `destination` | string | Yes | City, region, hotel name, or provider-supported location token. |
| `checkInDate` | date | Yes | ISO 8601 date without time. |
| `checkOutDate` | date | Yes | ISO 8601 date without time. |
| `adults` | integer | Yes | Number of adult guests. |
| `children` | integer | No | Defaults to 0. |
| `rooms` | integer | Yes | Number of requested rooms. |
| `roomType` | string | No | Optional room preference. |
| `maxPricePerNight` | decimal | No | Optional upper bound per night. |
| `currency` | string | No | Defaults to `USD` for initial implementation. |

### AvailabilitySearchResponse

| Field | Type | Notes |
| --- | --- | --- |
| `searchId` | string | Unique identifier for the search response. |
| `criteria` | AvailabilitySearchCriteria | Echoes normalized criteria used by the backend. |
| `results` | HotelAvailabilityResult[] | Matching hotels and room options. |
| `warnings` | string[] | Non-fatal messages such as partial provider failures or no results. |

### HotelAvailabilityResult

| Field | Type | Notes |
| --- | --- | --- |
| `hotelId` | string | Stable hotel identifier within the application boundary. |
| `hotelName` | string | Display name. |
| `location` | string | Human-readable location summary. |
| `rating` | decimal? | Optional rating if available. |
| `rooms` | RoomAvailabilityResult[] | Available room options. |

### RoomAvailabilityResult

| Field | Type | Notes |
| --- | --- | --- |
| `roomId` | string | Stable room or room-rate identifier. |
| `roomType` | string | Room category or display type. |
| `capacity` | integer | Maximum guest capacity. |
| `availableRooms` | integer? | Remaining inventory if provider exposes it. |
| `nightlyRate` | decimal | Per-night price. |
| `totalPrice` | decimal | Total stay price before booking. |
| `currency` | string | ISO 4217 currency code. |
| `cancellationPolicy` | string | Human-readable cancellation terms. |
| `providerCode` | string | Identifier for the source provider. |

## Provider Interfaces

Provider integrations should be defined behind an availability provider contract. The exact implementation language will be added during scaffolding, but the design intent is:

```text
AvailabilityProvider
  SearchAsync(criteria, cancellationToken) -> ProviderAvailabilityResult
```

Expected provider responsibilities:

- Accept normalized search criteria from the application layer.
- Query the backing inventory source.
- Return provider-specific availability data or mapped internal result data.
- Surface provider errors in a structured way.
- Avoid leaking transport, credential, or vendor-specific details into API controllers.

Expected application service responsibilities:

- Validate and normalize request criteria.
- Invoke one or more providers.
- Merge, filter, and sort provider results.
- Convert provider outcomes into API response contracts.
- Apply consistent error and warning handling.

## Validation Rules

- `destination` is required and must not be blank.
- `checkInDate` is required and must be today or a future date based on server date.
- `checkOutDate` is required and must be after `checkInDate`.
- Stay length must be at least 1 night.
- Maximum stay length should default to 30 nights unless changed by business requirements.
- `adults` must be at least 1.
- `children` must be 0 or greater.
- `rooms` must be at least 1.
- Total guests should not exceed a configurable maximum for the initial search request.
- `maxPricePerNight`, when supplied, must be greater than 0.
- `currency`, when supplied, must be a valid ISO 4217 currency code.
- Optional text fields should be trimmed before processing.

## Assumptions

- The initial system searches availability but does not complete reservations or process payments.
- The first provider can be a sample or in-memory provider to prove the end-to-end flow.
- All public API dates use ISO 8601 date strings.
- Prices are returned in a single currency per search response for the initial implementation.
- Authentication and user accounts are out of scope for the first implementation unless added later.
- Availability results are point-in-time responses and do not guarantee inventory until booking is implemented.
- Accessibility and responsive behavior are expected for the Angular client.

## Extension Points

- Additional availability providers through the provider interface.
- Provider-specific adapters for third-party hotel inventory APIs.
- Caching for repeated destination and date searches.
- Sorting and filtering capabilities on the API or client side.
- Authentication for saved searches or operator workflows.
- Booking, payment, and reservation confirmation workflows.
- Observability through structured logging, metrics, and tracing.
- Localization for currency, date formats, and display text.

## High-Level Architecture

```text
Angular Client
  -> Availability API Endpoint
    -> Request Validation
      -> Availability Application Service
        -> Availability Provider Interface
          -> Sample Provider or External Provider Adapter
        -> Result Normalization
    -> JSON Response
```

Primary boundaries:

- Presentation boundary: Angular owns user interaction and result display.
- API boundary: .NET Minimal API owns HTTP contracts and response formatting.
- Application boundary: Availability service owns orchestration and business decisions.
- Provider boundary: Provider implementations own inventory source integration.
- Model boundary: Shared contracts remain stable and independent of provider-specific payloads.
