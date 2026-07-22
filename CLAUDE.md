# CLAUDE.md

Guidance for Claude Code when working in this repository.

## What this is

A C# card-game engine used for a "build your own bot" hackathon. Participants write a
*Thinker* (a bot strategy) for one of two games and the engine simulates thousands of
games to rank them.

- **Gesjaakt** (3–7 players) — each turn decide `TAKECARD` or `SKIPWITHCOIN`. Lowest points wins.
- **Take-5! / 6 nimmt!** (2–10 players) — pick a card to play, and pick a row to take when forced. Fewest bull heads wins.

The user-facing docs (rules, hackathon format, results) live in `README.md`. Keep it in sync
when engine behaviour that participants depend on changes.

## Build & test

The solution lives in `Src/`, not the repo root:

```bash
cd Src
dotnet build Gesjaakt.sln
dotnet test  Gesjaakt.sln          # DomainTests (62) + ExtensionsTests (5)
dotnet run --project Presentation/ConsoleApp
dotnet run --project Presentation/ConsoleApp -- --simple-console   # no cursor tricks
```

Target framework is **net8.0** across every project (the installed SDK may be newer; don't
bump `TargetFramework` without asking). The app is interactive: it prompts for game, then
mode (simulate set / simulate all combinations / manual / visualize).

## Architecture

Layered, roughly Clean Architecture. Dependencies point inward; `Domain` references nothing
but `Shared/Extensions`.

```
Src/
  Core/Domain/          Entities + interfaces. Game rules live here (GameState, GameDealer, Player, Deck, Card).
  Core/DomainTests/     MSTest + FluentAssertions + Moq.
  Application/          Orchestration: GameRunner<TPlayer>, per-game Game/PlayerFactory/EventCollector, and all Thinkers.
  Infrastructure/       Logging (MS.Extensions.Logging), UserInterface (ConsoleDisplay), Visualization (ScottPlot).
  Presentation/ConsoleApp/  Composition root: Program.cs + ServiceCollectionExtensions.cs (all DI wiring).
  Shared/Extensions/    EnumerableExtensions (Shuffle) + its tests.
```

Key patterns to follow:

- **Generic runner**: `GameRunner<TPlayer>` is game-agnostic; per-game behaviour comes in via
  `IGame<TPlayer>`, `IPlayerFactory<TPlayer>`. A new game means new `IGame`/`IPlayerFactory`
  implementations plus an `Add<Game>Game` extension in `ServiceCollectionExtensions`, and an
  entry in the `Dictionary<Type, Func<IGameRunner>>` in `AddDynamicBehaviourToChooseGame`.
- **Events, not direct logging**: domain and application classes raise
  `EventHandler<InfoEvent|WarningEvent|ErrorEvent|CriticalEvent>`. `*EventCollector` classes
  subscribe and forward to `IGameEventHandler`, which filters by `EventLevel` and logs.
  Domain code must never take a logger. Log level is raised per mode in `App.SelectGameMode`
  (Critical for big simulations = much faster).
- **Read-only projections**: thinkers only ever see `IGesjaaktReadOnlyGameState` /
  `ITakeFiveReadOnlyGameState`, produced via `AsReadOnly()`. Never hand a mutable state to a bot.
- **Thinker isolation**: a throwing thinker is caught by the dealer and defaults to a safe move
  (`SKIPWITHCOIN`). Keep that guarantee — participant code is untrusted.

## Conventions

- File-scoped namespaces (`.editorconfig`), nullable enabled, implicit usings enabled.
- Namespaces are project-name-based (`Application.Gesjaakt.Thinkers`, `Domain.Entities.Game.TakeFive`),
  not folder-path-exact; match the neighbours in the folder you edit.
- Constructor injection with `private readonly` fields; primary constructors are used in a few
  newer files (`ThinkerPlotter`, `GameRunnerEventCollector`) — both are acceptable.
- `// REFACTOR - ...` comments mark known debt deliberately left in. Don't silently "fix"
  unrelated ones while doing other work; mention them instead.
- Tests: MSTest `[TestClass]/[TestMethod]`, `[TestInitialize]` for setup, FluentAssertions
  (`result.Should().Be(...)`), Moq for interfaces. Naming is `Method_Scenario_Expectation`-ish.

## Adding a Thinker

**Gesjaakt** — copy `Src/Application/Gesjaakt/Thinkers/TemplateThinker.cs`, implement
`IGesjaaktThinker.Decide(IGesjaaktReadOnlyGameState)`, then register in
`GesjaaktPlayerFactory.Create()` (the ≤7 players that play a normal simulation) **and**
`AllPlayerFactories()` (the pool used by "simulate all combinations").

**Take-5!** — copy `Src/Application/TakeFive/Thinkers/TemplateTakeFiveThinker.cs`, derive from
`BaseTakeFiveThinker` (gives you `_hand` via `SetState`), implement both `Decide` overloads and
`Name`, then register in `TakeFivePlayerFactory.Create()` and `AllPlayerFactories()`.

Both factories are hand-maintained lists with lots of commented-out entries — the active roster
differs between `Create()` and `AllPlayerFactories()`, and both differ from the README results
table. Check which list you actually need before editing.

## Known rough edges

Useful context before extending; not a to-do list.

- ~44 build warnings (mostly CS8618/CS8622 nullability, some dead fields). Build is otherwise clean.
- No CI workflow and no `dotnet format`/analyzer enforcement.
- No tests above the Domain layer: `GameRunner`, the factories, and the thinkers are untested.
- Simulation is single-threaded (`// REFACTOR - parallel`). `EnumerableExtensions.Shuffle` uses a
  shared static `Random` and `OrderBy(_ => random.Next())` — not thread-safe, so parallelising
  needs that fixed first (and it's an O(n log n) shuffle with collision bias).
- `GesjaaktReadOnlyGameState` allocates a new `ReadOnlyDeck` and re-projects `Players` on every
  property access — hot path during 10k-game runs.
- Games and thinkers are unseeded, so runs are not reproducible. Seed injection would help testing.
- Hard-coded rule constants (deck 3–35, remove 9 cards, coin counts) sit inline in
  `GesjaaktGameState`/`GesjaaktGameDealer`; Take-5 has a `TakeFiveRules` class — Gesjaakt has no equivalent.
- `GesjaaktGame` declares `public static string Name = "Gejaakt"` (typo) which shadows the
  inherited instance `Name`; the menu actually shows the type name from `GameOption`.
- `GesjaaktVisualizer` hard-codes which thinker it plots (`AnandThinker`) in its constructor.
- Visualization (ScottPlot) exists only for Gesjaakt.
