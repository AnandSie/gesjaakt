# 🃏 Gesjaakt

## ✨ Getting Started

- **Clone the repository**.
- **Create your own branch** to work on:

---

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
1. Go to `PlayerFactory.Create()`
2. Add your new `Player` with your `Thinker` there.

✅ The game supports **3 to 7 players**.

---
