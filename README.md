# Void: A JRPG-Inspired Engine Built from the Ground Up

A turn-based JRPG-style engine written in C#, inspired by *Final Fantasy VI*.
This project explores core game mechanics like stat-based combat, experience-based leveling, and modular monster generation — with full unit testing to support robust development.

---

## Features

- Custom player and monster classes with upgradable stats
- Fully tested EXP gain, leveling, damage formulas, and healing logic
- Turn-based combat with player choices (Attack, Defend, etc.)
- Dynamic monster generation via a `MonsterFactory`
- Combat log for replaying or analyzing battle sequences

---

## Tech Stack

- **Language**: C#
- **Testing**: xUnit
- **Editor**: VS Code
- **Runtime**: .NET 9+

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/en-us/download) (v9.0 or higher)
- Git (for cloning the repo)
- A C#-compatible editor (e.g., VS Code)

### Clone Repo

```bash
git clone https://github.com/demiipoet/Void.git
cd Void
```

### Testing Coverage
- EXP thresholds and level-up logic
- MaxHP, Strength, Defense scaling
- Combat damage formulas with mitigation
- Edge cases (negative EXP, damage limits, healing caps)
- Monster factory and error handling
- Attack, Defend, and Heal effects

### Quick Commands
- **Run Game:** `dotnet run --project FirstDraft`
- **Run Unit Tests:** `dotnet test FirstDraft.Tests`

### Example Output
![Void Turn-Based Combat Demo](https://github.com/demiipoet/Void/blob/main/demo/void_demo.gif)

---

## Future Plans
- Magic system
- Summons
- Random encounters
- Items / inventory
- Elemental weaknesses
- Save / load feature
- Buffs and debuffs
- Multiple enemy battles

## Acknowledgements
- Inspired by classic and modern JRPGs, such as Final Fantasy VI, Persona 5, and Dragon Quest IX

## License
The source code for Void is licensed under the MIT License. For details, see the LICENSE file.

The narrative content in this project is shared under the [CC BY-NC 4.0 License](https://creativecommons.org/licenses/by-nc/4.0/).

## About the Creator

I'm a former QA Analyst with 5 years of experience in Agile teams, now building *Void* full-time as a personal game development portfolio project. I’m currently learning Unity with the long-term goal of launching *Void* on the Nintendo Switch.


