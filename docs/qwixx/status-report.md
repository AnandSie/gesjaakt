# Qwixx Implementation — Status Report

_Branch: `feature/qwixx` · Written to checkpoint context before continuing in a fresh session._

## Goal

Add Qwixx as a third game in this hackathon engine, using a spec-driven / TDD workflow instead of hand-coding it directly like Gesjaakt and Take-5!:

1. Capture the rules as a numbered, verified markdown spec (`docs/qwixx/rules.md`, IDs `QX-001`...`QX-033`).
2. Write unit tests first, each referencing the rule ID(s) it covers, against a thin API-shape stub (`throw new NotImplementedException()` everywhere).
3. Implement the actual logic afterward, by hand, using the red tests as the spec to satisfy.

All three steps are now done for every class, including `QwixxGameDealer.Play()` — the whole Qwixx test suite is green.

## What exists now

| File | Status | Notes |
|---|---|---|
| `docs/qwixx/rules.md` | Done | 33 numbered rules + a link-only appendix to the official Gamewright PDF (no rulebook text reproduced, it's copyrighted) |
| `QwixxColor` | Done | Enum: Red, Yellow, Green, Blue |
| `QwixxRules` | Done, real values | Constants transcribed from the spec (dice counts, player range, penalty value, lock threshold, triangular score table), plus `MinDieValue`/`MaxDieValue` (1/6, standard six-sided dice — not a numbered `QX-###` rule since it's a physical dice property). Tests are green — they pin the constants against the spec, not TDD stubs |
| `QwixxRow` | Done | Per-color marking/locking/scoring. 47 tests (`QX-003,004,016,017,021-024,028,029`), all green |
| `QwixxDiceRoll` | Done | White sum + per-color candidate sums from a 6-dice roll. 7 tests (`QX-008,009,010`), all green |
| `QwixxDiceRoller` / `IQwixxDiceRoller` | Done | Extracted from `QwixxGameDealer` so the dealer can take rolling as a constructor-injected dependency instead of owning a `Random` inline. Composes 6 `Dice` instances. 7 tests, all green |
| `Dice` (`Domain.Entities.Components`) | Done | Generic, game-agnostic single die: `Dice(int min, int max)` + `Roll()`. Not Qwixx-specific and not under the `Qwixx` namespace — lives alongside `Deck<TCard>`/`Card`/`Coin` as a reusable component any future game can use, following the exact same "caller supplies min/max, component doesn't know about any game's Rules class" shape `Deck<TCard>` already uses. 3 tests, all green |
| `QwixxMark` | Done (record) | `(QwixxColor Color, int Number)` — a specific colored-mark candidate |
| `QwixxPlayer` / `IQwixxPlayer` / `IQwixxReadOnlyPlayer` / `IQwixxThinker` | Done | Thinker-driven, mirrors `TakeFivePlayer`. 22 tests, all green |
| `QwixxReadOnlyPlayer` | Done (pass-through) | No tests — matches how `TakeFiveReadOnlyPlayer`/`GesjaaktReadOnlyPlayer` are untested boilerplate in this repo |
| `QwixxGameState` / `IQwixxGameState` / `IQwixxReadOnlyGameState` | Done | Turn rotation + game-over check. 12 tests, all green |
| `QwixxReadOnlyGameState` | Done (pass-through) | Same as above |
| `QwixxGameDealer` / `IQwixxGameDealer` | Done | `Add`/`GetPlayerResults`/`Play()` all implemented by hand (tests only ever specified `Add`/`GetPlayerResults` plus one "does nothing if already over" case for `Play`). 4 tests, all green |

**Test totals:** 110/110 Qwixx tests green (107 Qwixx-specific + 3 for the game-agnostic `Dice` component, which now lives outside the Qwixx namespace).

Beyond the 4 unit tests, `Play()` was manually smoke-tested (not committed) with two throwaway `IQwixxThinker` implementations — one that greedily marks whatever it can and always locks, one that always declines — to confirm a full game actually terminates (no infinite loop) via both end conditions (2 locked colors, and max penalties). Both completed in well under a second.

## Key design decisions made along the way

- **Player delegates to a Thinker, same as the other two games.** Caught mid-build: my first `QwixxPlayer` draft was just a scoresheet with no way for a bot to act — missing the actual point of the hackathon. Fixed by adding `IQwixxThinker` (`DecideWhiteMark`, `DecideColoredMark`, `DecideToLock`) and making `Player` a pure delegator, matching `GesjaaktPlayer`/`TakeFivePlayer`.
- **Two decision points, not one**, because a Qwixx turn isn't symmetric: `DecideWhiteMark` is offered to *every* player every turn; `DecideColoredMark` only to the active roller. A third, `DecideToLock`, was added later (see below).
- **Locking is a real choice, not automatic.** Re-reading `QX-023` ("...or simply choose not to lock") while comparing `Player` against the other games surfaced that neither decision method asked "do you also want to lock?" — `IQwixxThinker.DecideToLock` closes that gap.
- **`Row.IsLocked` is derived, not stored.** A row has 11 numbers + 1 lock cell = 12 max marks; 12 is only reachable by having also marked the lock. So `IsLocked == (MarkedCount == 12)` — no separate flag that could drift from the mark count.
- **Locking is global game state, not per-row.** `GameState.IsColorLocked(color)` is a separate, genuinely independent fact from any single player's `Row.IsLocked` — "did I lock my row" (scoring-relevant, per player) vs. "is this color dead for everyone" (table-wide). Keeping the two in sync is explicitly **left as the dealer's job**, deferred on purpose (see Open Items).
- **Read-only views expose queries, never mutable objects.** `IQwixxReadOnlyPlayer` exposes `MarkedCount`/`IsRowLocked`/`CanMark` as methods, never the mutable `QwixxRow` itself — so a bot's `Decide()` can't reach in and mutate a row directly, mirroring why `IReadOnlyDeck` exists.
- **Test data should be literal, not re-derived.** Caught and fixed once already: shared test helpers that "compute" ascending/descending sequences from a color were removed in favor of explicit literal numbers per test case, because a bug in that shared helper would have silently validated against itself instead of catching anything.
- **`Play()` runs whole rounds, checking `IsGameOver` only between them**, not after every individual turn — this directly implements `QX-027` (finish the current round before stopping) without extra bookkeeping: the loop is `while (!IsGameOver) { for each player: PlayTurn(); NextPlayer(); }`.
- **`GameDealer.TryMark` is the single place that keeps `Row.IsLocked` and `GameState.IsColorLocked` in sync** — every mark attempt goes through it, and it calls `GameState.LockColor(color)` right after a successful `Row.Lock()`. This resolved the sync concern raised as an open item earlier, without needing the atomic `TryLockRow` alternative that was considered.
- **A thinker's colored-mark choice is validated against the roll's actual candidate sums**, not just against `Row.CanMark`. `IQwixxThinker` implementations are hackathon-participant code — an untrusted boundary — and `Row.CanMark` alone can't detect a fabricated number (e.g. claiming a sum the dice never produced) since it only checks ordering/lock state. `QwixxDiceRoll.ColoredSums(color)` already computes the exact legal set, so `OfferColoredMark` checks membership before marking.
- **`OfferWhiteMarkToAllPlayers` uses a single loop over all players, calling the single-player `OfferWhiteMark` exactly once per player.** An earlier version conflated "did this mark succeed" with "is this the active player" in one `&&`-joined `if`; a later attempt to fix that split the active player out into a separate call site entirely, which then had two places calling `OfferWhiteMark`. Settled on: one loop, one call site, and the active player's outcome is just recorded via a plain identity check inside that same loop — natural player-list order is preserved, and no marking logic is duplicated or reordered.
- **`OfferWhiteMarkToAllPlayers`/`OfferWhiteMark`/`OfferColoredMark` are all `void`.** House naming convention: a verb-named method (`Offer...`) is fire-and-forget, a noun-named one is a getter. `TryMark` keeps its `bool` return despite being a verb, since the `Try...` prefix is an established .NET idiom (`TryParse`-style) distinct from a plain fire-and-forget verb. The `QX-013` penalty check needed to know "did the active player mark *anything* this turn," which is answered by `GetMarkedCount(player)` — a plain total across all 4 rows, called before and after both offer steps — kept outside the offer methods entirely rather than threaded through their return values.
- **Dice rolling was extracted from `QwixxGameDealer` into `QwixxDiceRoller`/`IQwixxDiceRoller`**, injected via the dealer's constructor, with `QwixxRules.MinDieValue`/`MaxDieValue` replacing the `1`/`7` magic numbers that were passed to `Random.Next`. This both gives the roller its own focused test coverage and resolves the "can't seed/replay `Play()`'s dice sequence in a test" gap noted below (the dealer can now be constructed with a fake `IQwixxDiceRoller` in a test, though no test yet exploits this for deterministic `Play()` coverage).
- **A single die's randomness was pulled one level further out, into a generic `Dice` class in `Domain.Entities.Components`** — game-agnostic, not under the `Qwixx` namespace, not referencing `QwixxRules` at all. `Dice(int min, int max)` takes its range from the caller, exactly like `Deck<TCard>(int min, int max, ...)` already does — `QwixxDiceRoller` now just constructs `new Dice(QwixxRules.MinDieValue, QwixxRules.MaxDieValue)` and calls `.Roll()` six times, the same way `TakeFiveDeckFactory` builds its `Deck` from `TakeFiveRules.MinCardValue`/`MaxCardValue`. Reusable by any future game without modification.

## Problems found and corrected

1. **Initial rules.md draft was wrong** on the locking mechanic (thought the 2nd-to-last number needed 5 marks; actually only the *lock cell* does, the last number itself has no special gate) and on dice combinations (said 4 candidate sums, actually 8 — 2 white dice × 4 colors). Both corrected after your review against the physical rules, logged in the rules.md "Verification log".
2. **`CanLock` tests only covered one direction.** First pass tested ascending (Red) only; descending rows (Green/Blue) reach their "last number" via a different sequence, so this was a real gap, not just missing coverage — fixed with literal per-color `DataRow` data.
3. **Missing Thinker/Decide architecture in `QwixxPlayer`**, and **missing `DecideToLock`** — both described above.
4. **`QX-012` was fabricated during initial drafting and was wrong.** It claimed a player may mark at most one number per row per turn. The real rulebook (re-checked directly, 2026-07-23, prompted by a question about whether the active player can really use both dice actions) has no such restriction — the white-sum step and the active player's colored-combo step are each independently limited to one row per action, but nothing stops both from targeting the *same* row in one turn (a known legitimate tactic: e.g. white sum marks red 5, colored combo marks red 8 in the same turn). `rules.md` QX-012 was corrected in place (see its Verification log), and `QwixxGameDealer` had a same-row exclusion check built specifically to enforce the wrong rule — removed along with the per-color tracking machinery (`GetMarkedCountsByColor`/`ColorWithMarkedCountAbove`) that existed only to support it, which also substantially simplified `PlayTurn`. Verified via a deterministic smoke test (fixed dice roll, single player) that a mark via white *and* a mark via colored now both land in the same row in one turn as expected.

## Open items / future considerations

- **No unit tests cover `Play()`'s actual turn logic** — only "does nothing if already over" is tested; the roll/mark/lock/penalty/rotation behavior described above is verified solely by the manual smoke test (see Test totals), not by anything in the committed suite. Now that dice rolling is injected (`IQwixxDiceRoller`), a fake roller returning fixed `QwixxDiceRoll`s plus mocked thinkers could give this deterministic coverage — worth doing as a follow-up rather than continuing to rely on the throwaway smoke test.
- **No `TemplateQwixxThinker`** yet for hackathon participants to copy (like `TemplateThinker`/`TemplateTakeFiveThinker`).
- **No README section** for Qwixx yet (Gesjaakt and Take-5! each have one).
- **No console/UI event set** on `QwixxGameDealer` (the other two dealers raise `WarningEvent`/`InfoEvent`/`ErrorEvent` for notable moments — e.g. a locked row, a penalty, a thinker error). `Play()` does not currently raise anything, nor does it catch/report thinker exceptions the way `TakeFivePlayer.Decide`/`GesjaaktGameDealer.PlayerChoice` do.
- **No `QwixxGameDealer` constructor validation** of player count against `QwixxRules.Min/MaxNumberOfPlayers` — matches the other two games (neither enforces this either), but worth a conscious decision rather than an oversight.
- **`QwixxDiceRoll` has no interface** — unlike `IQwixxPlayer`/`IQwixxGameState`, it's a concrete class only. This was a non-issue in practice: `QwixxGameDealer` just constructs one directly each turn.

## Implementation order used

`QwixxRow` → `QwixxDiceRoll` → `QwixxPlayer`/`QwixxGameState` (already largely mock-driven, so these went green independent of the two above) → `QwixxGameDealer.Add`/`GetPlayerResults` → `QwixxGameDealer.Play()` last, since it's the one place everything meets.
