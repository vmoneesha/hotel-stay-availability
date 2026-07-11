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

## Prompt Purposes

| Prompt | Purpose | Result |
| --- | --- | --- |
| Documentation-only design phase | Define scope before code | README, spec, prompt log, reflection headings |
| Full solution scaffold | Generate runnable solution | `HotelStay.sln`, `HotelStay.Api`, `HotelStay.Tests`, `hotelstay-ui` |
| Provider architecture requirements | Preserve extensibility | `IHotelProvider`, provider mappers, DI registration |
| Validation and testing requirements | Encode expected behavior | xUnit tests for mapping, price, filtering, date, document, and reference rules |

## Notes for Future Prompts

- Ask for API integration tests against the Minimal API endpoints.
- Ask for Angular component tests or Playwright flows for search, reservation, and confirmation.
- Keep third-provider work additive: new provider, new mapper, DI registration, tests.
- Consider moving destination/document rules into configuration if the list grows.
- Ask for review before adding persistence, authentication, payment, or cloud deployment.
- Keep offline determinism unless a future prompt explicitly changes that constraint.
