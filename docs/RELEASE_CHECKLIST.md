# Release Checklist

## Product
- [ ] Finalize release scope and acceptance criteria.
- [ ] Complete interactive investigation workflow.
- [ ] Validate dashboard behavior and error handling.
- [ ] Document supported and unsupported payment operations.

## Quality
- [ ] Run deterministic regression tests.
- [ ] Verify real PayPal sandbox scenarios.
- [ ] Verify local AI integration and fallback behavior.
- [ ] Verify no unintended payment capture.
- [ ] Record final acceptance evidence.

## Distribution
- [ ] Confirm third-party licenses and model distribution rights.
- [ ] Document PayPal sandbox account setup.
- [ ] Document Ollama and model installation.
- [ ] Create release package.
- [ ] Verify installation and startup on a clean environment.
- [ ] Confirm secrets are excluded from release artifacts.

## Hackathon
- [ ] Review official submission and AI-use requirements.
- [ ] Prepare demonstration scenario.
- [ ] Prepare architecture and implementation explanation.
- [ ] Record demonstration video if required.
- [ ] Verify submission artifacts and deadline.

## PG-R04-144 - Packaged Release Verification

- [x] Packaged LiveTrace runtime startup verified (PG-R04-135).
- [x] Packaged dashboard-to-LiveTrace integration verified (PG-R04-136).
- [x] Real PayPal sandbox and local AI packaged investigation verified (PG-R04-137).
- [x] 47 deterministic tests passing (PG-R04-138).
- [x] Generated release artifacts excluded from Git (PG-R04-139).
- [x] Apache License 2.0 installed with user approval (PG-R04-140).
- [x] Credential utility initial static checks passed (PG-R04-142).
- [ ] Complete final security and dependency review.
- [ ] Prepare and verify clean Git release commit.
- [ ] Publish approved public repository.
- [ ] Complete demo/video and submission materials.
- [ ] Confirm official hackathon requirements and compliance.
- [ ] Obtain explicit approval before final Devpost submission.
