# Copilot Prompt Log

## Initial Copilot Prompts Used

### Prompt 1: Documentation-Only Design Phase

```text
You are acting as a Senior Software Architect.

The repository already contains a default README.md and .gitignore.

Update the existing README.md and create the following documentation files:

- spec.md
- prompts.md
- reflection.md

Do NOT generate any source code or project files yet.

The purpose is to complete the analysis and design phase before implementation.
```

### Purpose

Establish the repository as an analysis-first project and prevent premature scaffolding. This prompt defines the expected documentation deliverables and sets the boundary that no application source code should be generated yet.

## Purpose of Each Prompt

| Prompt | Purpose | Expected Output |
| --- | --- | --- |
| Documentation-only design phase | Create project planning documentation before implementation | Updated `README.md`, new `spec.md`, new `prompts.md`, headings-only `reflection.md` |
| Senior Software Architect role | Encourage architecture-level decisions and explicit contracts | Requirements, API contracts, provider boundaries, validation rules, assumptions, extension points |
| No source code instruction | Keep the repository in analysis mode | Documentation files only, with no backend or frontend scaffold |

## Notes for Future Prompts

- Ask Copilot to implement one vertical slice at a time after the specification is reviewed.
- Keep future prompts explicit about whether source code generation is allowed.
- Reference `spec.md` when generating backend contracts, validation logic, and provider interfaces.
- Reference `prompts.md` when documenting AI-assisted development decisions.
- Ask for tests alongside implementation once scaffolding begins.
- Use separate prompts for backend, frontend, test strategy, and architecture review.
- Request review prompts before accepting larger generated changes.
- Update this file whenever a meaningful implementation or review prompt influences project direction.
