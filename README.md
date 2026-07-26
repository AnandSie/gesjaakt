# 🃏 Bot Building Hackathon

Do you like programming and games? Then join this hackathon! Build your own bot and let it compete against the bots of other participants. Choose your game, code your strategy, and may the best algorithm win!

---

## 📋 Table of Contents

- [How It Works](#-how-it-works)
- [Getting Started](#-getting-started)
- [General Bot Development](#general-bot-development)
- [Game 1: Gesjaakt 🃏](#-game-1-gesjaakt)
- [Game 2: Take-5! 🐄](#-game-2-take-5)
- [Game 3: Qwixx 🎲](#-game-3-qwixx)

---

## ✨ How It Works

A simple **game engine** has been built for each supported game. Competitors develop a bot that plays against others through the engine. The engine tracks the full game state — your bot only needs to decide what action to take each turn based on the current state.

**Supported games:**
- [Gesjaakt](#-game-1-gesjaakt)
- [Take-5!](#-game-2-take-5)
- [Qwixx](#-game-3-qwixx)

---

## 🚀 Getting Started

> New to C#? Follow these steps to get up and running.
> *(Based on: https://code.visualstudio.com/docs/csharp/get-started)*

1. Download and install [Visual Studio Code](https://code.visualstudio.com/docs/?dv=win64user)
2. Install the [C# Dev Kit extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
3. After installation, you should see **"Get Started with C# Dev Kit"**
   - If not, open the command palette (`Ctrl+Shift+P`) and select **"Welcome: Open Walkthrough"**
4. Select **"Set up your environment"** → **"Install .NET SDK"**
   - Install version 8 or higher
5. Open a terminal (`Ctrl+\``) and clone the repo:
   ```bash
   git clone https://github.com/AnandSie/gesjaakt.git
   ```
6. Open the `gesjaakt` folder in VS Code (`Ctrl+K Ctrl+O`)
7. Open `gesjaakt\Src\Presentation\ConsoleApp\Program.cs`
8. Press `Ctrl+F5` to run, then select **"C#"** → **"C#: Console App"**

You should see:
```
Which game do you want to play?
1. GesjaaktGame
2. TakeFiveGame
3. QwixxGame
```

---

## General Bot Development

These rules and tools apply to **all games**.

### Language & Structure

- Bots must be written in **C#**
- Each bot must conform to the `TemplateThinker` of its respective game (see the game-specific sections below)
- Please develop your bot on a **new branch** in the repository

### Training & Testing

- Test locally against the **default bots** in the repository at any time
- To test against other competitors' bots during development, contact one of the **organizers**

### 📊 Gain Insight Into Your Algorithm

NOTE: This section currently only applies for the Gesjaakt Game, not for the Take Five Game

1. Go to the `Visualizer()` constructor
2. Add your `Thinker` there
3. Run the console and select the **visualize** option

### Game Events

Everything that happens in a game is raised as a **game event** — not a log record. Each event
carries how much it matters (`EventImportance`) and what kind of thing it was (`EventCategory`),
which are two separate questions:

| Importance     | What it covers                                                                    |
|----------------|-----------------------------------------------------------------------------------|
| `Ordinary`     | Routine play: a card is drawn, a coin is paid                                      |
| `Notable`      | Worth pointing out: a row is taken, a penalty is scored                            |
| `Special`      | Rare and consequential: a player is GESJAAKT, a thinker throws                     |
| `GameChanging` | Changes the course of the game or the run: a colour locks, a simulation finishes   |

| Category   | What it means                                                    |
|------------|------------------------------------------------------------------|
| `Play`     | Something happened inside the rules of the game                   |
| `Progress` | The runner moved through a simulation                             |
| `Result`   | A game or a simulation produced a result                          |
| `Fault`    | **Your thinker misbehaved** — threw, or asked for an illegal move |

Each game mode picks its own minimum importance, so you don't have to configure anything: a manual
game narrates everything from `Ordinary` up, a set simulation shows `Special` and above, and
"simulate all combinations" only shows `GameChanging`. Fewer events on screen also means a
**faster** run.

> 💡 Debugging your bot? Every fault is `Special`, so it stays visible in a set simulation even
> though the ordinary narration is filtered out.

### Event Summary

Events are **recorded even when they are filtered out of the display**, so every simulation ends
with a summary over all of them:

```
╭─ Event summary over 1000 game(s) ───────────────────────────────────────────────╮
│              EVENT                          COUNT    /GAME     MEAN    MIN  MAX │
│ ██░░░░░░░░░░ PlayerDecideError                  12     0.01        -      -   - │
│ ████████████ CardDrawnFromDeck               24,000    24.00    19.04   3.00 35 │
│ ███░░░░░░░░░ PlayerGesjaakt                   5,913     5.91    21.37   3.00 35 │
│ ─────────────────────────────────────────────────────────────────────────────── │
│ 41,925 events across 6 kinds                                                    │
╰─────────────────────────────────────────────────────────────────────────────────╯
```

- **COUNT** — how often the event happened across the whole run
- **/GAME** — mean occurrences per game
- **MEAN / MIN / MAX** — over the number the event carries, where it carries one (the value of the
  card taken, the coins on the table, the penalty number). Events with no number show `-`.

Faults sort to the top, so a bot that quietly throws once every few hundred games can't hide.

Only aggregates are kept, not the individual events — a 10.000-game run raises millions of them.

### Events Per Player

Events that happen *to* a specific player are also broken down per bot — this is usually the most
directly useful table for tuning a Thinker:

```
╭─ Events per player (share of each event) ────────────────────────────────────────────────────────╮
│ EVENT                          TOTAL      Bart    Marijn   Maarten     Barry     Anand    Jeremy │
│ SkippedWithCoin               89.594     16,7%     16,7%     16,7%     16,7%     16,6%     16,6% │
│ PlayerGesjaakt                 5.766     46,7%     17,0%     16,2%     15,5%      4,7%         - │
╰──────────────────────────────────────────────────────────────────────────────────────────────────╯
```

Read across a row: everyone pays coins about equally often, but **Bart absorbs 46,7% of every
GESJAAKT in the run** — and Bart finishes last. Jeremy shows `-`, meaning it never happened to
them once.

Cells are a **share of the row**, not a count, because "simulate all combinations" doesn't put
every bot in every game — a raw count would mostly measure who got dealt in most often. Rows sum
to 100%. Events that aren't about anybody (a card leaving the deck) don't appear here.

---


---

# 🐄 Game 2: Take-5!

Take-5! *(also known as 6 nimmt!)* is a fast-paced card game where the goal is to **avoid collecting cards with bull heads**. Choose your cards wisely — and hope your opponents don't ruin your plans!

## Rules

Each round, all players **simultaneously** choose a card from their hand to play. Cards are placed onto one of four rows on the table in ascending order. If your card becomes the **6th card in a row**, you collect the entire row and score its bull heads as **penalty points**.

Your bot decides which card to play each turn. The `Decide()` method receives an `IGameStateReader` containing:
- Your current hand
- The current state of all four rows
- Any other relevant game state

The bot with the **fewest penalty points** at the end wins.

## 🧠 Creating a Take-5! Bot

1. Copy the template. See location below.
2. Rename it to something like `YourNameTakeFiveThinker.cs` and place it in the same folder
3. Implement **both** `Decide()` methods
4. Add your thinker to `TakeFivePlayerFactory.Create`
5. Run the game. (Don't know how? See the [Getting Started](#-getting-started) section)

> 📄 Template location: `Src\Application\TakeFive\Thinkers\TemplateTakeFiveThinker.cs`

## 📊 Take-5! Results

*Results will be posted here after the tournament.*


---

# 🃏 Game 1: Gesjaakt

Gesjaakt is a card game with simple rules but a wide variety of possible strategies. Spend your coins wisely — run out and you're *Gesjaakt*!

## Rules

Each turn, your bot faces one decision:

> **Do I take the card, or play a coin?**

The engine handles all game state tracking. Your bot only needs to implement this logic in the `Decide()` method, which receives an `IGameStateReader` with the full current state.

## 🧠 Creating a Gesjaakt Bot

1. Add a new `Thinker` class under `Core.Domain.Entities.Thinkers`
2. Implement the `IThinker` interface
3. Use the provided `YourThinker.cs` as a starting point

```csharp
public class YourCustomThinker : IThinker
{
    public TurnAction Decide(IGameStateReader gameState)
    {
        if (someLogic)
        {
            return TurnAction.TAKECARD;
        }
        else
        {
            return TurnAction.SKIPWITHCOIN;
        }
    }
}
```

## 🏆 Gesjaakt Tournament Format

The tournament consists of **three rounds**, each slightly different from the last.

Each round includes:
- **45 minutes** of programming and testing
- **15 minutes** of live bot competition

Each round runs **10,000 games** back-to-back. Scoring is based on **percentage of wins**.

| Round | Format |
|-------|--------|
| **Round 1** | Each bot plays against the same lineup of default bots |
| **Round 2** | Bots are divided into pools based on Round 1 performance |
| **Round 3** | Bots are split into top, middle, and bottom thirds based on Round 2 |

<img src="https://github.com/user-attachments/assets/a245c81f-c013-4c45-9e49-ae4a0a0cb3be" width="400"/>

## 📊 Gesjaakt Results

| Name          | Wins       | Percentage |
|---------------|------------|------------|
| BarryReal     | 5,275,674  | 10.5%      |
| Mats_R3       | 4,810,460  | 9.5%       |
| Ruben         | 4,803,417  | 9.5%       |
| Jorrit_01     | 4,249,764  | 8.4%       |
| Jens_R3       | 3,761,551  | 7.5%       |
| Hans_R3       | 3,121,560  | 6.2%       |
| Maarten       | 3,029,080  | 6.0%       |
| Mels          | 2,999,765  | 6.0%       |
| Nils          | 2,679,116  | 5.3%       |
| Gerard        | 2,419,970  | 4.8%       |
| Marijn        | 2,236,197  | 4.4%       |
| Jessie_R3     | 1,863,957  | 3.7%       |
| Jeremy2       | 1,786,397  | 3.5%       |
| Anand         | 1,747,793  | 3.5%       |
| Jose          | 1,634,241  | 3.2%       |
| Oliver        | 1,516,605  | 3.0%       |
| Bart          | 994,878    | 2.0%       |
| ScaredThinker | 990,043    | 2.0%       |
| Tomas         | 467,532    | 0.9%       |

---

# 🎲 Game 3: Qwixx

Qwixx is a fast dice game where every player reacts to every roll. On your turn you roll 6 dice (2 white, 4 colored) and get two chances to cross out numbers on your own score sheet: everyone may use the white-dice sum, and only you (the active roller) may also combine a white die with a colored die. Cross out numbers in a row strictly left-to-right — skip ahead and you lose access to anything earlier. Reach a row's last number with enough marks and you can lock it, removing that color for everyone, for the rest of the game.

Full rule-by-rule spec: [`docs/qwixx/rules.md`](docs/qwixx/rules.md).

## 🧠 Creating a Qwixx Bot

1. Copy the template. See location below.
2. Rename it to something like `YourNameQwixxThinker.cs` and place it in the same folder
3. Implement all three methods: `DecideWhiteMark`, `DecideColoredMark`, `DecideToLock`. The inherited `Me` property is your own score sheet — use it to check what you have already marked before deciding.
4. Add your thinker to `QwixxPlayerFactory.Create`
5. Run the game. (Don't know how? See the [Getting Started](#-getting-started) section)

> 📄 Template location: `Src\Application\Qwixx\Thinkers\TemplateQwixxThinker.cs`

> 💥 **If your thinker throws, the game does not crash.** The exception is caught, logged as an error, and that decision is treated as "mark nothing" — so a broken bot quietly loses points (and collects penalties) instead of ending everyone's game. If your bot seems to be doing nothing, check the log for `Decide Exception - Player <name> could not decide`.

> 🚫 **An illegal mark is rejected, not applied.** Your mark is checked before it lands: the row has to actually allow that number (QX-015/QX-016), the color must not already be locked by someone else (QX-024), and for a colored mark the number has to be one of that roll's real candidate sums (QX-010). A `QwixxColor` you invented by casting an out-of-range number is rejected too. Any of these is logged as `Mark Rejected - player <name> ...` and your turn continues as if you had marked nothing — so if you were the active player, expect a penalty.

## 📊 Qwixx Results

*Results will be posted here after the tournament.*

---