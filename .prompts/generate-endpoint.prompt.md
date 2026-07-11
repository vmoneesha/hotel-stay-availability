# Purpose

Use this prompt when generating a new .NET 8 Minimal API endpoint for the Hotel Stay Availability solution.

This prompt is intended for endpoint additions that should follow the existing API style, validation approach, dependency injection patterns, and error response conventions.

# Context

The repository implements a deterministic offline hotel booking case study using .NET 8 Minimal API, C# 12, xUnit, dependency injection, SOLID principles, deterministic providers, and GitHub Copilot Enterprise.

Before generating an endpoint, inspect the existing API surface and related code:

- `Program.cs` endpoint mappings
- Request and response DTOs
- Validators
- Services
- Repositories
- Error response models
- Existing endpoint status-code behavior
- Existing xUnit test coverage

Endpoints should remain thin. Business logic belongs in services, validators, mappers, repositories, or extension methods.

# Inputs

Provide the following inputs before using this prompt:

- Endpoint route
- HTTP method
- Request source: route, query string, or body
- Request DTO shape
- Response DTO shape
- Required validation rules
- Business service to call
- Expected success status code
- Expected error status codes
- Test scenarios to cover

# Instructions

Generate a Minimal API endpoint that matches the repository architecture.

- Use .NET 8 Minimal API conventions.
- Use constructor or parameter dependency injection for validators, services, repositories, or mappers.
- Validate request data before business logic.
- Return consistent validation error responses.
- Use proper HTTP status codes.
- Prefer `TypedResults` or strongly typed Minimal API result patterns where applicable.
- Keep endpoint logic focused on routing, validation, status-code selection, and delegation.
- Place business logic in a service or validator instead of the endpoint.
- Handle not-found, validation, and business-rule failures explicitly.
- Avoid leaking provider-specific contracts through the endpoint response.
- Add XML comments only for public APIs where they add useful documentation.
- Preserve existing naming, nullability, async, and formatting conventions.
- Do not add placeholder code, TODO comments, external APIs, databases, authentication, or nondeterministic behavior.
- Suggest unit or integration tests for the endpoint behavior.

# Expected Output

The generated change should include:

- A Minimal API route mapping.
- Request and response DTOs if needed.
- Validator changes if needed.
- Service method changes if needed.
- Consistent error responses.
- Unit test or integration test suggestions, and test implementation when requested.
- Documentation updates only if the public API contract changes.

# Validation Checklist

Verify the generated output:

- The solution builds successfully.
- Existing tests pass.
- New endpoint validates inputs before business logic.
- Success and failure responses use correct HTTP status codes.
- Error responses follow existing response shape.
- Endpoint remains thin and delegates business logic.
- Provider-specific contracts are not exposed.
- Suggested or generated tests cover positive, negative, and boundary scenarios.
