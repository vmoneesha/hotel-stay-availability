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

## Prompt Purposes

| Prompt | Purpose | Result |
| --- | --- | --- |
| Documentation-only design phase | Define scope before code | README, spec, prompt log, reflection headings |
| Full solution scaffold | Generate runnable solution | `HotelStay.sln`, `HotelStay.Api`, `HotelStay.Tests`, `hotelstay-ui` |
| Repository-level Copilot instructions | Standardize future AI-assisted work | `.github/copilot-instructions.md` |
| Reusable prompt engineering assets | Create lifecycle prompt files | `.prompts/*.prompt.md` |
| Domain layer only | Add domain contracts without implementation logic | `HotelStay.Api/Domain` |
| Provider layer only | Add deterministic domain providers without services/endpoints | `HotelStay.Api/Domain/Providers` |
| Business services only | Add provider search, reservation, and document services without endpoints | `HotelStay.Api/Domain/Services` |
| Minimal API endpoints only | Wire API routes with TypedResults and DI | `HotelStay.Api/Program.cs` |
| Provider architecture requirements | Preserve extensibility | `IHotelProvider`, provider mappers, DI registration |
| Validation and testing requirements | Encode expected behavior | xUnit tests for mapping, price, filtering, date, document, and reference rules |

## Notes for Future Prompts

- Ask for API integration tests against the Minimal API endpoints.
- Ask for Angular component tests or Playwright flows for search, reservation, and confirmation.
- Keep third-provider work additive: new provider, new mapper, DI registration, tests.
- Consider moving destination/document rules into configuration if the list grows.
- Ask for review before adding persistence, authentication, payment, or cloud deployment.
- Keep offline determinism unless a future prompt explicitly changes that constraint.
- Update `.github/copilot-instructions.md` whenever architectural constraints or coding conventions materially change.
- Keep reusable prompt files in `.prompts` synchronized with current architecture and technology choices.
- Keep domain contracts additive until the implementation layer is intentionally migrated to consume them.
- Keep provider-layer additions deterministic and isolated from services, endpoints, and existing provider implementations unless a migration is explicitly requested.
- Keep business service additions endpoint-free until API integration is explicitly requested.
- Keep endpoint prompts focused on routing, validation, status codes, DI, and delegation; do not introduce frontend work unless explicitly requested.
