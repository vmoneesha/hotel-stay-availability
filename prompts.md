# Copilot Prompt Log

## Prompt 1: Documentation-Only Design Phase

```text
You are acting as a Senior Software Architect.

Update README.md and create spec.md, prompts.md, and reflection.md.
Do NOT generate source code or project files yet.
Complete the analysis and design phase before implementation.
```

Purpose: establish project intent, architecture, API contracts, assumptions, and AI workflow before implementation.

## Prompt 2: Full Solution Scaffold

```text
You are an expert .NET 8 Solution Architect, Senior Backend Engineer, Senior Frontend Engineer and QA Engineer.

Autonomously scaffold an entire production-quality solution following clean architecture principles.
Build an end-to-end hotel booking application using .NET 8 Minimal API, Angular, xUnit, dependency injection, clean code, and SOLID principles.
The solution must run completely offline with no external APIs, database, authentication, or cloud deployment.
```

Purpose: move from design into a complete runnable scaffold with API, frontend, tests, provider architecture, validation rules, and documentation.

## Prompt 3: Repository-Level Copilot Instructions

```text
You are acting as a Senior Software Architect and AI Engineering Lead.

Generate a repository-level GitHub Copilot custom instructions file at:

.github/copilot-instructions.md

This file will guide GitHub Copilot across the entire Hotel Stay Availability solution and should enforce consistent coding standards, architectural decisions, naming conventions, testing practices, and AI behavior.

Project Context

The solution is being built for a Hotel Stay Availability system using:

- .NET 8 Minimal API
- C# 12
- Angular
- xUnit
- Dependency Injection
- SOLID Principles
- GitHub Copilot Enterprise

The solution must remain fully offline, use deterministic stub providers, and be designed so that adding a third hotel provider requires minimal changes.

Generate a professional copilot-instructions.md that includes repository purpose, architecture guidelines, coding standards, naming conventions, API standards, testing standards, frontend guidelines, and AI behavior.

Write the file in clean Markdown with clear headings and concise bullet points.

Do not generate any other files or modify existing code.
```

Purpose: create project-wide Copilot guidance so future AI-assisted changes preserve the offline architecture, provider extensibility, naming standards, testing expectations, and implementation quality bar.

## Prompt 4: Reusable Prompt Engineering Assets

```text
You are acting as an AI Engineering Lead responsible for establishing reusable prompt engineering assets for this repository.

The project already contains:

- README.md
- spec.md
- prompts.md
- reflection.md
- .github/copilot-instructions.md

Generate a new folder named:

.prompts

Inside it, create reusable GitHub Copilot Prompt Files (.prompt.md) that will be used throughout the project lifecycle.

Create the following files:

1. add-provider.prompt.md
2. generate-endpoint.prompt.md
3. generate-tests.prompt.md
4. normalize-provider.prompt.md

Each prompt file should follow a consistent structure with the following sections:

# Purpose
# Context
# Inputs
# Instructions
# Expected Output
# Validation Checklist

Project Context

This repository implements the Hotel Stay Availability case study using:

- .NET 8 Minimal API
- Angular
- xUnit
- Dependency Injection
- SOLID Principles
- Deterministic hotel providers
- GitHub Copilot Enterprise

The prompt files should be generic enough to be reusable but specific enough to enforce the project's architecture and coding conventions.

Create prompt files for adding providers, generating endpoints, generating tests, and normalizing provider-specific responses into `HotelRoomDto`.

Write all prompt files in clean Markdown.

Do not modify any implementation files.
Only create the reusable prompt files inside the .prompts folder.
```

Purpose: create reusable lifecycle prompt files for provider extension, endpoint generation, test generation, and provider normalization while preserving the solution architecture and offline deterministic constraints.

## Prompt 5: Domain Layer Only

```text
Continue working in the existing Hotel Stay Availability repository.

Use the existing repository context including:

- spec.md
- README.md
- .github/copilot-instructions.md
- .prompts
- Previous design decisions

Do not regenerate any existing files.

Implement ONLY the domain layer.

Generate:

Enums
- RoomType
- CancellationPolicy
- DocumentType

DTOs
- HotelSearchRequest
- HotelRoomDto
- ReservationRequest
- ReservationResponse
- ReservationDetailsDto

Provider Contracts
- IHotelProvider

Provider Models
- PremierStays models
- BudgetNests models

Requirements

- .NET 8
- C# 12
- record DTOs
- XML comments only for public contracts
- compile-ready code
- explain each generated file
- do not implement services
- do not implement endpoints
```

Purpose: add an additive domain contract layer without changing existing runtime endpoints, services, mappers, repositories, or provider implementations.

## Prompt 6: Provider Layer Only

```text
Using the existing architecture, implement only the provider layer.

Generate

PremierStaysProvider

BudgetNestsProvider

Implement IHotelProvider.

Requirements

PremierStays

- PascalCase response
- Rate
- Amenities
- Star Rating
- Cancellation Policy

BudgetNests

- snake_case response
- available flag
- cancellation
- nightly rate

The providers must be deterministic.

Use hardcoded sample data.

BudgetNests must return some unavailable rooms.

Do not generate services or endpoints.
```

Purpose: add deterministic provider-layer implementations over the domain `IHotelProvider` contract without adding services, endpoints, or orchestration logic.

## Prompt 7: Business Services Only

```text
Implement only the business services.

Generate

HotelSearchService

ReservationService

DocumentValidationService

Requirements

HotelSearchService

- query all providers
- normalize responses
- filter unavailable rooms
- calculate total stay price

ReservationService

- generate reservation reference
- return confirmation

DocumentValidationService

Rules

Domestic

Hyderabad
Bangalore
Mumbai

International

London
Dubai
Singapore

International requires Passport.

Domestic accepts National ID.

Do not generate endpoints.
```

Purpose: add business service classes for provider search orchestration, response normalization, reservation confirmation, and document validation without generating endpoints.

## Prompt 8: Minimal API Endpoints Only

```text
Generate only the Minimal API endpoints.

Implement

GET /hotels/search

POST /hotels/reserve

GET /hotels/reservation/{reference}

Requirements

Return 400

missing destination

missing dates

checkout before checkin

Return 422

invalid document

Use TypedResults.

Register DI.

No frontend.
```

Purpose: wire the Minimal API endpoints to the existing domain services using dependency injection, typed results, validation status codes, and in-memory reservation lookup without frontend changes.

## Prompt 9: Frontend Implementation Only

```text
Do not regenerate any existing backend code.

Implement ONLY the frontend for the Hotel Stay Availability Angular application.

Requirements

- Use Angular 20.
- Use Standalone Components.
- Use Angular Signals where appropriate.
- Use Reactive Forms.
- Use HttpClient to call the existing backend API.
- Use SCSS.
- Create a premium, modern, responsive hotel booking design inspired by Booking.com, but unique to this project.
- Keep the UI responsive, accessible, and keyboard-friendly.
- Display loading, empty, validation error, and success states.
- Keep frontend models aligned with backend DTOs.
- Do not hardcode provider-specific payloads in UI components.
- Do not modify the backend.
- Generate compile-ready Angular code only.
- Explain each generated folder before generating files.
```

Purpose: create a componentized Angular booking experience with reusable search, results, reservation, confirmation, badge, loading, error, empty, and pricing components while preserving the existing backend API unchanged.

## Prompt 10: Date Picker Validation

```text
Implement date picker validation for the Hotel Stay Availability Angular application.

Requirements

Check-in Date

- Disable all dates before the current date.
- Users must not be able to select any past date.
- Set the minimum selectable date to today's date.
- Default the check-in date to today's date.
- Prevent manual entry of invalid past dates.

Check-out Date

- Disable all dates that are before or equal to the selected check-in date.
- The minimum selectable check-out date must always be one day after the selected check-in date.
- Automatically update the minimum check-out date whenever the check-in date changes.
- If the user changes the check-in date and the existing check-out date becomes invalid, automatically clear the check-out date and prompt the user to select a new one.

Validation

- Use Angular Reactive Forms.
- Validate dates in real time.
- Show inline validation messages.
- Highlight invalid fields.
- Disable the Search button until all date validations pass.

User Experience

- Display today's date as the default check-in date.
- Display tomorrow's date as the default check-out date.
- Prevent users from selecting invalid dates through both the date picker and manual keyboard input.
- Ensure the implementation works correctly across different time zones by comparing dates without time components.

Implementation

- Use Angular Material Datepicker (or the existing date picker used in the project).
- Use strongly typed TypeScript.
- Keep the implementation reusable.
- Follow Angular best practices.
- Do not modify any backend code.

Explain the implementation before generating the code.
```

Purpose: strengthen the reusable Angular search bar with date-only defaults, dynamic minimum dates, real-time reactive validation, invalid field styling, and submit disabling without changing backend behavior.

## Prompt 11: Reservation and Confirmation UX Enhancement

```text
Continue working in the existing Angular application.

Enhance the Reservation and Confirmation workflow by improving the overall user experience and making the booking process feel similar to a modern hotel booking platform.

Implement a three-step booking progress indicator:

1. Your Selection
2. Your Details
3. Finish Booking

Improve document selection based on destination, redesign the reservation details page, add a sticky booking summary, and redesign the confirmation page with a polished success experience.

Use Angular Standalone Components, Angular Reactive Forms, Angular Material, SCSS, responsive layouts, reusable components, accessible design, clean typography, a blue and white theme, consistent spacing, and smooth transitions.

Do not modify the backend.

Explain the implementation approach before generating the code.
```

Purpose: add the booking progress indicator, sticky booking summary, enhanced reservation form layout, and polished confirmation success page while keeping backend code unchanged.

## Prompt 12: Comprehensive xUnit Tests

```text
Generate comprehensive xUnit tests.

Cover

DocumentValidationService
HotelSearchService
ReservationService
Provider normalization
Unavailable room filtering
Price calculation
Boundary conditions
Positive
Negative
Arrange Act Assert.

Explain each test.
```

Purpose: add focused domain-level xUnit coverage for document validation, hotel search normalization/filtering/ordering, price calculations, reservation confirmations, invalid document handling, and boundary scenarios using explicit Arrange / Act / Assert sections.

## Prompt 13: Comprehensive Angular Unit Tests

```text
Continue working in the existing Angular application.

Generate comprehensive Angular unit tests for the frontend.

Use the project's existing testing framework.
Cover components, services, reactive forms, business rules, HTTP behavior, and user interaction.
Use Angular TestBed and HttpTestingController.
Follow Arrange / Act / Assert.
Cover positive, negative, and boundary scenarios.
Do not modify production code unless a small change is required to improve testability.
```

Purpose: add deterministic Angular TestBed coverage for API calls, selection-store state, search form validation, hotel card interaction, reservation form document rules, search/reservation page workflows, and confirmation lookup behavior.

## Prompt 14: Runtime, Test, and Validation Prompt Files

```text
Add .prompts/

run-application.prompt.md
run-tests.prompt.md
validate-solution.prompt.md

Create reusable GitHub Copilot prompt files for running the complete application, running all tests, and validating the full solution.
The run-application prompt must cover backend startup, frontend startup, Swagger/OpenAPI, endpoint availability, browser console checks, and complete booking workflow verification.
```

Purpose: add reusable operational prompts for running the full solution, executing backend/frontend tests, and validating builds, runtime behavior, API/frontend integration, and booking workflows before handoff.

## Prompt 15: Swagger/OpenAPI Documentation

```text
Continue working in the existing Hotel Stay Availability .NET 8 Minimal API project.

Enable Swagger/OpenAPI support for the API while following .NET 8 best practices.

Register all required Swagger/OpenAPI services, configure middleware in Program.cs, enable Swagger UI for Development, document all Minimal API endpoints, group related endpoints under Hotels, add summaries and descriptions, include request and response schemas, document validation responses, configure launchSettings.json to open Swagger by default, and add meaningful examples for requests, successful responses, and validation errors.

Do not modify business logic or endpoint behavior.
```

Purpose: enable professional Swagger/OpenAPI documentation for the Minimal API with Development-only Swagger UI, Hotels endpoint grouping, summaries, descriptions, schemas, examples, validation response metadata, and launch settings that open Swagger by default.

## Prompt 16: Case Study Hardening and Review Fixes

```text
Apply the prioritized case-study improvement list:

1. Add IProviderRoomNormalizer strategy pattern.
2. Move endpoint validation out of Program.cs.
3. Add GitHub Actions CI.
4. Add one or two negative-path Playwright tests.
5. Make sure prompts.md includes the final cleanup/refactor/test prompts.
```

Purpose: convert review findings into implementation improvements by making provider normalization additive, moving request validation into injectable validators, adding continuous integration, expanding e2e coverage for invalid dates and document mismatch, and updating the AI prompt log.

## Prompt 17: Comprehensive Recommendation Review and Implementation

```text
Continue working in the existing Hotel Stay Availability solution.

The application is already complete and functional.

Perform a comprehensive review of the entire repository and implement the following improvements without changing the existing business functionality or user experience.

Do not regenerate existing features.

Preserve all API contracts and Angular UI behavior.

Only make the improvements listed below.

------------------------------------------------------------
Backend Improvements
------------------------------------------------------------

1. Remove Dead API Mapping Logic

Review HotelApiService.search.

The backend currently returns a plain array.

Remove any unnecessary dual-shape response handling or dead code while keeping the implementation future-proof.

------------------------------------------------------------

2. Improve Architecture

Review the current architecture.

Where practical, improve the separation of concerns without introducing unnecessary complexity.

If a lightweight Domain class library can be introduced without breaking the solution, migrate shared contracts and abstractions there.

Otherwise, explain why the current structure is acceptable for the scope of this case study.

------------------------------------------------------------

3. Introduce Reservation Store Abstraction

Replace direct ConcurrentDictionary usage with an abstraction.

Create

IReservationStore

Implement

InMemoryReservationStore

Register it through Dependency Injection.

Update ReservationService to depend only on the interface.

------------------------------------------------------------

4. Remove Duplicate Validation

Review reservation validation.

Ensure DocumentValidationService is executed only once.

Validation should exist in the business layer rather than being duplicated inside Minimal API endpoints.

Keep HTTP responses unchanged.

------------------------------------------------------------

5. Reservation Timestamp

Review ReservationService.

Do not hardcode timestamps.

Use DateTimeOffset.UtcNow in production code.

If deterministic timestamps are required for tests, inject an abstraction such as

IClock

or

TimeProvider

rather than hardcoding dates.

------------------------------------------------------------

6. Backend Integration Tests

Generate WebApplicationFactory integration tests.

Cover

GET /hotels/search

POST /hotels/reserve

GET /hotels/reservation/{reference}

Verify

200

400

422

Successful reservation

Reservation retrieval

Keep tests deterministic.

------------------------------------------------------------
Frontend Improvements
------------------------------------------------------------

1. Reservation Navigation

Prevent direct navigation to the Reservation page without first selecting a hotel.

If no hotel is selected,

redirect users back to the Search page

or display a friendly message.

Implement using Angular Route Guards or existing routing best practices.

------------------------------------------------------------

2. Angular Consistency

Review the Angular project.

Use a consistent coding style.

Where appropriate,

replace constructor dependency injection with inject().

Use Signals consistently where already adopted.

Avoid mixing architectural styles unnecessarily.

------------------------------------------------------------

3. API Contract

Review HotelApiService.

Remove any dead response parsing logic.

Align the Angular service exactly with the backend response contract.

------------------------------------------------------------

4. End-to-End Tests

Generate Playwright end-to-end tests.

Cover

Search hotels

Sort results

Reserve hotel

Domestic booking

International booking

Document validation

Booking confirmation

Verify responsive navigation.

------------------------------------------------------------
Documentation Improvements
------------------------------------------------------------

Update README.md.

Add a quick reference table containing

Domestic Destinations

International Destinations

Accepted Document Types

Sample Reservation Payload

Swagger URL

Angular URL

Run Commands

Testing Commands

Reviewer Notes

------------------------------------------------------------
Code Quality
------------------------------------------------------------

Review the entire repository.

Improve

SOLID

Dependency Injection

Naming

Code duplication

Comments

Readability

Maintainability

Do not introduce unnecessary abstractions.

Avoid overengineering.

------------------------------------------------------------
Validation
------------------------------------------------------------

After completing all improvements,

verify

- Solution builds successfully.
- Angular builds successfully.
- Backend tests pass.
- Angular unit tests pass.
- Integration tests pass.
- Playwright tests pass.
- No compile warnings.
- No dead code.
- No duplicated validation.

------------------------------------------------------------
Output
------------------------------------------------------------

Before modifying files,

explain the proposed improvements.

After implementation,

summarize

- Files modified
- Architectural improvements
- Tests added
- Documentation updates
- Any recommendations that were intentionally not implemented and the reasons why.
```

Purpose: verify external review findings against the actual repository, fix only confirmed gaps, preserve behavior and user experience, introduce project-level domain separation, remove dead API-client parsing, centralize reservation validation/storage, add route-guard protection, modernize Angular confirmation state, expand e2e coverage, update documentation, and validate the complete solution.

## Workspace Context Hooks Used

| Context Hook | Example Usage | Why It Helped |
| --- | --- | --- |
| `#file` | `#file:challenge-hotelstay.pdf` and targeted references to files such as `README.md`, `spec.md`, `Program.cs`, and Angular component files | Kept Copilot grounded in the case-study brief and the exact implementation surface being changed. |
| `#codebase` | Repository-wide review prompts such as evaluating architecture, duplicate backend layers, validation coverage, provider extensibility, and final case-study readiness | Helped Copilot reason across backend, frontend, tests, prompt assets, and documentation instead of optimizing one file in isolation. |
| `#selection` | Focused edits and review of selected files/sections, especially `prompts.md`, `reflection.md`, UI components, endpoint handlers, and validation snippets | Reduced prompt ambiguity and kept small refactors scoped to the active text or code region. |

These context hooks were used deliberately so Agent Mode could combine the PDF requirements, current repository state, and selected implementation details while avoiding drift from earlier design decisions.


## Prompt Purposes

| Prompt | Purpose | Result |
| --- | --- | --- |
| Documentation-only design phase | Define scope before code | README, spec, prompt log, reflection headings |
| Full solution scaffold | Generate runnable solution | `HotelStay.sln`, `HotelStay.Api`, `HotelStay.Domain`, `HotelStay.Tests`, `hotelstay-ui` |
| Repository-level Copilot instructions | Standardize future AI-assisted work | `.github/copilot-instructions.md` |
| Reusable prompt engineering assets | Create lifecycle prompt files | `.prompts/*.prompt.md` |
| Domain layer only | Add domain contracts without implementation logic | `HotelStay.Domain/Domain` |
| Provider layer only | Add deterministic domain providers without services/endpoints | `HotelStay.Domain/Domain/Providers` |
| Business services only | Add provider search, reservation, and document services without endpoints | `HotelStay.Domain/Domain/Services` |
| Minimal API endpoints only | Wire API routes with TypedResults and DI | `HotelStay.Api/Program.cs` |
| Frontend implementation only | Build the Angular booking UI without backend changes | `hotelstay-ui/src/app` |
| Date picker validation | Add reusable date-only picker validation to search | `SearchBarComponent` |
| Reservation and confirmation UX enhancement | Add booking progress, sticky summary, and polished success flow | `BookingProgressComponent`, `BookingSummaryCardComponent`, reservation/confirmation pages |
| Document validation parity | Align frontend and backend document rules | `DocumentValidationService`, `ReservationValidator`, `ReservationFormComponent`, validation tests |
| International National ID UX | Make invalid international National ID selection guided and recoverable | `ReservationFormComponent` |
| Confirmation action simplification | Remove duplicate finish-booking action | `ConfirmationComponent` template |
| Comprehensive xUnit tests | Add broad domain service and provider behavior coverage | `HotelStay.Tests/DomainServiceTests.cs` |
| Comprehensive Angular unit tests | Add frontend service, component, form, HTTP, and workflow coverage | `hotelstay-ui/src/app/**/*.spec.ts` |
| Runtime, test, and validation prompt files | Add reusable operational prompt assets | `.prompts/run-application.prompt.md`, `.prompts/run-tests.prompt.md`, `.prompts/validate-solution.prompt.md` |
| Swagger/OpenAPI documentation | Add documented Swagger UI and OpenAPI metadata | `HotelStay.Api/Program.cs`, `HotelStay.Api/HotelStay.Api.csproj`, `HotelStay.Api/Properties/launchSettings.json` |
| Case study hardening and review fixes | Address final architecture, CI, e2e, and prompt-log gaps | `IProviderRoomNormalizer`, validators, `.github/workflows/ci.yml`, Playwright negative paths, `prompts.md` |
| Comprehensive recommendation review and implementation | Verify review findings and implement confirmed improvements without behavior or UX changes | `HotelStay.Domain`, `IReservationStore`, `ReservationService`, `HotelApiService`, route guard, Playwright flows, README quick reference |
| Workspace context hooks | Use `#file`, `#codebase`, and `#selection` to scope Copilot context deliberately | PDF-grounded analysis, repository-wide review, focused file/selection edits |
| Provider architecture requirements | Preserve extensibility | `IHotelProvider`, provider mappers, DI registration |
| Validation and testing requirements | Encode expected behavior | xUnit tests for mapping, price, filtering, date, document, and reference rules |

## Notes for Future Prompts

- Keep API integration tests against the Minimal API endpoints current as endpoint contracts evolve.
- Keep Angular component tests and Playwright flows current for search, reservation, confirmation, and negative validation paths.
- Keep third-provider work additive: new provider, new normalizer, DI registration, tests.
- Consider moving destination/document rules into configuration if the list grows.
- Ask for review before adding persistence, authentication, payment, or cloud deployment.
- Keep offline determinism unless a future prompt explicitly changes that constraint.
- Update `.github/copilot-instructions.md` whenever architectural constraints or coding conventions materially change.
- Keep reusable prompt files in `.prompts` synchronized with current architecture and technology choices.
- Keep domain contracts additive until the implementation layer is intentionally migrated to consume them.
- Keep provider-layer additions deterministic and isolated from services, endpoints, and existing provider implementations unless a migration is explicitly requested.
- Keep business service additions endpoint-free until API integration is explicitly requested.
- Keep endpoint prompts focused on routing, validation, status codes, DI, and delegation; do not introduce frontend work unless explicitly requested.
- Keep frontend-only prompts scoped to `hotelstay-ui` and validate with `npm run build`.
- Keep date validation reusable inside form components and compare date-only values to avoid timezone drift.
- Keep document validation rules implemented in both Angular Reactive Forms and backend services/validators.
- Keep international document mismatch UX guided: show a clear message, disable dependent fields when needed, and provide an easy path back to a valid Passport selection.
- Keep confirmation actions focused; avoid duplicate navigation choices that lead to the same route.
- Keep backend behavior covered with focused xUnit tests that use real deterministic providers and services when practical.
- Keep operational prompt files focused on running, testing, and validating the existing solution without changing working functionality.
- Keep Swagger/OpenAPI updates documentation-only for API behavior: add metadata, examples, and launch configuration without changing endpoint business logic.
- Keep provider normalization additive through `IProviderRoomNormalizer` strategies; adding a provider should not require provider-specific mapping inside `HotelSearchService`.
- Keep endpoint validators outside `Program.cs` so Minimal API handlers stay focused on routing, status codes, and delegation.
- Keep CI aligned with local validation: backend tests, frontend build, Angular unit tests, and Playwright e2e.
- Use `#file` for authoritative documents or implementation anchors, `#codebase` for cross-cutting reviews, and `#selection` for tightly scoped edits.
