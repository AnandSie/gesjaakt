# 🃏 Bot Building Hackathon

Do you like programming and card games? Then join this hackathon! Build your own bot and let it compete against the bots of other participants. Choose your game, code your strategy, and may the best algorithm win!

---

## 📋 Table of Contents

- [How It Works](#-how-it-works)
- [Getting Started](#-getting-started)
- [General Bot Development](#-general-bot-development)
- [Game 1: Gesjaakt 🃏](#-game-1-gesjaakt)
- [Game 2: Take-5! 🐄](#-game-2-take-5)

---

## ✨ How It Works

A simple **game engine** has been built for each supported game. Competitors develop a bot that plays against others through the engine. The engine tracks the full game state — your bot only needs to decide what action to take each turn based on the current state.

**Supported games:**
- [Gesjaakt](#-game-1-gesjaakt)
- [Take-5!](#-game-2-take-5)

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
7. Open `gesjaakt\Src\Application\GameRunner.cs`
8. Press `Ctrl+F5` to run, then select **"C#"** → **"C#: Console App"**

You should see:
```
Which game do you want to play?
1. GesjaaktGame
2. TakeFiveGame
```

---

## 🛠️ General Bot Development

These rules and tools apply to **both games**.

### Language & Structure

- Bots must be written in **C#**
- Each bot must conform to the `TemplateThinker` of its respective game (see the game-specific sections below)
- Please develop your bot on a **new branch** in the repository

### Adding Your Player to the Game

1. Go to the `PlayerFactory.Create()` method
2. Add your new `Player` with your `Thinker`
3. A `YourThinker.cs` starter file is already provided — just uncomment the line and give your player a name.

### Training & Testing

- Test locally against the **default bots** in the repository at any time
- To test against other competitors' bots during development, contact one of the **organizers**

### 📊 Gain Insight Into Your Algorithm

1. Go to the `Visualizer()` constructor
2. Add your `Thinker` there
3. Run the console and select the **visualize** option

### Logging Configuration

Configure the log level in `Program.cs` to balance detail vs. performance:

| Log Level     | What it shows                                                                 |
|---------------|-------------------------------------------------------------------------------|
| `Debug`       | All internal logs — very detailed; not recommended for large simulations      |
| `Information` | Key game flow: coin counts, all "GESJAAKT!" moments, and round results        |
| `Warning`     | Only critical events and round results                                        |
| `Critical`    | Only the **final aggregated results** — recommended for large simulations     |

> 💡 Using `Warning` or `Critical` makes the app run **faster** by reducing console output.

---

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
5. Run the game

> 📄 Template location: `Src\Application\TakeFive\Thinkers\TemplateTakeFiveThinker.cs`

## 📊 Take-5! Results

*Results will be posted here after the tournament.*
