# Hotel Stay Availability

Hotel Stay Availability is an offline, deterministic hotel booking application for the SkyRoute case study. It searches local hotel provider stubs, normalizes their different contracts, validates destination-specific identity document rules, and confirms reservations in memory.

No external APIs, credentials, databases, authentication, cloud services, or nondeterministic data are required.

## Quick Reference

| Topic | Details |
| --- | --- |
| Domestic destinations | `Hyderabad`, `Bangalore`, `Mumbai` |
| International destinations | `London`, `Dubai`, `Singapore` |
| Accepted documents | Domestic: `NationalId` or `Passport`; international: `Passport` only |
| API URL | `http://localhost:5000` |
| Swagger URL | `http://localhost:5000/swagger` |
| Angular URL | `http://localhost:4200` |
| Run API | `dotnet run --project HotelStay.Api\HotelStay.Api.csproj --launch-profile http` |
| Run UI | `Push-Location hotelstay-ui; npm start; Pop-Location` |
| Backend tests | `dotnet test HotelStay.Tests\HotelStay.Tests.csproj` |
| Angular tests | `Push-Location hotelstay-ui; npm test -- --watch=false --browsers=ChromeHeadless; Pop-Location` |
| Playwright tests | `Push-Location hotelstay-ui; npm run e2e; Pop-Location` |
| Reviewer notes | Offline deterministic providers, runtime UTC reservation timestamps, in-memory storage by scope, no secrets, no database, no external APIs |

Sample reservation payload:

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

## Requirements Covered

- Search hotel rooms by destination, check-in date, check-out date, and optional room type.
- Query two deterministic providers: PremierStays and BudgetNests.
- Normalize provider-specific payloads into one public API contract.
- Filter unavailable BudgetNests rooms.
- Display provider badge, room type, per-night price, total stay price, and cancellation policy.
- Validate stay dates and reservation document rules on the backend.
- Mirror date and document validation in the Angular frontend.
- Return HTTP 400 for malformed or missing required input.
- Return HTTP 422 for document/destination mismatches.
- Confirm reservations with deterministic references such as `HS-000001`.
- Retrieve reservations by reference while the API process is running.

## Technology Stack

- .NET 8 Minimal API
- C# 12 records, enums, dependency injection, and typed Minimal API results
- Swagger/OpenAPI through Swashbuckle
- Angular 20 standalone components
- Angular signals, Reactive Forms, HttpClient, SCSS
- xUnit backend unit and integration tests
- Karma/Jasmine Angular unit tests
- Playwright browser end-to-end test
- In-memory deterministic provider and reservation data

## Architecture

```text
Angular UI
  -> HotelApiService
    -> .NET Minimal API endpoints
      -> Request validation
      -> HotelStay.Domain
        -> HotelSearchService
          -> IHotelProvider implementations
          -> IProviderRoomNormalizer strategies
        -> DocumentValidationService
        -> ReservationService
        -> IReservationStore
          -> InMemoryReservationStore
```

Primary backend boundaries:

- `HotelStay.Api/Program.cs`: Minimal API routing, HTTP status-code selection, Swagger metadata, and thin request validation.
- `HotelStay.Domain/Domain/Dtos`: public search, room, reservation, and normalized response contracts.
- `HotelStay.Domain/Domain/Enums`: stable JSON enum values for room type, document type, and cancellation policy.
- `HotelStay.Domain/Domain/ProviderContracts`: provider abstraction and provider result wrapper.
- `HotelStay.Domain/Domain/Providers`: deterministic PremierStays and BudgetNests provider stubs.
- `HotelStay.Domain/Domain/ProviderModels`: provider-specific source payload models isolated from API clients.
- `HotelStay.Domain/Domain/Normalization`: provider-specific normalizer strategies that convert source payloads into `HotelRoomDto`.
- `HotelStay.Domain/Domain/Services`: search orchestration, document validation, UTC reservation timestamping, and reservation confirmation.
- `HotelStay.Domain/Domain/Stores`: reservation persistence abstraction and offline in-memory implementation.
- `HotelStay.Api/Validation`: request validators for search and reservation inputs.
- `HotelStay.Api/Dtos/ValidationProblemResponse.cs`: shared field-level validation response contract.
- `HotelStay.Api/Documentation`: Swagger/OpenAPI examples.

Adding a third provider should be additive: introduce a provider-specific source contract, implement `IHotelProvider`, add an `IProviderRoomNormalizer`, register both with dependency injection, and add tests.

## API

The API runs at `http://localhost:5000` with the `http` launch profile.

Swagger UI is available in development at:

```text
http://localhost:5000/swagger
```

### `GET /hotels/search`

Query parameters:

- `destination` required: `Hyderabad`, `Bangalore`, `Mumbai`, `London`, `Dubai`, or `Singapore`
- `checkIn` required ISO date, for example `2026-08-10`
- `checkOut` required ISO date, must be after check-in
- `roomType` optional: `Standard`, `Deluxe`, or `Suite`

Successful responses return a JSON array of normalized rooms.

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

HTTP 400 is returned for missing destination, missing dates, invalid date formats, or checkout on/before check-in.

### `POST /hotels/reserve`

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

Successful responses use HTTP 201 and include a `Location` header for `/hotels/reservation/{reference}`.

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

HTTP 400 is returned for missing destination, missing room/provider fields, missing guest name, missing document number, invalid dates, checkout on/before check-in, or non-positive per-night price.

HTTP 422 is returned when the document type is not valid for the destination.

### `GET /hotels/reservation/{reference}`

Returns a confirmed reservation from in-memory storage, or HTTP 404 when the reference is not found.

## Document Rules

- Domestic destinations: `Hyderabad`, `Bangalore`, `Mumbai`
- International destinations: `London`, `Dubai`, `Singapore`
- International reservations require `Passport`.
- Domestic reservations accept `NationalId` or `Passport`.
- Document mismatches return HTTP 422 with field-level validation details.

## Run Locally

Prerequisites:

- .NET 8 SDK
- Node.js and npm
- Chrome for the Playwright e2e tests

Restore and test backend:

```powershell
dotnet restore HotelStay.sln
dotnet test HotelStay.Tests\HotelStay.Tests.csproj
```

Run API:

```powershell
dotnet run --project HotelStay.Api\HotelStay.Api.csproj --launch-profile http
```

Install frontend dependencies:

```powershell
Push-Location hotelstay-ui
npm install
Pop-Location
```

Run frontend:

```powershell
Push-Location hotelstay-ui
npm start
Pop-Location
```

The Angular app runs at `http://localhost:4200`.

## Run Tests

Backend unit and API integration tests:

```powershell
dotnet test HotelStay.Tests\HotelStay.Tests.csproj
```

Angular unit tests:

```powershell
Push-Location hotelstay-ui
npm test -- --watch=false --browsers=ChromeHeadless
Pop-Location
```

Playwright e2e tests:

```powershell
Push-Location hotelstay-ui
npm run e2e
Pop-Location
```

The Playwright configuration starts or reuses the API at `http://localhost:5000` and Angular at `http://localhost:4200`. The e2e suite covers the happy reservation flow plus invalid date and international document mismatch paths.

## Repository Structure

```text
hotel-stay-availability/
  .github/copilot-instructions.md
  .prompts/
  HotelStay.Api/
  HotelStay.Domain/
  HotelStay.Tests/
  hotelstay-ui/
  README.md
  spec.md
  prompts.md
  reflection.md
```

## AI Usage

GitHub Copilot Enterprise Agent Mode in VS Code was used across analysis, specification, implementation, testing, documentation, refactoring, and validation. Prompt history is captured in [prompts.md](prompts.md), and design/tooling trade-offs are captured in [reflection.md](reflection.md).