# Purpose

Run the complete Hotel Stay Availability solution and verify that both the backend API and Angular frontend are functioning correctly.

# Context

Use the existing repository structure, project configuration, and environment settings.

The repository implements an offline deterministic Hotel Stay Availability solution using .NET 8 Minimal API, Angular, xUnit, dependency injection, SOLID principles, deterministic providers, and GitHub Copilot Enterprise.

Do not regenerate or modify application code unless required to resolve startup issues.

# Inputs

Provide the following inputs before using this prompt when they differ from repository defaults:

- Repository root path
- Backend project path
- Backend URL or port
- Frontend project path
- Frontend URL or port
- Browser to use for verification
- Any known startup constraints or already-running terminals

# Instructions

Run the complete application.

## Backend

- Restore NuGet packages.
- Build the .NET solution.
- Start the `HotelStay.Api` project.
- Verify there are no build errors.
- Confirm the API starts successfully.
- Display the API URL.
- Verify Swagger/OpenAPI is accessible.
- Check Dependency Injection registration.
- Verify all endpoints are available.

## Frontend

- Restore npm packages if required.
- Build the Angular application.
- Start the Angular development server.
- Verify compilation succeeds without errors.
- Display the application URL.
- Verify routing works correctly.
- Ensure API communication is configured correctly.
- Confirm there are no runtime errors in the browser console.

## Integration Verification

Verify the complete booking workflow.

- Search hotels.
- Display results.
- Sort by total price.
- Reserve a hotel.
- Validate document rules.
- Display booking confirmation.
- Verify backend communication.

# Expected Output

Provide:

- Build status
- Backend status
- Frontend status
- URLs
- Any warnings
- Any errors
- Recommended fixes, if necessary

Do not modify working functionality unless required to resolve startup issues.

# Validation Checklist

Verify the run result:

- NuGet restore succeeds.
- .NET solution builds successfully.
- Backend starts and exposes the expected API URL.
- Swagger/OpenAPI is reachable.
- Search, reserve, and reservation lookup endpoints are reachable.
- npm dependencies are present or restored.
- Angular production build succeeds.
- Angular dev server starts and exposes the expected frontend URL.
- Browser routing works for search, reservation, and confirmation pages.
- Browser console has no unexpected runtime errors.
- End-to-end booking workflow succeeds with deterministic provider data.
