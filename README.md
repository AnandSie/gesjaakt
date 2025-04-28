# 🃏 Gesjaakt Hackathon

Do you like programming and card games? Then join this hackathon! You will build your own bot and let it compete against the bots of the other participants in the card game **“Gesjaakt”**.

Gesjaakt is a card game with simple rules, but with a large variety of possible strategies. Take care not to spend your coins too quickly, because then you are *Gesjaakt*!

## ✨ How It Works 

For this hackathon, we have made a simple **Gesjaakt engine**. Competitors develop a bot, which can play against others through the engine. The engine keeps track of the game state, so the only thing your bot has to decide on its turn is:  
**Do I take the card, or play a coin?**

Due to the engine setup:
- Bots must be developed in **C#**.
- Bots must conform to the `IThinker` interface and implement a `Decide()` method.
- For development, please create a **new branch** on the repository where you can push changes to your own bot.

## 🏆 Tournament Overview

The tournament consists of three rounds, each slightly different from the prior.  
Every round consists of:
- **45 minutes** of programming and testing your bot.
- Followed by **15 minutes** where the bots clash.

Competitors can tweak and improve their bots based on the results after each round.

Each round:
- Bots will play **10000 games** of Gesjaakt back-to-back.
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


## 🛠️ Training and Testing 

The repository for the **Gesjaakt engine** will be available to contestants throughout every round, allowing players to run it locally and test their bots.

- Contestants can always test against the **default bots** in the repository.
- To let bots play against one another during the development phase, please consult one of the **organizers**.

## ⚙️ Logging Configuration

You can configure the **log level** in `Program.cs`:

| LogLevel        | What it shows                                                                                                  |
|-----------------|----------------------------------------------------------------------------------------------------------------|
| `Debug`         | All internal logs (very detailed; not recommended for many simulations).                                       |
| `Information`   | Key game flow logs: how many coins are on the table, all "GESJAAKT!" moments, and round results.                |
| `Warning`       | Only "GESJAAKT!" events and round results.                                                                     |
| `Critical`      | Only the **final aggregated results** after all simulations (recommended for large simulations).               |

> **Note:**  
> Using a more severe log level (`Warning`, `Critical`) makes the application run **faster** by reducing console output.

---

## 🧠 Adding a New Bot (Thinker)

If you want to create your own bot/AI player:
1. Add a new `Thinker` class under:  
   `Core.Domain.Entities.Thinkers`
2. Implement the `IThinker` interface.

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

---

## 👥 Adding Your Player to the Game

To add your player:
1. Go to the `PlayerFactory.Create() method`
2. Add your new `Player` with your `Thinker` there.

✅ The game supports **3 to 7 players**.

---
