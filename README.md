# 🃏 Bot Building Hackathon

Do you like programming and card games? Then join this hackathon! You will build your own bot and let it compete against the bots of the other participants. Choose your game, code your strategy, and may the best algorithm win!

## ✨ How It Works

We have built a simple **game engine** for each supported game. Competitors develop a bot, which plays against others through the engine. The engine keeps track of the game state — the only thing your bot has to decide on each turn is what action to take based on the current state.

The currently implemented games are:
- Gesjaakt
- Take-5!

The parts of the repo relevant for each individual game are explained in their respective section below after a general explanation relevant to both games.

# General Explanation

## Getting started
If you have never run a C# application. Start here! These instructions will get you to running the engine. The instructions are a stripped down/customized version of: https://code.visualstudio.com/docs/csharp/get-started.
1. Download Visual Studio Code: https://code.visualstudio.com/docs/?dv=win64user
2. Install Visual Studio Code
3. Open Visual Studio Code
4. Install the C# Dev Kit extension: https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp
5. After install you should see "Get Started with C# Dev Kit".
   - If this page doesn't show - open the command palette (Ctrl+Shift+P) and select "Welcome: Open Walkthrough".
6. Select "Set up your environment"
7. Select "Install .NET SDK"
8. Install the recommended or any version higher than 8.
9. Open a terminal (Ctrl+`)
10. Clone the repo: `git clone https://github.com/AnandSie/gesjaakt.git`
11. Open the "gesjaakt" folder in Visual Studio Code (Ctrl+K Ctrl+O)
12. Open the "gesjaakt\Src\Application\GameRunner.cs" file
13. Press "Ctrl+F5" to run
14. Select "C#"
15. Select "C#: Console App"
16. You should see:
```
Which game do you want to play?
1. GesjaaktGame
2. TakeFiveGame
```

## Bot Development
Due to the engine setup:
- Bots must be developed in **C#**.
- Bots must be made to conform to the TemplateThinker of their respective game, i.e. `Src\Application\TakeFive\Thinkers\TemplateTakeFiveThinker.cs`
- For development, please create a **new branch** on the repository where you can push changes to your own bot.

## 🛠️ Training and Testing
The repository for the engine will be available to contestants throughout every round, allowing players to run it locally and test their bots.

- Contestants can always test against the **default bots** in the repository.
- To let bots play against one another during the development phase, please consult one of the **organizers**.

## 👥 Adding Your Player to the Game

To add your player:
1. Go to the `PlayerFactory.Create()` method.
2. Add your new `Player` with your `Thinker` there.
3. To give you a head start, we have already added the `YourThinker.cs` — you only have to uncomment the line and give your player a name.

✅ The game supports **3 to 7 players**.

## 📊 Gain Insight Into Your Algorithm

1. Go to the `Visualizer()` constructor.
2. Add your new `Thinker` there.
3. Run the console and select the visualize option.

## ⚙️ Logging Configuration

You can configure the **log level** in `Program.cs`:

| LogLevel        | What it shows                                                                                                  |
|-----------------|----------------------------------------------------------------------------------------------------------------|
| `Debug`         | All internal logs (very detailed; not recommended for many simulations).                                       |
| `Information`   | Key game flow logs: how many coins are on the table, all "GESJAAKT!" moments, and round results.               |
| `Warning`       | Only critical events and round results.                                                                        |
| `Critical`      | Only the **final aggregated results** after all simulations (recommended for large simulations).               |

> **Note:**  
> Using a more severe log level (`Warning`, `Critical`) makes the application run **faster** by reducing console output.


## Gesjaakt




---

# 🎴 Game 1: Gesjaakt

Gesjaakt is a card game with simple rules, but with a large variety of possible strategies. Take care not to spend your coins too quickly, because then you are *Gesjaakt*!

## Rules

On each turn, your bot faces a single decision:  
**Do I take the card, or play a coin?**

The engine handles all game state tracking. Your bot only needs to implement the logic for this choice in the `Decide()` method, which receives an `IGameStateReader` with the full current game state.

## 🏆 Gesjaakt Tournament Overview

The tournament consists of three rounds, each slightly different from the prior.  
Every round consists of:
- **45 minutes** of programming and testing your bot.
- Followed by **15 minutes** where the bots clash.

Competitors can tweak and improve their bots based on the results after each round.

Each round:
- Bots will play **10,000 games** back-to-back.
- Scoring is based on **percentage of wins**.

**First Round:**
- Each bot individually plays against the same lineup of default bots included in the engine.

**Second Round:**
- Bots are placed into pools based on first round performance and divided across three groups.

**Third Round:**
- Bots are again placed into three groups based on second round performance:
  - Top third
  - Middle third
  - Bottom third

<img src="https://github.com/user-attachments/assets/a245c81f-c013-4c45-9e49-ae4a0a0cb3be" width="400"/>

## 🧠 Adding a New Bot (Thinker)

If you want to create your own bot/AI player:
1. Add a new `Thinker` class under:
   `Core.Domain.Entities.Thinkers`
2. Implement the `IThinker` interface.
3. To give you a head start you can reuse the `YourThinker.cs`

Example:
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

## Results

Running all bots against each other resulted in the following:

| Name | Wins | Percentage |
|---|---|---|
| BarryReal | 5,275,674 | 10.5% |
| Mats_R3 | 4,810,460 | 9.5% |
| Ruben | 4,803,417 | 9.5% |
| Jorrit_01 | 4,249,764 | 8.4% |
| Jens_R3 | 3,761,551 | 7.5% |
| Hans_R3 | 3,121,560 | 6.2% |
| Maarten | 3,029,080 | 6.0% |
| Mels | 2,999,765 | 6.0% |
| Nils | 2,679,116 | 5.3% |
| Gerard | 2,419,970 | 4.8% |
| Marijn | 2,236,197 | 4.4% |
| Jessie_R3 | 1,863,957 | 3.7% |
| Jeremy2 | 1,786,397 | 3.5% |
| Anand | 1,747,793 | 3.5% |
| Jose | 1,634,241 | 3.2% |
| Oliver | 1,516,605 | 3.0% |
| Bart | 994,878 | 2.0% |
| ScaredThinker | 990,043 | 2.0% |
| Tomas | 467,532 | 0.9% |

---

# 🐄 Game 2: Take-5!

Take-5! (also known as 6 nimmt!) is a fast-paced card game where the goal is to avoid collecting cards with bull heads. Choose your cards wisely — and hope your opponents don't ruin your plans!

## Rules

Each round, all players simultaneously choose a card from their hand to play. Cards are placed onto one of four rows on the table in ascending order. If your card is placed as the **6th card in a row**, you collect the entire row and score its bull heads as penalty points.

Your bot will need to decide which card to play each turn. The `Decide()` method receives an `IGameStateReader` with:
- Your current hand
- The current state of all four rows
- Any other relevant game state

The bot with the **fewest penalty points** at the end wins.

## 🧠 Adding a New Take-5! Bot (Thinker)
If you want to create your own bot/AI player:
1. Copy the template from: `Src\Application\TakeFive\Thinkers\TemplateTakeFiveThinker.cs`
2. Rename it, i.e. `YourNameTakeFiveThinker.cs`, and put it in the same folder.
3. Implement the two `Decide()` methods
4. Add your thinker to the `TakeFivePlayerFactory.Create` method.
5. Run the game.


## Results
*Results will be posted here after the tournament.*
