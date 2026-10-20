# PayGuard Flight Recorder — Completed Hackathon Demo

## Video

Local file: artifacts/demo-video/PayGuard-final-silent-demo.mp4

Format: silent, 87.75 seconds.

Status: rendered, decoded successfully, and reviewed locally.
Public video hosting: pending.

## Demonstrated evidence

The project has separately verified a real PayPal sandbox
investigation:

- Nonexistent order GET: HTTP 404.
- Local Ollama Qwen2.5 3B advisory diagnosis.
- Replacement sandbox order CREATE: HTTP 201.
- Independent replacement-order GET: HTTP 200.
- Original failed order repaired: NO.
- Payment captured: NO.

The video includes accelerated visual replay of recorded
automated tests and a paced replay of real production
authorization-engine mock results.

## Automated tests

- CoreTests: 29 passed, 0 failed.
- TraceTests: 18 passed, 0 failed.
- Total: 47 passed, 0 failed.

## Local mock authorization scenarios

The mock runner references the production PayGuard.Core
project. It made no PayPal API requests.

- Amount substitution: BLOCKED / AMOUNT_MISMATCH.
- Merchant substitution: BLOCKED / MERCHANT_MISMATCH.
- Currency substitution: BLOCKED / CURRENCY_MISMATCH.
- Original authorized payment: ALLOWED / AUTHORIZED_ONCE.
- Replayed approval: BLOCKED / REPLAY_DENIED.
- Unknown approval: BLOCKED / UNKNOWN_APPROVAL.

All six expected outcomes passed.

Measured engine execution: 11.03 milliseconds.
Total mock process wall time including .NET startup:
2.221 seconds.

The video is an animated presentation of genuine recorded
results, not a live terminal recording. The 11.03 ms figure
is not a PayPal API or AI inference benchmark.

## Limitations

- The video does not demonstrate a production payment capture.
- The mock authorization scenarios are local, not PayPal API calls.
- AI diagnosis is advisory.
- The original failed PayPal order was not repaired.
- Only one controlled real PayPal failure scenario was verified.
- Public hosting and Devpost submission remain pending.

## Submission safety

Never expose sandbox credentials or OAuth tokens.
Do not claim production payment readiness.
Do not submit without explicit user approval.
