# Domivium Improvement Plan

Last updated: 2025-08-19 11:05 (local)

## 0) Executive Summary
This plan is adjusted for the early development phase to optimize for very fast answers and tight iteration loops. The priority is speed over completeness. We will:
- Default to the smallest possible change that unblocks the next question or decision.
- Skip build/CI/test verification for now; defer heavy validation to a later phase.
- Favor stubs/mocks and throwaway scaffolding to rapidly explore interfaces and UX.
- Keep each iteration timeboxed (30–60 minutes) and document decisions briefly.
- Maintain only essential guardrails (e.g., Editor-only code segregation) that do not slow iteration.

When the project matures, we can re-enable the full staged plan (architecture hardening, CI/build checks, and extensive testing).

## 0a) Early-Stage Working Mode: Rapid Answers
Why: Minimize cycle time from question to answer; prioritize learning and momentum.
- E0a.1: Timebox iterations to 30–60 minutes; ship the smallest diff that answers the current question.
- E0a.2: Skip builds/CI/tests unless strictly necessary to answer the question.
- E0a.3: Prefer stubs, interfaces, and mock responses over full implementations.
- E0a.4: Asynchronous Q&A Protocol:
  - Ask one focused question at a time; include current context, options, and a proposed default.
  - If no response in 30 minutes, proceed with the proposed default and note the assumption.
- E0a.5: Decision Log (short): one-liner per decision with context and default picked.
- E0a.6: Small Deltas: Keep changes under ~100 lines when possible; avoid broad refactors.
- E0a.7: Guardrails kept: only UNITY_EDITOR segregation for editor scripts; avoid platform leakage when trivial.
- E0a.8: Rollback: prefer additive changes; when risky, isolate behind flags or partial classes for easy revert.

## 1) Key Goals & Constraints Extracted
- Goals
  - G1: Follow SOLID (SRP, OCP, LSP, ISP, DIP).
  - G2: Use game development domain terms for variables and function names.
- Platforms/Tech Stack
  - Unity 6.1; Platforms: PC, iOS, Android, Nintendo Switch.
  - DB: PostgreSQL.
  - Server: MagicOnion (gRPC over HTTP/2) with MessagePack serialization ecosystem.
  - Client DI: VContainer.
  - World: Tilemap on X/Z; Characters/Props as Plane meshes.
  - UI: MVP with a ViewModel blend; Navigation + UI Manager; New Input System.
- Functional Structure
  - Three projects: Domivium.Client, Domivium.Shared, Domivium.Server.
  - Use asmdef to further split by feature and layer alignment.
- Quality & Testing
  - Editor-related code must never break player builds; always guard/segregate editor code.

## 2) Project & Solution Structure
Why: Enforcing boundaries avoids dependency cycles and platform leakage; supports SOLID and reliable builds.
- Actions
  - A2.1: Ensure the solution has three top-level projects (Client/Shared/Server) with minimal cross-coupling: Client -> Shared; Server -> Shared; no Client <-> Server direct refs.
  - A2.2: In Unity, create asmdefs per feature/layer:
    - Client.Core (domain), Client.UI (presentation), Client.Input, Client.Networking, Client.Tools (Editor-only), Client.Content.X (features like Combat, Inventory).
    - Each Editor-only assembly has includePlatforms set to Editor and/or define constraints UNITY_EDITOR.
  - A2.3: Place shared contracts (DTOs, enums) and pure logic in Domivium.Shared (no UnityEngine types). Serialization via MessagePack annotations.
  - A2.4: Place server services in Domivium.Server, referencing Shared contracts; forbid Unity deps.
- Rationale
  - Clear boundaries (SRP, ISP). Editor code isolation prevents build failures; Shared library decouples transport and engine specifics.

## 3) Dependency Injection with VContainer (Client)
Why: DIP from SOLID encourages explicit composition roots and clear lifetimes.
- Actions
  - A3.1: Establish a single composition root (e.g., ClientBootstrapper) that registers:
    - Lifetimes: Singleton (global services), Scoped (scene/UI scope), Transient (presenter instances).
  - A3.2: Create FeatureInstaller patterns (e.g., UIInstaller, CombatInstaller) to localize registrations per feature/scene.
  - A3.3: Follow constructor injection; avoid ServiceLocator patterns. Prefer interfaces for boundaries.
- Rationale
  - Enforces DIP and ISP; simplifies testing and swapping implementations (OCP).

## 4) Networking with MagicOnion
Why: Robust client-server separation, versioned contracts, and efficient serialization.
- Actions
  - A4.1: Define IGrpcService interfaces and Request/Response DTOs in Domivium.Shared with [MessagePackObject] attributes; keep them engine-agnostic.
  - A4.2: Introduce explicit versioning: namespace or interface version suffixes (e.g., ILoginServiceV1) and MessagePack [Key] indices frozen once published.
  - A4.3: Implement retry/backoff policies client-side where safe; keep gameplay state authoritative on server.
  - A4.4: Add interceptors for logging/metrics (exclude PII), and deadline/cancellation support.
- Rationale
  - Stable wire contracts and backward compatibility (OCP), observability for operations, alignment with performance goals.

## 5) Gameplay Ability System (GAS-like) Foundation
Why: Requirement to mirror Unreal’s GAS in Unity while fitting the tech stack.
- Actions
  - A5.1: Core Concepts in Shared (pure C#):
    - AttributeSet (e.g., Health, Mana, Strength) with change notifications.
    - GameplayTag system (lightweight) for querying/modifiers.
    - GameplayEffect (instant/periodic/duration) with stacking/cooldowns.
    - GameplayAbility (activation policy, cost, cooldown, target data).
  - A5.2: Server Authority in Server project:
    - AbilitySystemComponent server-side processes activations, applies effects, and replicates resulting state deltas to clients.
    - Deterministic resolution where possible; anti-cheat stance: client is non-authoritative.
  - A5.3: Client Integration in Client project:
    - ViewModels react to replicated states; Presenters translate to visuals (animations, VFX) on Plane mesh actors.
    - Prediction window (optional later) for responsive UX; reconcile with server.
  - A5.4: Data-Driven Config
    - ScriptableObjects on client for authoring; export to shared schema (JSON/MessagePack) to feed server on build.
- Rationale
  - Separation of concerns (SRP), extensibility (OCP) by defining base abstractions; mirrors UE GAS concepts familiar to game devs (G2).

## 6) UI Architecture (MVP + VM) and Navigation
Why: Consistent UI structure and tooling reduce defects and manual wiring, particularly across many screens.
- Actions
  - A6.1: Standardize UI layers: View (MonoBehaviour), Presenter (pure C#), ViewModel (state), and Messages/Events.
  - A6.2: UI Navigation & UI Manager
    - Central UIManager handles stack/overlay/modal patterns.
    - Explicit navigation requests (Push/Pop/Replace) via interfaces to keep presenters testable.
  - A6.3: Editor Tooling
    - Enhance existing UI Editor Window & UIMapping Generator to:
      - Validate naming conventions (domain terms per G2) and enforce Capitalize rules.
      - Autogenerate mapping IDs, interfaces, and boilerplate presenters/views.
      - Guard with UNITY_EDITOR; place under Editor assemblies only.
  - A6.4: New Input System Integration
    - Map actions (Navigate, Submit, Cancel, Point) and device-specific bindings.
    - Abstract input in presenters behind IInputService.
- Rationale
  - MVP+VM aligns with SRP/ISP; editor automation reduces human error and speeds up iteration.

## 7) World Representation and Rendering
Why: Meet the constraint of X/Z tilemap floor and Plane mesh actors across platforms.
- Actions
  - A7.1: Tilemap System: define grid coordinates, conversion utilities (grid <-> world), and pathing helpers.
  - A7.2: Plane Mesh Actors: standard prefab with components (SelectionHighlight, ClickHandler, Animator hook).
  - A7.3: Rendering Settings: URP profiles per platform; batching and material instancing plan.
- Rationale
  - Consistent coordinate system utilities remove duplication and bugs; platform-specific URP settings improve performance.

## 8) Multi-Platform Strategy (PC, iOS, Android, Switch)
Why: Proactive abstraction avoids platform-specific regressions.
- Actions
  - A8.1: Platform Abstraction Layer for file I/O, storage paths, haptics, and networking quirks.
  - A8.2: Performance Budgets per platform (target FPS/memory):
    - PC: 60+ FPS; Mobile: 30/60 FPS variant; Switch: 30/60 with handheld/docked budgets.
  - A8.3: Asset management: Addressables profiles per platform.
- Rationale
  - Early constraints drive correct architectural decisions and prevent late surprises.

## 9) Build, CI/CD, and Editor Code Safety
Why: Even in rapid mode, minimal guardrails prevent costly missteps; however, heavy CI/build steps slow iteration.
- Actions (Early Stage)
  - A9.1: Ensure all Editor scripts are inside Editor folders and/or Editor-only asmdefs with includePlatforms: Editor.
  - A9.2: Wrap Editor-only APIs with #if UNITY_EDITOR guards.
  - A9.D: Deferred: CI pipeline builds and static analyzer checks will be reintroduced when exiting rapid mode.
- Rationale
  - Keep the cheapest guardrails (foldering and preprocessor guards) while deferring CI to maintain speed.

## 10) Data & Server Persistence (PostgreSQL)
Why: Clear persistence layer improves reliability and evolvability.
- Actions
  - A10.1: Choose lightweight data access (e.g., Dapper) with explicit SQL for control, or EF Core if productivity is preferred; document choice.
  - A10.2: Migrations with Flyway/Liquibase or EF Migrations; version schema alongside server builds.
  - A10.3: Connection pooling and retry policies; secrets managed via environment variables.
- Rationale
  - Reliable migrations and clear data access patterns reduce defects and ease ops.

## 11) Observability, Logging, and Metrics
Why: Needed for live service stability.
- Actions
  - A11.1: Structured logging (Serilog/NLog) on server; correlation IDs per request.
  - A11.2: Basic gameplay metrics (ability activations, match stats) exported; sampling to control volume.
  - A11.3: Client logs minimal and privacy-safe; opt-in detailed logs via developer build defines.
- Rationale
  - Faster incident response; informs balancing and tuning.

## 12) Testing Strategy
Why: In early rapid mode, testing is minimized to accelerate feedback; structured testing will return later.
- Actions (Early Stage)
  - A12.L: Lightweight ad-hoc checks in-Editor only when needed to answer a question.
  - A12.D: Deferred: Unit/Play/Edit Mode tests and any build verification on CI until we exit rapid mode.
- Rationale
  - Reduce ceremony now; reintroduce formal testing once core flows stabilize.

## 13) Coding Standards & Naming (Game Domain Terminology)
Why: G2 requires domain terms common in game dev to improve clarity.
- Actions
  - A13.1: Adopt naming conventions (e.g., Ability, Effect, Cooldown, AttributeSet, Actor, Controller, Tick).
  - A13.2: Linting with Roslyn analyzers/editorconfig to enforce casing, suffixes (Presenter/View/ViewModel), and forbid ambiguous names.
  - A13.3: PR checklist referencing SOLID and naming.
- Rationale
  - Shared vocabulary improves collaboration; static checks sustain the standard.

## 14) Security & Fair Play (Initial)
Why: Even early prototypes benefit from basic safeguards.
- Actions
  - A14.1: Server authoritative state for combat; validate requests against server state.
  - A14.2: Rate limit risky RPCs; sanitize inputs; avoid trusting client timestamps.
- Rationale
  - Minimizes exploit surface without heavy anti-cheat investment yet.

## 15) Phased Roadmap
- Phase 1 (Foundation)
  - Project/asmdef restructuring (A2), DI setup (A3), Editor safety (A9), naming standards (A13).
- Phase 2 (Core Systems)
  - Networking contracts (A4), GAS core in Shared/Server (A5.1–A5.2), basic client integration (A5.3), UI stack + navigation (A6.1–A6.2).
- Phase 3 (Tooling & Platform)
  - UI tooling enhancements (A6.3), input abstraction (A6.4), world/tilemap utilities (A7), platform abstractions and budgets (A8).
- Phase 4 (Ops & Quality)
  - DB/migrations (A10), observability (A11), CI tests (A12), initial security (A14).

## 16) Immediate Next Steps Checklist (Rapid Mode)
- [ ] Define Q&A SLA: 30–60 minute iteration timebox; proceed with default if no reply in 30 minutes (E0a.4).
- [ ] Create a minimal DECISIONS.md in /docs to log one-liner decisions and assumptions (E0a.5).
- [ ] Pick one UI screen and wire a stubbed flow end-to-end using MVP+VM, skipping builds/tests (E0a.2, A6.1/A6.2).
- [ ] Ensure all Editor scripts are under Editor folders or Editor-only asmdefs; add UNITY_EDITOR guards where applicable (A9.1/A9.2).
- [ ] Create a tiny iteration checklist template for future quick loops (under /docs/templates, optional).

