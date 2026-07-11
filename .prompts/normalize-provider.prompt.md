# Purpose

Use this prompt when generating normalization logic that converts provider-specific hotel availability responses into the common `HotelRoomDto` model.

This prompt is intended for new provider mappers, mapper refactors, and mapping-rule updates.

# Context

The repository implements the Hotel Stay Availability case study with .NET 8 Minimal API, C# 12, xUnit, dependency injection, SOLID principles, deterministic hotel providers, and GitHub Copilot Enterprise.

Providers may return different contract shapes, naming conventions, and levels of detail. The public API must return normalized DTOs and must never expose provider-specific contracts directly.

Before generating normalization logic, inspect:

- `HotelRoomDto`
- `RoomType`
- Existing provider contracts
- Existing provider mappers
- Price calculation helpers
- Cancellation policy conventions
- Provider filtering behavior
- Existing mapper tests

# Inputs

Provide the following inputs before using this prompt:

- Provider name and provider code
- Provider-specific response model
- Provider room type values
- Provider cancellation policy values
- Nightly rate fields
- Availability fields or filtering rules
- Amenities and rating fields, if present
- Destination and hotel identity fields
- Expected normalized output examples

# Instructions

Generate normalization logic that is safe, deterministic, and extensible.

- Convert provider-specific responses into `HotelRoomDto`.
- Map provider room type values into the common `RoomType` enum.
- Map provider cancellation policy values into the normalized cancellation policy text or enum used by the project.
- Calculate per-night price from the provider nightly rate.
- Calculate total stay price from per-night price and stay length.
- Handle unavailable rooms according to provider rules.
- Handle null or missing optional provider fields safely.
- Preserve deterministic behavior and avoid external calls.
- Keep provider-specific mapping isolated in a mapper class.
- Do not add provider-specific branching to orchestration services.
- Prefer small helper methods when mapping logic becomes complex.
- Preserve existing naming, nullability, async, and formatting conventions.
- Add or update mapper tests for positive, negative, and boundary scenarios.
- Do not generate placeholder logic or TODO comments.

# Expected Output

The generated change should include:

- Provider-specific normalization logic.
- Room type mapping.
- Cancellation policy mapping.
- Per-night and total stay price calculation.
- Unavailable-room handling.
- Null-safe handling for optional provider data.
- Tests that prove the mapper produces expected `HotelRoomDto` output.

# Validation Checklist

Verify the generated output:

- The solution builds successfully.
- All existing tests pass.
- Mapper tests cover room type mapping, cancellation policy mapping, price calculation, unavailable-room filtering, and null safety.
- Provider-specific contracts remain internal to provider and mapper boundaries.
- The normalized API model remains stable.
- The implementation remains extensible for future providers.
