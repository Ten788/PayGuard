# PayPal Flight Recorder

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

Development prototype. Not yet packaged or release-ready.

See CHANGELOG.md and docs/RELEASE_CHECKLIST.md.