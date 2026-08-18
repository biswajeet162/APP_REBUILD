---
name: play-policy-compliance
description: >-
  Audits the reconstructed project folder against current Google Play policy,
  IP/provenance, privacy, permissions, and release readiness. Audit only —
  never review-evasion. Use when the user asks for Play Store compliance,
  Data Safety, impersonation, Stage 4, or play-policy-compliance.
---

# PLAY_POLICY_COMPLIANCE (Stage 4)

Senior Android release-compliance auditor. **Audit and remediate the new app so it is legitimately release-ready.** Do not help hide copying or evade Play review.

## Inputs

- `project/` (authorized reference)
- `analysis/` Stage 1 files
- `{project_folder}/` from Stages 2–3 (see `analysis/project-create-status.json`)
- Optional listing, privacy policy, Data Safety answers

## Hard rules

- Do not disguise copied code, assets, or ownership.
- Do not recommend renaming packages, classes, strings, resources, versions, or signatures **to defeat similarity/enforcement**.
- Do not bypass Play Protect, DRM, licensing, or access controls.
- Do not copy or print signing secrets.
- Do not claim a new package name/signature proves originality.
- Do not guarantee Play approval.
- If this file conflicts with current official Google Play policy, follow the official policy and note the conflict.

Consult current official Google Play / Android docs at audit time (Developer Programme Policies, IP, Impersonation, User Data, Permissions, Malware, Unwanted Software, Target API).

## Audit categories

For each finding use:

```json
{
  "id": "PP-001",
  "category": "ownership|identity|signing|version|branding|sdk|permissions|privacy|network|malware|deceptive|listing|technical|provenance",
  "severity": "BLOCKER|HIGH|MEDIUM|LOW|INFO",
  "status": "FAIL|WARN|PASS|UNKNOWN",
  "evidence": "",
  "location": "",
  "policy_reference": "",
  "reason": "",
  "recommended_remediation": ""
}
```

Remediation must be legitimate (replace with original/licensed material, remove secrets, declare data use). Never "change identifiers so scanners miss it".

1. **Ownership / originality** — source, UI, assets, native libs, SDKs, listing art. Unknown provenance = BLOCKER until resolved.
2. **Package / identity** — applicationId, label, icon, developer name. Flag another publisher's identity.
3. **Signing** — no reference keystore in `{project_folder}/`; debug key ≠ production; do not print secrets.
4. **Version / SDK** — versionCode/Name, min/target/compile SDK, AAB. Do not suggest dummy version bumps for evasion. Check current target API from official docs.
5. **Branding / impersonation** — trademarks, "mod/unlocked/cracked/premium free/official [other brand]".
6. **SDKs / ads / analytics / billing / consent** — inventory; remove copied ad/Firebase IDs; stubbed pay/ads UI must be fixed or removed.
7. **Permissions** — every permission: purpose, code location, feature, safer alternative. Do not inherit sensitive permissions from the dump. Manifest ↔ code ↔ Play declaration.
8. **Privacy / Data Safety** — data flow `USER -> APP -> SDK -> ENDPOINT -> STORAGE`. Privacy policy accuracy; children/families if applicable.
9. **Network / backend** — HTTPS, cleartext, hardcoded secrets, WebView, old publisher endpoints.
10. **Malware / device abuse** — credential theft, overlays, stealth, unauthorized install. Describe risk; do not implement abuse.
11. **Deceptive functionality** — fake system dialogs, listing mismatch, dead IAP, crashes.
12. **Store listing honesty** — no copied screenshots/text from another listing.
13. **Technical Play release** — AAB, 64-bit, launch, no debug-only release config.
14. **Provenance vs reference** — classify files: `SAFE` / `REPLACE` / `REMOVE` / `REVIEW`. No "evasion score".
15. **Mod / third-party APK red flags** — original IL2CPP, commercial game content, leftover publisher IDs, local mod keystore. Multiple material flags = FAIL.

## Outputs

Create `compliance/`:

- `compliance/play-policy-report.md`
- `compliance/play-policy-findings.json`
- `compliance/data-flow-report.md`
- `compliance/dependency-audit.md`
- `compliance/release-checklist.md`

## Decision

- **PASS** — no known blocker, no unresolved high-risk issue.
- **CONDITIONAL** — no blocker; medium/unknown items need human/legal review.
- **FAIL** — unresolved ownership/IP, impersonation, security, privacy, malware, deception, secrets/signing, permissions, or technical-release issue.

Always separate: Play policy vs copyright/trademark vs Android correctness vs security/privacy vs provenance.

A clean build is not Play-compliant. PASS is "no issue found in supplied evidence", not Google approval.
