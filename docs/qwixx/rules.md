# Qwixx — Rules Specification

This document is the source of truth for the Qwixx base game rules used by this repo. It covers the **base game only** (2–5 players) — expansions/variants (e.g. Qwixx Wild & Extreme) are out of scope and not referenced here.

Every atomic, testable rule has a stable ID of the form `QX-NNN`. IDs are assigned once, in sequence, and are **append-only**: once published, an ID's number is never reassigned or reordered, even if a later rule is added out of order. Future unit tests should reference these IDs (e.g. in a test name or comment) so spec and tests stay traceable to each other.

---

## 1. Components & Setup

- **QX-001**: The game uses 6 dice: 2 white dice and 4 colored dice — one each of red, yellow, green, blue.
- **QX-002**: Each player has their own score sheet with four colored rows: red, yellow, green, blue.
- **QX-003**: The red and yellow rows list numbers in ascending order: 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12.
- **QX-004**: The green and blue rows list numbers in descending order: 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2.
- **QX-005**: Each row has a "lock" cell after its final number, printed with a padlock symbol.
- **QX-006**: The game supports 2 to 5 players.
- **QX-007**: One player is designated to roll the dice each turn; the role of active roller passes to the left after each turn.

## 2. Turn Structure

- **QX-008**: On their turn, the active player rolls all 6 dice simultaneously.
- **QX-009**: After the roll, the active player first sums the two white dice together; **every player** (including the active player) may, optionally, mark that white-dice sum in one row of their own choice on their own sheet.
- **QX-010**: After the white-dice marking step (QX-009) has been resolved for everyone, the active player computes, for each of the two white dice individually, its sum with each of the four colored dice — up to **8 candidate sums** (2 whites × 4 colors: white_a+red, white_b+red, white_a+yellow, white_b+yellow, white_a+green, white_b+green, white_a+blue, white_b+blue). **Only the active player** may mark one resulting number, in the row matching that sum's color, on their own sheet. This step is always resolved strictly after QX-009, never before or in parallel with it.
- **QX-011**: Marking any number is always optional ("may"), never mandatory — a player may choose to mark nothing available to them on a given turn.
- **QX-012**: A player may mark at most one number per row per turn, regardless of how many combinations would have allowed marking in that row.
- **QX-013**: If the active player marks nothing at all on their turn (neither the white-sum option nor a colored-sum option), they must take a penalty (see QX-018).
- **QX-014**: If a non-active player marks nothing using the white-dice sum on a given turn, they take no penalty — penalties for a missed mark apply to the active player only for that turn.

## 3. Marking Numbers in a Row

- **QX-015**: Within a row, numbers must be marked in the row's printed left-to-right order (ascending for red/yellow, descending for green/blue).
- **QX-016**: Once a number in a row has been marked, no number to its left (i.e., earlier in that row's order) may be marked afterward, even if it becomes available via a later dice combination.
- **QX-017**: A player may skip over available numbers in a row (mark a later number without having marked earlier ones) — skipped numbers become permanently unavailable in that row for that player.

## 4. Penalties

- **QX-018**: A penalty is recorded by marking one of the four penalty boxes on the score sheet.
- **QX-019**: Each penalty box marked is worth −5 points at scoring time (see QX-030).
- **QX-020**: Once all four penalty boxes are marked, the game ends immediately (see QX-026).

## 5. Locking a Row

- **QX-021**: A row's last number (12 for ascending rows, 2 for descending rows) has no special marking requirement beyond the normal sequential rule (QX-015/QX-016) — it does not need 5 prior marks to be reached.
- **QX-022**: The lock cell of a row may only be marked in the same turn a player marks that row's last number, **and** only if doing so brings that player's total marks in the row (including the last number just marked) to **at least 5**.
- **QX-023**: A player may mark a row's last number without marking the lock cell in that same turn (e.g., they have fewer than 5 total marks in the row, or simply choose not to lock). In that case the lock cell can never be marked afterward for that row, since no further numbers remain to mark there.
- **QX-024**: Once **any** player marks a row's lock cell, that row becomes locked **for all players**, not only for the one who locked it: from that point on, nobody may mark any further numbers in that color's row for the rest of the game.
- **QX-025**: Locking is global because the dice are shared: physically, the locked row's colored die is removed from the dice pool, so nobody can produce a sum for that color again. An implementation does not need to literally remove a die from a pool — directly enforcing QX-024 ("no marks in a locked row, for anyone") has the same effect and is simpler for an engine/bot to reason about.

## 6. End of Game

- **QX-026**: The game ends immediately when either (a) two or more rows have been locked (by any player(s), not necessarily the same player), or (b) any player has marked all four penalty boxes.
- **QX-027**: When the end-of-game condition is triggered mid-round, the current round is completed (all players finish taking their turn for that roll) before final scoring.

## 7. Scoring

- **QX-028**: Each row's score is determined by the count of marks in that row, using the triangular sequence: 1 mark = 1 point, 2 = 3, 3 = 6, 4 = 10, 5 = 15, 6 = 21, 7 = 28, 8 = 36, 9 = 45, 10 = 55, 11 = 66, 12 marks (row fully marked, including the lock cell counted as one of the 12) = 78.
- **QX-029**: A row with zero marks scores 0 points for that row.
- **QX-030**: Each marked penalty box subtracts 5 points from the player's total (see QX-019).
- **QX-031**: A player's final score is the sum of all four row scores minus the total penalty deduction.
- **QX-032**: The player with the highest final score wins.
- **QX-033**: If two or more players tie for the highest final score, all tied players are recorded as winners.

---

## Verification log

Reviewed against the physical rulebook on 2026-07-22. Outcome of each item raised during drafting:

1. **QX-021 through QX-025 (locking)** — *corrected*. The last number of a row (2 or 12) has no special gate; only the **lock cell** requires ≥5 total marks in the row, marked the same turn as the last number. Locking is a **global** effect (all players lose access to that color's row, not just the locker) — modeled after the shared dice pool losing that color's die, though an engine can enforce this directly via "no marks in a locked row" without literally simulating die removal.
2. **QX-013 / QX-014 (who takes the penalty)** — *confirmed as drafted*. Only the active player is penalized for marking nothing; non-active players are never penalized.
3. **QX-026 (end condition)** — *confirmed as drafted*. Game ends on 2 rows locked (by anyone) OR any player reaching 4 penalties.
4. **QX-009 / QX-010 (order of white vs. colored marking, and dice combinations)** — *corrected*. Order is confirmed fixed (white-sum step for all players always resolves before the active player's colored step). Also corrected: the active player has **8** candidate sums to choose from (2 white dice × 4 colors), not 4 — see updated QX-010.

---

## Appendix: Original Source

Official rulebook (publisher: Gamewright): **[Qwixx — Rules of Play](https://gamewright.com/pdfs/Rules/QwixxTM-RULES.pdf)** (PDF, hosted on gamewright.com).

> Note: this appendix intentionally contains a **link only**, not a copy of the rulebook text — the rulebook is copyrighted by the publisher, so it isn't reproduced here. When verifying or extending the `QX-###` rules above, open the PDF directly and compare section-by-section; if a discrepancy is found, correct the relevant `QX-###` entry in this file (in place, per the append-only ID policy) rather than pasting rulebook text into this document.
