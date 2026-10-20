# PayGuard Flight Recorder

An AI-assisted investigation tool for PayPal sandbox API failures.

## Current capabilities

- Local browser dashboard using ASP.NET Core (.NET 10).
- Real PayPal sandbox API integration.
- Controlled invalid-order investigation.
- Local AI diagnosis using Ollama and Qwen2.5 3B.
- Transaction evidence, HTTP status, and request latency display.
- Separate verified evidence and AI-generated recommendations.

## Development requirements

- Windows with .NET 10 SDK.
- PayPal sandbox application credentials.
- Ollama with the `qwen2.5:3b` model.

## Dashboard

The development dashboard listens on:

http://127.0.0.1:5078

The current workflow uses an existing local encrypted credential
store and a separate LiveTrace executable.

Automated installation and configuration are not yet available.

## Safety and limitations

The verified investigation workflow does not capture payments.
AI-generated explanations are advisory, not authorization for
payment actions. The current replacement-order verification
does not repair the original failed order.

## Release status

Verified local release candidate. Source code is publicly available on GitHub; hackathon submission remains pending.

See CHANGELOG.md and docs/RELEASE_CHECKLIST.md.
## Verified release candidate

The packaged dashboard and LiveTrace completed a real PayPal sandbox
investigation with local Ollama Qwen2.5 3B inference on 2026-10-20.

Verified stages:
- Nonexistent order GET: HTTP 404.
- Local AI advisory diagnosis: GENERATED.
- New $1.00 sandbox order CREATE: HTTP 201.
- Independent retrieval of new order: HTTP 200.

Recovery verification passed. The original failed order was not
repaired. No payment was captured.

## Local reproduction

Requirements: Windows, .NET 10 SDK, PayPal sandbox credentials,
Ollama, and the qwen2.5:3b model.

From the repository root:
- Pull the model: ollama pull qwen2.5:3b
- Configure credentials using tools/Set-PayPalSandboxCredentials.ps1
  with Windows PowerShell in STA mode.
- Build tools/PayGuard.Dashboard/PayGuard.Dashboard.csproj in Release.
- Build tools/PayGuard.LiveTrace/PayGuard.LiveTrace.csproj in Release.

The development dashboard defaults to http://127.0.0.1:5078.
PAYGUARD_DASHBOARD_URL overrides the listening address.
The verified published layout places LiveTrace and its runtime
dependencies in the dashboard's LiveTrace subdirectory.

Automated installation is not provided.

## Release verification and limitations

- CoreTests: 29 passed; TraceTests: 18 passed.
- Packaged dashboard-to-LiveTrace integration: PASS.
- Real sandbox and local AI investigation: PASS.
- Only one controlled live failure scenario has been verified.
- This is a sandbox demonstration, not a production payment service.
- AI explanations are advisory, not independently fact-verified.
- AI output does not authorize payment actions.
- The original failed order is not repaired.
- No payment capture occurs in the verified workflow.
- Comprehensive security review has not been completed.

Licensed under Apache License 2.0. See LICENSE.
Public source repository: https://github.com/Ten788/PayGuard
Devpost submission remains pending.
See docs/TEST_EVIDENCE.md for recorded evidence.