# Decisions Log (One-Liners)

Purpose: Capture assumptions and defaults when proceeding quickly in Early-Stage Rapid Mode.

Template (copy one line per decision):
- [DATE: YYYY-MM-DD hh:mm] [Context]: [Question/Decision]. Options: [A|B|C]. Default chosen: [X]. Reason: [short]. Owner: [name/role]. Follow-up: [if any].

Examples:
- 2025-08-19 17:25 UI Navigation: Choose stack-based push/pop over replace for first stub. Options: [Push/Pop|Replace|Hybrid]. Default chosen: Push/Pop. Reason: simplest to demo. Owner: Client/UI. Follow-up: Revisit after first user test.
- 2025-08-19 17:27 Networking Contract: Use V1 DTOs without version negotiation. Options: [V1 no-negotiation|V1 + negotiation|Protobuf alt]. Default chosen: V1 no-negotiation. Reason: fastest path. Owner: Shared/Server. Follow-up: Add negotiation once endpoints stabilize.

Notes:
- Keep entries short; prefer defaults and proceed if no response within 30 minutes (per plan.md 0a.4).
- If a decision is reverted, add a new line noting the rollback and why.
