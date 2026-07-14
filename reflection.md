# Reflection

## Planning Reflections

The strongest decision was to start with documentation and specification before generating source code. That made the provider contracts, endpoint responsibilities, document rules, and offline constraints explicit before implementation. It also made later review easier because the implementation could be compared against a written target instead of a moving idea.

The main planning risk was allowing the implementation to evolve faster than the documentation. During review, README and spec content had drifted from the runtime API shape, especially around `providerCode`, raw search array responses, Swagger, and domestic document behavior. The final cleanup pass corrected those mismatches.

## Architecture Reflections

The solution uses a domain-oriented runtime path: Minimal API endpoints delegate to focused domain services and deterministic provider stubs. Provider-specific source models stay isolated from public DTOs, and clients receive normalized `HotelRoomDto` and `ReservationResponse` contracts.

One important refactoring was removing the duplicate backend layer that existed beside the active `Domain` layer. Keeping two provider/service/DTO stacks made the repository look more complex than it really was and weakened maintainability. After cleanup, dependency injection points to one backend path, and tests target the runtime behavior directly.

Provider normalization now uses `IProviderRoomNormalizer` strategies. `HotelSearchService` orchestrates providers and delegates provider-specific mapping to normalizers, so adding a third provider is closer to an additive change.

## Implementation Reflections

Minimal API worked well for the small HTTP surface. Typed results made status-code behavior explicit, and records kept request/response contracts compact. Swagger/OpenAPI was added as an operability improvement even though it was not required by the brief.

The frontend benefited from Angular standalone components, signals, and Reactive Forms. Search, reservation, and confirmation are separated into page components, while reusable pieces such as search bar, hotel cards, provider badges, booking summary, and confirmation card keep the UI easier to reason about.

The reservation workflow was tightened after review. Initially, the frontend validated guest and document fields, while the backend focused mostly on dates and document mismatch. The backend now also rejects missing provider, hotel, room, guest, document number, and invalid price fields with HTTP 400, which makes the API safer and less dependent on the UI.

## Testing Reflections

The backend test suite now includes both domain-level tests and Minimal API integration tests through `WebApplicationFactory<Program>`. The integration tests are valuable because they validate the actual route binding, JSON enum behavior, status codes, validation response shape, reservation creation, and lookup flow.

Angular unit tests cover the services, store, pages, and shared components. The Playwright suite adds browser-level checks for the most important user journey plus invalid date and international document mismatch paths.

The Playwright setup exposed a realistic integration issue: the test originally used `127.0.0.1:4200`, while the API CORS policy allowed `http://localhost:4200`. The fix was to align Playwright's base URL with the configured local development origin. This was a useful reminder that e2e tests catch environment assumptions that unit tests cannot.

## AI Tooling Reflections

GitHub Copilot Enterprise Agent Mode was useful for moving across backend, frontend, tests, and documentation without losing the case-study constraints. The best prompts were scoped by phase, such as documentation-only, domain-only, provider-only, endpoint-only, frontend-only, and test-only. Those constraints reduced accidental rewrites and made each validation step smaller.

Workspace context hooks were important for keeping Copilot grounded. I used `#file` for authoritative anchors such as the case-study PDF and active project files, `#codebase` style repository review prompts for architecture and readiness checks, and `#selection` style scoped prompts when editing focused sections such as prompt history, reflection notes, endpoint handlers, and UI components.

AI was most effective when paired with explicit human judgement:

- I kept the application offline and deterministic even when production-style ideas such as persistence, auth, or external APIs could have appeared attractive.
- I accepted Angular Material/CDK where it helped controls, but kept the application-specific UI composed from local components.
- I chose to add Swagger as a demo and validation aid without changing endpoint behavior.
- I chose to remove duplicate backend layers instead of leaving unused abstractions that only made the repository look more complicated.
- I kept the e2e test to one core workflow rather than creating a brittle browser suite for every component.

AI also needed correction. Some generated documentation described an earlier architecture with mapper and repository abstractions that no longer matched the runtime path. Review and tests were necessary to catch that drift. The final pass aligned docs with code and added integration coverage around the real API surface.

## Trade-Offs

| Decision | Benefit | Cost |
| --- | --- | --- |
| In-memory reservations | Fully offline and simple to run from a clean clone. | Reservations reset when the API restarts. |
| Minimal API endpoints | Small, readable HTTP surface for the challenge. | `Program.cs` can grow if more endpoints are added. |
| Domain provider stubs | Deterministic tests and no external dependencies. | Does not model provider latency, errors, retries, or partial failures. |
| Provider normalizer strategies | Keeps provider-specific mapping additive and easier to test. | Adds a small amount of indirection for a two-provider case study. |
| Focused Playwright coverage | Proves the happy path and key negative paths without heavy maintenance. | Does not replace broader accessibility or responsive browser testing. |

## What Went Well

- The solution runs fully offline.
- Provider-specific contracts are isolated from the public API response.
- Client and server both enforce document rules.
- Backend tests cover domain rules and HTTP endpoint behavior.
- Frontend tests cover service, store, page, and component behavior.
- Playwright verifies the primary user workflow and key negative paths.
- Swagger makes the API easy to inspect during review.
- Prompt history documents how AI was used across the delivery process.

## What I Would Improve Next

- Expand provider normalizer tests if a third provider is added.
- Add more request validators if more endpoint contracts are introduced.
- Add a small accessibility pass with keyboard and screen-reader checks.
- Consider configuration-backed destination/document rules if business rules are expected to change.
- Add structured logging and a health endpoint if this were prepared beyond the case-study scope.

## Final Assessment

The solution is strong for a case-study submission: it demonstrates requirements analysis, clean enough architecture, deterministic offline behavior, meaningful tests, a working UI, and documented AI usage. It is not production-ready in the full enterprise sense because it intentionally avoids durable persistence, authentication, deployment, observability, and broader failure handling. Those omissions are appropriate for the challenge scope, but they should be named clearly during evaluation.