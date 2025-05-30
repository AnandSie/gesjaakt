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
3. To give you a headstart you can reuse the `YourThinker.cs`

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
3. To give you a headstart, we have already added the `YourThinker.cs`, you only have to uncomment the line and your player it a name

✅ The game supports **3 to 7 players**.

---

## Gain insight into your algorithm

1. Go to the `Visualizer() constructor`
2. Add your new `Thinker` there.
3. Run the console and select the visualize option

---

# Results

Running all bots against eachother resulted in the following

| Name | Wins | Percentage |
| ---|---|---|
| BarryReal | 5275674 wins |  10,5% |
| Mats_R3 | 4810460 wins |  9,5% |
| Ruben | 4803417 wins |  9,5% |
| Jorrit_01 | 4249764 wins |  8,4% |
| Jens_R3 | 3761551 wins |  7,5% |
| Hans_R3 | 3121560 wins |  6,2% |
| Maarten | 3029080 wins |  6,0% |
| Mels | 2999765 wins |  6,0% |
| Nils | 2679116 wins |  5,3% |
| Gerard | 2419970 wins |  4,8% |
| Marijn | 2236197 wins |  4,4% |
| Jessie_R3 | 1863957 wins |  3,7% |
| Jeremy2 | 1786397 wins |  3,5% |
| Anand | 1747793 wins |  3,5% |
| Jose | 1634241 wins |  3,2% |
| Oliver | 1516605 wins |  3,0% |
| Bart | 994878 wins |  2,0% |
| ScaredThinker | 990043 wins | 2,0 % |
| Tomas | 467532 wins |  0,9% |