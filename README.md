# Hotel Stay Availability

Hotel Stay Availability is a planned full-stack application for searching hotel room availability across dates, guests, room types, and provider inventory sources. The project is currently in the analysis and design phase; implementation, scaffolding, and source code generation are intentionally deferred until the specification is reviewed.

## Project Overview

The application will expose hotel availability search capabilities through a .NET 8 Minimal API backend and an Angular frontend. The system will model hotels, rooms, rates, stay criteria, availability responses, and provider integrations in a way that supports both local sample data and future external booking or inventory providers.

The repository will also demonstrate an AI-assisted software development workflow using GitHub Copilot Enterprise, Agent Mode, prompt files, and custom instructions. Documentation will capture the intent, prompts, architectural choices, and reflections across the development lifecycle.

## Problem Statement

Travelers need a fast and reliable way to determine whether hotel stays are available for a selected destination, date range, occupancy, and room preference. Hotel operators or platform teams need a backend contract that can validate search criteria, query one or more availability providers, normalize responses, and return predictable results to client applications.

The first implementation phase should focus on a clear vertical slice: accepting validated search criteria, querying a provider abstraction, returning available stay options, and presenting those options in a simple user interface.

## Technology Stack

- Backend: .NET 8 Minimal API
- Frontend: Angular
- Language: C# for backend services, TypeScript for frontend code
- API style: REST over HTTP with JSON payloads
- Validation: Backend request validation with explicit business rules
- Testing: Planned unit, integration, and end-to-end tests
- AI tooling: GitHub Copilot Enterprise, Agent Mode, prompt files, and custom instructions

## Planned Architecture

The planned architecture separates API contracts, application logic, provider integrations, validation, and presentation concerns.

- Angular client submits availability search criteria and displays normalized stay options.
- .NET Minimal API receives requests, validates input, and delegates to application services.
- Availability service coordinates one or more provider implementations.
- Provider interfaces abstract inventory sources such as in-memory sample data, partner APIs, or future booking systems.
- Shared contracts define request and response models exchanged over HTTP.

## Repository Structure (Planned)

```text
hotel-stay-availability/
	README.md
	spec.md
	prompts.md
	reflection.md
	backend/                 # Planned .NET 8 Minimal API project
	frontend/                # Planned Angular application
	docs/                    # Optional future supporting documentation
	tests/                   # Planned test projects and test assets
```

No application source directories have been generated yet.

## Setup

Setup instructions will be added after the implementation scaffold is created.

Planned prerequisites:

- .NET 8 SDK
- Node.js LTS
- Angular CLI or project-local Angular tooling
- GitHub Copilot-enabled development environment

## AI Tooling Approach

The project will use AI assistance deliberately during each phase:

- Analysis: Capture requirements, assumptions, API contracts, data models, and extension points before coding.
- Design: Use prompt files and architectural review prompts to refine boundaries and validate implementation strategy.
- Implementation: Generate focused slices only after the design documents are accepted.
- Review: Use Copilot to inspect code for correctness, test coverage, maintainability, and security considerations.
- Reflection: Record what worked, what changed, and what should improve in future AI-assisted iterations.

See [spec.md](spec.md), [prompts.md](prompts.md), and [reflection.md](reflection.md) for the design-phase documentation.
