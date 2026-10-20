# Changelog

All notable changes to PayPal Flight Recorder are recorded here.

## [Unreleased]

### Added
- ASP.NET Core local investigation dashboard.
- Interactive PayPal sandbox failure investigation.
- Local Qwen2.5 3B AI diagnosis integration.
- Live evidence cards, transaction timeline, and raw trace viewer.
- Read-only replacement-order verification workflow.
- Deterministic payment trace regression tests.

### Verified
- Genuine PayPal sandbox OAuth and Orders v2 integration.
- Controlled GET_ORDER HTTP 404 investigation.
- Local AI inference integrated into the dashboard.
- Replacement order CREATE_ORDER HTTP 201 and GET_ORDER HTTP 200.
- No payment capture in verified investigation workflows.

### Known limitations
- Live investigation currently covers one controlled failure scenario.
- Replacement verification is not repair of the original failed order.
- Live AI text is advisory and not independently verified.
- Public distribution, final security review, and demo materials remain outstanding.
### Full investigation milestone (PG-R03-078 through PG-R03-082)
- Added `--full-investigation` mode to LiveTrace.
- Combined controlled PayPal failure, local AI diagnosis, and replacement-order verification.
- Fixed recovery-event selection when a trace contains both failed and successful GET_ORDER events.
- Connected the dashboard investigation launcher to the new mode.
- Preserved the existing single-purpose investigation modes.
- Dashboard integration compiled successfully; browser acceptance remains pending.
### Dashboard end-to-end investigation
- Activated the full-investigation engine in the running dashboard.
- Verified the dashboard API can orchestrate the complete four-stage workflow.
- Confirmed real PayPal sandbox failure, local AI diagnosis, replacement creation, and retrieval.
- Browser presentation of all four stages remains pending.
### Four-stage dashboard and AI presentation
- Added evidence-driven failure, AI, replacement creation, and retrieval stages.
- Added safe client-side formatting for AI-generated headings, bold text, and lists.
- Preserved raw investigation evidence and advisory AI labeling.
- Verified the updated presentation in the browser on 2026-10-20.
### Verified replacement summary
- Added a recovery summary metric requiring matching replacement-order IDs.
- Displays HTTP 201 to HTTP 200 verification only when required evidence exists.
- Shows the replacement order ID and explicitly states that the original order was not repaired.
- Preserves the no-payment-capture disclosure.
- Browser presentation visually accepted on 2026-10-20.
### Verified release candidate - 2026-10-20

- Published dashboard launched packaged LiveTrace successfully.
- Real PayPal sandbox HTTP 404 failure observed.
- Local Qwen2.5 3B AI diagnosis generated.
- New sandbox order created: HTTP 201.
- Independent retrieval verified: HTTP 200.
- Original failed order not repaired; no payment captured.
- All 47 deterministic Release tests passed.
- Apache License 2.0 installed with user approval.
- Release artifacts excluded from Git.
- Initial credential utility static screening passed.
- Public GitHub distribution and Devpost submission pending.