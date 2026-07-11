# Execute All Tests

## Purpose

Run and validate all automated tests across the solution.

## Context

The solution contains:

- .NET 8 Minimal API
- xUnit backend tests
- Angular frontend tests

Do not regenerate tests.

Only execute, analyze, and summarize results.

## Instructions

### Backend

Run all xUnit tests.

Verify:

- DocumentValidationService
- HotelSearchService
- ReservationService
- Provider normalization
- Price calculation
- Room filtering
- Validation logic

### Frontend

Run all Angular unit tests.

Verify:

- Components
- Services
- Reactive Forms
- Date validation
- Document validation
- API services
- UI interactions

### Test Analysis

Provide:

- Total tests executed
- Passed tests
- Failed tests
- Skipped tests
- Execution time
- Coverage summary (if available)

If any tests fail:

- Explain the cause.
- Recommend the fix.
- Do not modify production code unless necessary.

## Expected Output

Produce a concise test execution summary highlighting any failures or warnings.
