# Purpose

Use this prompt when generating or expanding xUnit tests for the Hotel Stay Availability solution.

This prompt should be used for backend business rules, provider normalization, validation, price calculation, reservation behavior, endpoint behavior, and regression coverage.

# Context

The repository implements an offline deterministic Hotel Stay Availability case study using .NET 8 Minimal API, C# 12, xUnit, dependency injection, SOLID principles, deterministic providers, and GitHub Copilot Enterprise.

Before generating tests, inspect the code under test and nearby existing tests. Match the current test naming, assertions, and Arrange-Act-Assert style.

Important areas include:

- Provider normalization
- Room type mapping
- Cancellation policy mapping
- Price calculation
- Date validation
- Document validation
- Provider filtering
- Reservation reference generation
- API status-code behavior

Tests must remain deterministic and must not depend on external APIs, databases, cloud services, random values, or current wall-clock time unless the code under test explicitly requires it and the value is controlled.

# Inputs

Provide the following inputs before using this prompt:

- Class, service, validator, mapper, or endpoint under test
- Business rule or regression being tested
- Expected inputs
- Expected outputs
- Positive scenarios
- Negative scenarios
- Boundary scenarios
- Existing test file or preferred new test file

# Instructions

Generate xUnit tests that are focused, deterministic, and readable.

- Follow Arrange-Act-Assert.
- Use meaningful test names that describe behavior and expected outcome.
- Cover positive scenarios.
- Cover negative scenarios.
- Cover boundary conditions.
- Use deterministic dates, provider data, prices, references, and assertions.
- Prefer real validators, mappers, services, and in-memory implementations when practical.
- Avoid unnecessary mocking.
- Keep each test focused on one behavior.
- Avoid over-testing framework behavior.
- Avoid brittle assertions that depend on incidental ordering unless ordering is part of the requirement.
- Add theory data when it reduces duplication without reducing readability.
- Preserve existing test project conventions.
- Do not add placeholder tests or TODO comments.

# Expected Output

The generated change should include:

- xUnit test methods or theory tests.
- Clear Arrange, Act, and Assert sections.
- Deterministic inputs and assertions.
- Test coverage for relevant success, failure, and edge cases.
- Minimal test doubles only when real collaborators would make the test unclear or nondeterministic.

# Validation Checklist

Verify the generated output:

- The test project builds successfully.
- All tests pass consistently.
- Test names clearly describe the behavior under test.
- Positive, negative, and boundary cases are covered.
- Tests do not rely on external systems or nondeterministic behavior.
- Tests avoid unnecessary mocking.
- The tests would fail if the business rule regressed.
