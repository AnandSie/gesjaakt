# Qwixx Implementation — Status Report

_Branch: `feature/qwixx` · Written to checkpoint context before continuing in a fresh session._

## Goal

Add Qwixx as a third game in this hackathon engine, using a spec-driven / TDD workflow instead of hand-coding it directly like Gesjaakt and Take-5!:

1. Capture the rules as a numbered, verified markdown spec (`docs/qwixx/rules.md`, IDs `QX-001`...`QX-033`).
2. Write unit tests first, each referencing the rule ID(s) it covers, against a thin API-shape stub (`throw new NotImplementedException()` everywhere).
3. Implement the actual logic afterward, by hand, using the red tests as the spec to satisfy.

We are at the end of step 2 for the whole class breakdown — no real logic has been implemented yet, by design.

## What exists now

| File | Status | Notes |
|---|---|---|
| `docs/qwixx/rules.md` | Done | 33 numbered rules + a link-only appendix to the official Gamewright PDF (no rulebook text reproduced, it's copyrighted) |
| `QwixxColor` | Done | Enum: Red, Yellow, Green, Blue |
| `QwixxRules` | Done, real values | Constants transcribed from the spec (dice counts, player range, penalty value, lock threshold, triangular score table). Tests are green — they pin the constants against the spec, not TDD stubs |
| `QwixxRow` | Done | Per-color marking/locking/scoring. 47 tests (`QX-003,004,016,017,021-024,028,029`), all green |
| `QwixxDiceRoll` | Done | White sum + per-color candidate sums from a 6-dice roll. 7 tests (`QX-008,009,010`), all green |
| `QwixxMark` | Done (record) | `(QwixxColor Color, int Number)` — a specific colored-mark candidate |
| `QwixxPlayer` / `IQwixxPlayer` / `IQwixxReadOnlyPlayer` / `IQwixxThinker` | Done | Thinker-driven, mirrors `TakeFivePlayer`. 22 tests, all green |
| `QwixxReadOnlyPlayer` | Done (pass-through) | No tests — matches how `TakeFiveReadOnlyPlayer`/`GesjaaktReadOnlyPlayer` are untested boilerplate in this repo |
| `QwixxGameState` / `IQwixxGameState` / `IQwixxReadOnlyGameState` | Done | Turn rotation + game-over check. 12 tests, all green |
| `QwixxReadOnlyGameState` | Done (pass-through) | Same as above |
| `QwixxGameDealer` / `IQwixxGameDealer` | Stub | `Add`/`GetPlayerResults` fully specified by tests; `Play()`'s turn logic deliberately left undesigned. 4 tests |

**Test totals:** 100 Qwixx tests, 96 green (`QwixxRules` pinning + `QwixxRow` + `QwixxDiceRoll` + `QwixxPlayer` + `QwixxGameState`), 4 red (`QwixxGameDealer`, `NotImplementedException` as expected for the remaining stub).

## Key design decisions made along the way

- **Player delegates to a Thinker, same as the other two games.** Caught mid-build: my first `QwixxPlayer` draft was just a scoresheet with no way for a bot to act — missing the actual point of the hackathon. Fixed by adding `IQwixxThinker` (`DecideWhiteMark`, `DecideColoredMark`, `DecideToLock`) and making `Player` a pure delegator, matching `GesjaaktPlayer`/`TakeFivePlayer`.
- **Two decision points, not one**, because a Qwixx turn isn't symmetric: `DecideWhiteMark` is offered to *every* player every turn; `DecideColoredMark` only to the active roller. A third, `DecideToLock`, was added later (see below).
- **Locking is a real choice, not automatic.** Re-reading `QX-023` ("...or simply choose not to lock") while comparing `Player` against the other games surfaced that neither decision method asked "do you also want to lock?" — `IQwixxThinker.DecideToLock` closes that gap.
- **`Row.IsLocked` is derived, not stored.** A row has 11 numbers + 1 lock cell = 12 max marks; 12 is only reachable by having also marked the lock. So `IsLocked == (MarkedCount == 12)` — no separate flag that could drift from the mark count.
- **Locking is global game state, not per-row.** `GameState.IsColorLocked(color)` is a separate, genuinely independent fact from any single player's `Row.IsLocked` — "did I lock my row" (scoring-relevant, per player) vs. "is this color dead for everyone" (table-wide). Keeping the two in sync is explicitly **left as the dealer's job**, deferred on purpose (see Open Items).
- **Read-only views expose queries, never mutable objects.** `IQwixxReadOnlyPlayer` exposes `MarkedCount`/`IsRowLocked`/`CanMark` as methods, never the mutable `QwixxRow` itself — so a bot's `Decide()` can't reach in and mutate a row directly, mirroring why `IReadOnlyDeck` exists.
- **Test data should be literal, not re-derived.** Caught and fixed once already: shared test helpers that "compute" ascending/descending sequences from a color were removed in favor of explicit literal numbers per test case, because a bug in that shared helper would have silently validated against itself instead of catching anything.

## Problems found and corrected

1. **Initial rules.md draft was wrong** on the locking mechanic (thought the 2nd-to-last number needed 5 marks; actually only the *lock cell* does, the last number itself has no special gate) and on dice combinations (said 4 candidate sums, actually 8 — 2 white dice × 4 colors). Both corrected after your review against the physical rules, logged in the rules.md "Verification log".
2. **`CanLock` tests only covered one direction.** First pass tested ascending (Red) only; descending rows (Green/Blue) reach their "last number" via a different sequence, so this was a real gap, not just missing coverage — fixed with literal per-color `DataRow` data.
3. **Missing Thinker/Decide architecture in `QwixxPlayer`**, and **missing `DecideToLock`** — both described above.

## Open items / future considerations

- **`QwixxGameDealer.Play()` turn logic is entirely undesigned** — rolling, offering both mark decisions to the right players, validating a thinker's answer against *both* `Row.CanMark`/`CanLock` (local) and `GameState.IsColorLocked` (global) before applying it, handing out penalties (`QX-013`), and rotating turns until `QX-026`. This is the biggest remaining piece, and deliberately left for hand-implementation.
- **Keeping `Row.IsLocked` and `GameState.IsColorLocked` in sync is unenforced by the type system** — whoever writes `Play()` must remember to call `GameState.LockColor(color)` whenever a player's `Row.Lock()` succeeds. We discussed an atomic `TryLockRow(player, color)` alternative and deliberately deferred the decision to when `Play()` is actually being written.
- **No `TemplateQwixxThinker`** yet for hackathon participants to copy (like `TemplateThinker`/`TemplateTakeFiveThinker`).
- **No README section** for Qwixx yet (Gesjaakt and Take-5! each have one).
- **No console/UI event set** on `QwixxGameDealer` (the other two dealers raise `WarningEvent`/`InfoEvent`/`ErrorEvent` for notable moments — e.g. a locked row, a penalty, a thinker error).
- **No `QwixxGameDealer` constructor validation** of player count against `QwixxRules.Min/MaxNumberOfPlayers` — matches the other two games (neither enforces this either), but worth a conscious decision rather than an oversight.
- **`QwixxDiceRoll` has no interface** — unlike `IQwixxPlayer`/`IQwixxGameState`, it's a concrete class only. This means any `GameDealer` test that needs a real dice roll will stay red until `QwixxDiceRoll` is implemented (same bottom-up dependency already accepted for `QwixxPlayer.Score`). Deliberately not retrofitted with an interface to avoid touching several already-committed files for a testing convenience — flagged here in case it's worth revisiting once `Play()` is underway.

## Suggested implementation order

`QwixxRow` → `QwixxDiceRoll` → `QwixxPlayer`/`QwixxGameState` (already largely mock-driven, so these can go green independent of the two above) → `QwixxGameDealer.Add`/`GetPlayerResults` (mock-driven, no dependency) → `QwixxGameDealer.Play()` last, since it's the one place everything meets.
