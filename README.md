# Hotel Stay Availability

Hotel Stay Availability is an offline, deterministic hotel booking application built with .NET 8 Minimal API, Angular, xUnit, dependency injection, and clean architecture-style boundaries. The solution searches hotel rooms from two local providers, normalizes their different contracts, validates reservation documents, and stores confirmed reservations in memory.

No external APIs, database, authentication, or cloud services are required.

## Problem Statement

Hotel availability providers often expose different payload shapes, naming conventions, and levels of detail. The application solves that integration problem by isolating provider-specific contracts behind provider and mapper abstractions, then exposing a stable API and UI model for search and reservation workflows.

The reservation workflow enforces destination-specific identity document rules:

- Domestic destinations: Hyderabad, Bangalore, Mumbai require National ID.
- International destinations: London, Dubai, Singapore require Passport.
- Document mismatches return HTTP 422.

## Technology Stack

- .NET 8 Minimal API
- Angular standalone components
- xUnit
- C# records, enums, dependency injection, extension methods
- TypeScript strict mode and reactive forms
- In-memory repository for deterministic reservations
- Local provider implementations with no network dependencies

## Architecture

```text
Angular UI
  -> HotelApiService
    -> .NET Minimal API endpoints
      -> Validators
      -> Service layer
        -> IHotelProvider implementations
        -> IProviderRoomMapper implementations
        -> IReservationRepository
```

Primary backend boundaries:

- Endpoints: HTTP routing and status-code selection.
- DTOs: API-facing search, room, reservation, validation, and enum contracts.
- Providers: offline provider-specific availability contracts.
- Mappers: provider contract normalization into `HotelRoomDto`.
- Services: orchestration for search and reservations.
- Repositories: in-memory persistence abstraction.
- Validation: date and document-rule enforcement.

Adding a third provider should require a new `IHotelProvider`, a matching `IProviderRoomMapper`, and dependency injection registration. The availability service does not contain provider-specific branching.

## API Endpoints

### `GET /hotels/search`

Query parameters:

- `destination`
- `checkIn`
- `checkOut`
- `roomType` optional: `Standard`, `Deluxe`, `Suite`

Returns HTTP 400 for missing destination, missing check-in, missing check-out, or check-out less than or equal to check-in.

### `POST /hotels/reserve`

Accepts reservation details for a selected room and returns a deterministic reservation reference such as `HS-000001`.

Returns:

- HTTP 400 for missing required reservation/date fields.
- HTTP 422 when the supplied document type does not match the destination rule.

### `GET /hotels/reservation/{reference}`

Returns a confirmed in-memory reservation by reference, or HTTP 404 when not found.

## Provider Behavior

PremierStays uses a rich PascalCase contract with amenities, star rating, cancellation policy, and nightly rates.

BudgetNests uses a minimal snake_case contract with nightly rates and an `available` flag. Unavailable BudgetNests rooms are filtered during mapping.

Both providers normalize into `HotelRoomDto`, including per-night price and calculated total stay price.

## Repository Structure

```text
hotel-stay-availability/
  HotelStay.sln
  HotelStay.Api/          # .NET 8 Minimal API
  HotelStay.Tests/        # xUnit tests
  hotelstay-ui/           # Angular frontend
  README.md
  spec.md
  prompts.md
  reflection.md
```

## Setup

Prerequisites:

- .NET 8 SDK compatible with the installed SDK
- Node.js and npm

Restore and test backend:

```powershell
dotnet restore HotelStay.sln
dotnet test HotelStay.sln
```

Run API:

```powershell
dotnet run --project HotelStay.Api --launch-profile http
```

The API listens on `http://localhost:5000` for the `http` launch profile.

Install and build frontend:

```powershell
Push-Location hotelstay-ui
npm install
npm run build
Pop-Location
```

Run frontend:

```powershell
Push-Location hotelstay-ui
npm start
Pop-Location
```

The Angular dev server uses `http://localhost:4200` and the API CORS policy allows that origin.

## Testing

The xUnit suite covers:

- PremierStays normalization
- BudgetNests room filtering
- Room type mapping
- Total stay price calculation
- Date validation
- Domestic and international document validation
- Reservation reference generation
- Multi-provider availability service composition

## AI Tooling Approach

GitHub Copilot Enterprise Agent Mode was used in two phases:

- Design phase: create README, specification, prompt log, and reflection structure before implementation.
- Scaffold phase: generate the .NET solution, API, tests, Angular client, and updated documentation from the approved requirements.

Prompt history and workflow notes are captured in [prompts.md](prompts.md). Implementation observations are captured in [reflection.md](reflection.md).
