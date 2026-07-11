# Purpose

Use this prompt when adding a new deterministic hotel provider to the Hotel Stay Availability solution.

This prompt should be used for provider additions that must follow the existing provider architecture without modifying existing provider implementations.

# Context

The repository implements a Hotel Stay Availability case study using .NET 8 Minimal API, C# 12, xUnit, dependency injection, SOLID principles, deterministic stub providers, and GitHub Copilot Enterprise.

Before generating code, inspect the existing provider architecture, including:

- `IHotelProvider`
- Existing provider implementations such as `PremierStaysProvider` and `BudgetNestsProvider`
- Provider-specific response models
- Provider room mappers
- `HotelRoomDto`
- Dependency injection registration
- Existing provider and mapping tests

The solution must remain fully offline. Do not introduce external APIs, databases, authentication, cloud services, random provider behavior, or nondeterministic data.

# Inputs

Provide the following inputs before using this prompt:

- Provider name
- Provider code or badge text
- Provider-specific response contract shape
- Supported destinations
- Supported room types
- Nightly rate examples
- Availability rules
- Cancellation policy behavior
- Amenities or rating data, if supported
- Expected unavailable-room behavior

# Instructions

Generate a complete provider addition that follows the Open/Closed Principle.

- Implement `IHotelProvider` for the new provider.
- Keep provider data deterministic and in memory.
- Define provider-specific models that reflect the provider's native contract.
- Do not expose provider-specific models directly through API endpoints.
- Add a provider mapper that converts the provider contract into `HotelRoomDto`.
- Map provider-specific room type values into the common `RoomType` enum.
- Calculate per-night price and total stay price consistently with existing logic.
- Handle unavailable rooms according to the provider rules.
- Register the provider and mapper through dependency injection.
- Add or update tests for the new provider and mapper.
- Avoid changing existing provider implementations unless a shared abstraction must evolve.
- If a shared abstraction changes, update all affected providers, mappers, tests, documentation, and frontend models as needed.
- Preserve existing naming, folder, nullability, async, and formatting conventions.
- Do not generate placeholder code or TODO comments.

# Expected Output

The generated change should include:

- A new provider class implementing `IHotelProvider`.
- Provider-specific response model records or classes.
- A mapper that normalizes the provider response into `HotelRoomDto`.
- Dependency injection registration for the new provider and mapper.
- xUnit tests covering provider normalization and any provider-specific rules.
- Documentation updates only if the provider changes public behavior or documented architecture.

# Validation Checklist

Verify the generated output:

- The solution builds successfully.
- All existing tests pass.
- New xUnit tests cover the provider's positive, negative, and boundary behavior.
- Existing providers were not modified unnecessarily.
- The availability service does not contain provider-specific branching.
- The new provider works offline with deterministic data.
- Provider-specific contracts are not returned directly from public API endpoints.
- A future provider can still be added using the same pattern.
