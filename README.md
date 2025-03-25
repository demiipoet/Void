# ⚔️ Void: A JRPG-Inspired Engine Built from the Ground Up

A turn-based JRPG-style engine written in C#, inspired by the *Final Fantasy* and *Persona* game series.
This project explores core game mechanics like stat-based combat, experience-based leveling, and modular monster generation — with full unit testing to support robust development.

---

## 🎮 Features

- Custom player and monster classes with upgradable stats
- Fully tested EXP gain, leveling, damage formulas, and healing logic
- Turn-based combat with player choices (Attack, Defend, etc.)
- Dynamic monster generation via a `MonsterFactory`
- Combat log for replaying or analyzing battle sequences

---

## 🧰 Tech Stack

- **Language**: C#
- **Testing**: [xUnit](https://xunit.net/)
- **Editor**: VS Code
- **Runtime**: .NET 9+

## 🚀 Getting Started

### 🔧 Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/en-us/download) (v9.0 or higher)
- Git (for cloning the repo)
- A C#-compatible editor (e.g., VS Code)

### 📥 Clone Repo

```bash
git clone https://github.com/demiipoet/Void.git
cd Void
```

### ✅ Testing Coverage
- EXP thresholds and level-up logic
- MaxHP, Strength, Defense scaling
- Combat damage formulas with mitigation
- Edge cases (negative EXP, damage limits, healing caps)
- Monster factory and error handling

### ⚡ Quick Commands
- **Run Game:** `dotnet run --project FirstDraft`
- **Run Unit Tests:** `dotnet test FirstDraft.Tests`

### 👾 Example Output
```csharp
Welcome to Void.

A wild [Monster] Bat appears!
[Player] Zaza's HP: 300/300
[Monster] Bat's HP: 150/150

--- Turn 1 ---
Attack (A), Defend (D), or Heal (H)?
```

---

## 📋 Future Plans
- Magic system
- Summons
- Random encounters
- Items / inventory
- Elemental weaknesses
- Save / load feature
- Buffs and debuffs
- Multiple enemy battles

## 😼 Acknowledgements
- Inspired by classic and modern JRPGs, such as Final Fantasy VI, Persona 5, and Dragon Quest IX

## 📄 License
This project is for portfolio and demonstration purposes only and is not currently licensed for reuse or distribution.
