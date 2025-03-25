using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Security;

namespace FirstDraft
{
    /* ~~~~~~~~~~~~ Stats ~~~~~~~~~~~~ */
    public class Stats(int strength, int defense)
    {
        // Player: 29
        // Bat: 5
        // Wolf: 6
        public int Strength { get; set; } = strength; 

        // Player: 52
        // Bat: 6
        // Wolf: 7
        public int Defense { get; set; } = defense; 

    }
    
    /* ~~~~~~~~~~~~ Player ~~~~~~~~~~~~ */
    public class Player(string name, Stats baseStats, int maxHP = 300)
    {
        public string? Name { get; } = $"[Player] {name}";
        public int Level { get; private set; } = 1;
        public int MaxHP { get; private set; } = maxHP;
        public int CurrentHP { get; private set; } = maxHP;
        public Stats BaseStats { get; private set; } = baseStats;
        public int Experience { get; private set; } = 0;
        /* Deleted [ExpThreshold => Level * 10;] */
        private int ExpThreshold => Level * 10;

        public string ExpUp(int exp)
        {
            if (exp < 0)
            {
                return $"(Error) {Name} cannot receive negative EXP! ({exp})";
            }
            
            string logMessage = $"\n{Name} gained {exp} EXP!\n";
            int prevExp;
            Experience += exp;
            logMessage += $"Current EXP: {Experience}\n";
            
            // Update threshold after level up
            while (Experience >= ExpThreshold)
            {
                prevExp = Experience;
                Experience -= ExpThreshold;
                logMessage += LevelUp();
                logMessage += $"Current EXP: {Experience}\n";
            }
            return logMessage;
        }

        private string LevelUp()
        {
            int prevLevel = Level;
            Level++;

            if (Level > 99)
            {
                Level = 99;
            }

            int prevMaxHP = MaxHP;
            // MaxHP += 50;
            MaxHP += 999;
            if (MaxHP > 999)
            {
                MaxHP = 999;
            }

            int prevStrength = BaseStats.Strength;
            // BaseStats.Strength += 10;
            BaseStats.Strength += 999;
            if (BaseStats.Strength> 999)
            {
                BaseStats.Strength = 999;
            }

            int prevDefense = BaseStats.Defense;
            // BaseStats.Defense += 10;
            BaseStats.Defense += 999;
            if (BaseStats.Defense > 999)
            {
                BaseStats.Defense = 999;
            }

            /* ~~~~~~~~ Written for debugging purposes ~~~~~~~~ */
            return $"{Name} leveled up!\n" + 
            $"Previous Level: {prevLevel}, New Level: {Level}\n" +
            $"Previous Max HP: {prevMaxHP}, New Max HP: {MaxHP}\n" + 
            $"Previous Strength: {prevStrength}, New Strength: {BaseStats.Strength}\n" +
            $"Previous Defense: {prevDefense}, New Defense: {BaseStats.Defense}\n\n";

            // Console.WriteLine($"{Name} leveled up!");
            // Console.WriteLine($"Previous Level: {prevLevel}, New Level: {Level}");
            // Console.WriteLine($"Previous Max HP: {prevMaxHP}, New Max HP: {MaxHP}");
            // Console.WriteLine($"Previous Strength: {prevStrength}, New Strength: {BaseStats.Strength}");
            // Console.WriteLine($"Previous Defense: {prevDefense}, New Defense: {BaseStats.Defense}");
        }
        
        public string TakeDamage(double damage, Monster monster)
        {
            if (damage < 0)
            {
                return $"(Error) {Name} cannot take negative damage! ({damage})";
            }

            double incomingDamage = damage;
            double mitigationFactor = (255.0 - BaseStats.Defense) / 256;
            int finalDamage = (int)Math.Round(incomingDamage * mitigationFactor + 1);
            
            /* Do we need this in addition to the above? */
            // finalDamage = Math.Max(finalDamage, 1);

            if (finalDamage >= 9999)
            {
                finalDamage = 9999;
            }

            CurrentHP = Math.Max(CurrentHP - finalDamage, 0);

            return $"{monster.Name} attacks {Name} for {finalDamage} damage!\n" +
            $"{Name}'s HP: {CurrentHP}/{MaxHP}\n" + 
            $"{monster.Name}'s HP: {monster.CurrentHP}/{monster.MaxHP}";
        }

        public void HealHP(int amount)
        {
            if (amount < 0)
            {
                Console.WriteLine($"(Error) {Name} cannot heal a negative amount! ({amount})");
                return;
            }

            int prevHP = CurrentHP;
            // Prevents [CurrentHP] from going above [MaxHP]
            CurrentHP = Math.Min(CurrentHP + amount, MaxHP);
            
            Console.WriteLine($"{Name} healed {amount} HP. HP: {CurrentHP}/{MaxHP}");

            /* ~~~~~~~~ Written for debugging purposes ~~~~~~~~ */
            // Console.WriteLine($"{Name} healed {amount} HP.");
            // Console.WriteLine($"Previous HP: {prevHP}, New HP: {CurrentHP}");
        }

        public string KillMonster(Monster monster)
        {
            string logMessage = $"{Name} defeated {monster.Name}\n";
            logMessage += ExpUp(monster.ExpGiven);
            return logMessage;

            // Console.WriteLine($"{Name} defeated {monster.Name}\n");
            // ExpUp(monster.ExpGiven);
        }
    }

    /* ~~~~~~~~~~~~ Monster ~~~~~~~~~~~~ */
    public class Monster(string name, int maxHP, int expGiven, Stats baseStats)
    {
        public string? Name { get; } = $"[Monster] {name}";
        public int MaxHP { get; } = maxHP;
        public int CurrentHP { get; private set; } = maxHP;
        public int ExpGiven { get; } = expGiven;
        public Stats BaseStats {get; private set; } = baseStats;

        public string TakeDamage(int damage, Player player)
        {
            if (damage < 0)
            {
                return $"(Error) {Name} cannot take negative damage! ({damage})";
            }

            double incomingDamage = damage;
            double mitigationFactor = (255.0 - BaseStats.Defense) / 256;
            int finalDamage = (int)Math.Round(incomingDamage * mitigationFactor + 1);

            if (finalDamage >= 9999)
            {
                finalDamage = 9999;
            }

            CurrentHP = Math.Max(CurrentHP - finalDamage, 0);

            return $"\n{player.Name} attacks {Name} for {finalDamage} damage!\n" +
            $"{player.Name}'s HP: {player.CurrentHP}/{player.MaxHP}\n" +
            $"{Name}'s HP: {CurrentHP}/{MaxHP}\n";
        }

        public void HealHP(int amount)
        {
            if (amount < 0)
            {
                Console.WriteLine($"(Error) {Name} cannot heal by a negative amount!({amount})");
                return;
            }

            int prevHP = CurrentHP;
            // Prevents [CurrentHP] from going above [MaxHP]
            CurrentHP = Math.Min(CurrentHP + amount, MaxHP);
            
            // Console.WriteLine($"{Name} healed {amount} HP. HP: {CurrentHP}/{MaxHP}");

            /* ~~~~~~~~ Written for debugging purposes ~~~~~~~~ */
            Console.WriteLine($"{Name} healed {amount} HP.");
            Console.WriteLine($"Previous HP: {prevHP}, New HP: {CurrentHP}");
        }
    }

    /* ~~~~~~~~~~~~ Monster Factory ~~~~~~~~~~~~ */
    public static class  MonsterFactory
    {
        public static Monster CreateMonster(int monsterID) =>
        monsterID switch
        {
            // 1 => new("Bat", 150, 5, new Stats(5, 6)),
            1 => new("Bat", 150, 5, new Stats(999999, 6)),
            2 => new("Wolf", 150, 7, new Stats(6, 7)),
            _ => throw new ArgumentException("Invalid MonsterID")
        };
    }

    /* ~~~~~~~~~~~~ Game Manager (Handles Combat) ~~~~~~~~~~~~ */
    public static class Game
    {
        private static string GetPlayerChoice()
        {
            string? choice;
            do
            {
                Console.WriteLine("Attack (A), Defend (D), or Heal (H)? ");
                choice = (Console.ReadLine() ?? "").ToUpper();

                if (choice != "A" && choice != "D" & choice != "H")
                {
                    Console.WriteLine("\nInvalid choice!\n");
                }
            } while (choice != "A" && choice != "D" && choice != "H");

            return choice;
        }

        private static readonly List<string> combatLog = new();

        public static void Log(string message)
        {
            combatLog.Add(message);
            Console.WriteLine(message);
        }
        
        public static void ShowCombatLog()
        {
            Console.WriteLine("\n======== Start Combat Log ========");
            foreach (var entry in combatLog)
            {
                Console.WriteLine(entry);
            }
        //    Console.WriteLine("==================\n"); 
           Console.WriteLine("======== End Combat Log ========\n"); 
        }

        public static void Battle(Player player, Monster monster)
        {
            int turnNumber = 1;
            combatLog.Clear();

            if (player.CurrentHP > 0)
            {
                Random rng = new();

                Log($"\nA wild {monster.Name} appears!");
                Log($"{player.Name}'s HP: {player.CurrentHP}/{player.MaxHP}");
                Log($"{monster.Name}'s HP: {monster.CurrentHP}/{monster.MaxHP}\n");

                while (player.CurrentHP > 0 && monster.CurrentHP > 0)
                {
                    /* ~~~~ Player Attacks ~~~~ */
                    Log($"\n--- Turn {turnNumber} ---");
                    string? processedBattleChoice = GetPlayerChoice();
                    
                    if (processedBattleChoice == "A")
                    {
                        int playerDamageDealt = rng.Next(3, 8) + player.BaseStats.Strength;

                        string damageMessage = monster.TakeDamage(playerDamageDealt, player);
                        Log(damageMessage);

                        if (monster.CurrentHP <= 0)
                        {
                            string killMonsterMessage = player.KillMonster(monster);
                            Log(killMonsterMessage);
                            break;
                        }
                    }
                    else if (processedBattleChoice == "D")
                    {
                        Log($"{player.Name} defends!\n");
                    }

                    /* ~~~~ Monster Attacks ~~~~ */
                    int monsterDamageDealt;
                    
                    if (processedBattleChoice == "D")
                    {
                        monsterDamageDealt = rng.Next(3, 9) + monster.BaseStats.Strength;
                        double reducedDamage = monsterDamageDealt / 2;
                        string damageMessage = player.TakeDamage(reducedDamage, monster);

                        Log(damageMessage);
                    }
                    else
                    {
                        monsterDamageDealt = rng.Next(3, 9) + monster.BaseStats.Strength;
                        string damageMessage = player.TakeDamage(monsterDamageDealt, monster);

                        Log(damageMessage);
                    }

                    if (player.CurrentHP <= 0)
                    {
                        Log($"{player.Name} has been defeated!");
                    }

                    turnNumber++;
                }
            }
        }
    }

    /* ~~~~~~~~~~~~ Main Program ~~~~~~~~~~~~ */
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nWelcome to Void.");

            // Stats playerStats = new(29, 52);
            Stats playerStats = new(29, 52);
            Player player = new("Zaza", playerStats);
            Monster bat = MonsterFactory.CreateMonster(1);
            Monster wolf = MonsterFactory.CreateMonster(2);

            Game.Battle(player, bat);
            Console.WriteLine("\n~~~~~~~~");
            Game.ShowCombatLog();

            // Console.WriteLine("\n~~~~~~~~");

            // Game.Battle(player, wolf);
            // Console.WriteLine("\n~~~~~~~~");
            // Game.ShowCombatLog();
            
        }
    }

}