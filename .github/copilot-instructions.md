# GitHub Copilot Instructions

## Repository Purpose

This repository contains the Hotel Stay Availability solution: an offline, deterministic hotel booking application built with .NET 8 Minimal API, C# 12, Angular, xUnit, dependency injection, and SOLID principles.

The expected architecture separates HTTP endpoints, DTOs, validation, services, provider integrations, mapping, and in-memory persistence. Hotel providers may expose different source contracts, but clients must receive normalized application DTOs. The solution must remain fully offline, must not call external APIs, must not require a database, and must keep provider data deterministic.

Design all changes so adding a third hotel provider requires minimal additive work: introduce a provider, introduce a mapper if needed, register both with dependency injection, and add tests. Do not modify existing provider implementations unless their own behavior is changing.

## Architecture Guidelines

- Use .NET 8 Minimal API for backend HTTP endpoints.
- Follow SOLID principles and clean architecture boundaries.
- Prefer composition over inheritance.
- Use constructor dependency injection for services, validators, repositories, mappers, and providers.
- Keep endpoints thin: endpoints should route, validate, choose status codes, and delegate business behavior.
- Place business logic inside focused services, validators, mappers, or domain-oriented helpers.
- Use extension methods where they improve readability or centralize registration and shared calculations.
- Design for extensibility and avoid provider-specific branching in orchestration services.
- Ensure a new hotel provider can be added without modifying existing providers.
- Keep provider-specific response contracts isolated from public API DTOs.

## Coding Standards

- Use C# 12 features when they improve clarity and remain easy to maintain.
- Prefer immutable `record` DTOs for request, response, and normalized data contracts.
- Use async methods for service and repository contracts that represent I/O or future I/O boundaries.
- Return `TypedResults` or consistent Minimal API results where applicable.
- Validate all inputs before executing business logic.
- Avoid duplicate logic; extract shared rules into validators, services, or extension methods.
- Keep methods focused, readable, and reasonably small.
- Use meaningful names that describe business intent.
- Keep code compile-ready after every change.
- Avoid placeholder implementations, dead code, sample-only stubs without deterministic behavior, and TODO comments.
- Preserve existing formatting, nullability, and project conventions.

## Naming Conventions

- Interfaces use the `I` prefix, such as `IHotelProvider`.
- Services use clear business names, such as `HotelSearchService`, `ReservationService`, and `DocumentValidationService`.
- DTO names use the suffixes `Request`, `Response`, or `Dto`.
- Enums use singular domain names, such as `RoomType`, `CancellationPolicy`, and `DocumentType`.
- Provider classes use provider-specific names, such as `PremierStaysProvider` and `BudgetNestsProvider`.
- Mapper names should identify the source provider and target behavior, such as `PremierStaysRoomMapper`.
- Test classes should name the behavior under test, such as `ReservationValidationTests` or `AvailabilityMappingTests`.

## API Standards

- Return consistent error responses with field-level validation details.
- Use HTTP status codes correctly:
  - `200 OK` for successful searches and reads.
  - `201 Created` for successful reservations.
  - `400 Bad Request` for malformed or missing required input.
  - `404 Not Found` for missing reservation references.
  - `422 Unprocessable Entity` for valid requests that violate document rules.
- Validate requests before business logic or provider calls.
- Normalize provider responses before returning data to clients.
- Never expose provider-specific contracts directly through public API endpoints.
- Keep enum and JSON contracts stable for the Angular frontend.
- Keep the API fully offline and deterministic.

## Testing Standards

- Every business rule must have unit tests.
- Use xUnit for backend tests.
- Follow Arrange-Act-Assert in tests.
- Cover positive, negative, and boundary scenarios.
- Prefer deterministic tests with fixed dates, fixed provider data, and predictable reservation references.
- Avoid unnecessary mocking; prefer real validators, mappers, and in-memory implementations when practical.
- Add or update tests whenever validation rules, provider mapping, price calculation, reservation behavior, or API contracts change.
- Tests must not depend on external APIs, databases, cloud services, network availability, or wall-clock randomness.

## Frontend Guidelines

- Use Angular with standalone components.
- Prefer functional, reusable, focused UI components and services.
- Use reactive forms for validation-heavy workflows.
- Validate forms client-side before submitting to the API.
- Keep UI responsive, accessible, and keyboard-friendly.
- Display loading, empty, validation error, and success states explicitly.
- Keep frontend models aligned with backend DTOs.
- Do not hardcode provider-specific payloads in UI components; consume normalized API responses.

## AI Behavior

When generating or modifying code:

- Maintain consistency with the existing architecture.
- Read the existing codebase before generating new code.
- If a model changes, update all affected backend DTOs, frontend models, mappers, validators, tests, and documentation.
- Explain architectural decisions before large or cross-cutting changes.
- Never generate incomplete implementations.
- Never introduce compile errors.
- Preserve existing project conventions and naming patterns.
- Prefer incremental changes over rewriting working code.
- Keep changes scoped to the user request.
- Do not introduce external APIs, databases, authentication, cloud deployment, nondeterministic provider data, or hidden runtime dependencies unless explicitly requested.
- Validate changes with focused tests or builds whenever possible.
