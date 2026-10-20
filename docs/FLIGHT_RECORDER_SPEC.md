# PayPal Flight Recorder - Draft Specification

Status: DRAFT - NOT APPROVED FOR PRODUCT IMPLEMENTATION
Milestone: PG-R03
Governance: VERGE v1.8.0
Baseline: 94c08d4

## Objective

Build a competition-quality AI-powered payment investigation
workbench demonstrating real PayPal sandbox transactions,
controlled fault reproduction, evidence-grounded AI diagnosis,
safe recovery, and independently verified outcomes.

## Intended user

Developers investigating payment integration failures.

## Required outcomes (proposed)

FR-001: Execute genuine PayPal sandbox payment operations.
FR-002: Capture sanitized request, response, and state evidence.
FR-003: Reproduce supported payment failures deterministically.
FR-004: Use a genuinely executing AI model for trace diagnosis.
FR-005: Distinguish AI hypotheses from verified root causes.
FR-006: Demonstrate a controlled correction and regression proof.
FR-007: Prevent unintended duplicate payment execution.
FR-008: Provide a polished, understandable investigation UI.
FR-009: Preserve reproducible evidence and negative-path tests.
FR-010: Meet official hackathon submission and IP requirements.

## Feasibility gates

F1: Authenticate and execute supported sandbox API operations.
F2: Run a zero-cost AI model and obtain grounded diagnostic output.
F3: Reproduce a supported failure with observable evidence.
F4: Demonstrate successful recovery without duplicate capture.
F5: Prove a complete end-to-end investigation workflow.
F6: Resolve competition eligibility, licensing, and AI disclosure.

## Architecture constraints (proposed)

- Zero required paid services.
- Local-first development and test execution.
- Real sandbox integration; simulated fixtures clearly labeled.
- AI cannot independently authorize financial operations.
- No credentials, tokens, or private customer data in logs.
- Deterministic core checks and proportionate regression tests.
- Preserve original PayGuard source and baseline history.

## Scope exclusions (initial)

- Production payment processing.
- Handling real customer funds.
- Unapproved live publication or external submission.
- Autonomous modification of third-party production systems.
- Claims of automatic repair without verified evidence.

## No-go conditions

- Sandbox operations cannot be demonstrated.
- AI functionality is merely scripted or misrepresented.
- Payment safety or credential handling is unverified.
- Critical recovery claims lack reproducible tests.
- Submission would violate competition or licensing rules.

## Open decisions

- Exact PayPal APIs and supported failure scenarios.
- AI model and local runtime.
- User interface framework and demo deployment.
- Definition and limits of the repair workflow.
- Final requirements and release acceptance thresholds.

## Approval boundary

Product direction approved by owner on 2026-10-20.
This draft is a proposal, not approval of final requirements,
architecture, implementation scope, or publication.