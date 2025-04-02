using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Security;
using System.IO;

namespace FirstDraft
{
    /* ~~~~~~~~~~~~ Magic ~~~~~~~~~~~~ */
    public class MagicList()
    {
        public int Cure { get; private set; } = 10;
    }

    /* ~~~~~~~~~~~~ Stats ~~~~~~~~~~~~ */
    public class Stats(int strength, int defense, int magic)
    {
        /*
        Player: 29
        Bat: 5
        Wolf: 6
        */
        public int Strength { get; set; } = strength; 

        /*
        Player: 52
        Bat: 6
        Wolf: 7
        */
        public int Defense { get; set; } = defense;

        /*
        Player: 35
        Bat: 7
        Wolf: 8
        */
        public int Magic { get; set; } = magic;

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
                logMessage += $"Previous EXP: {prevExp}, Current EXP: {Experience}\n";
            }
            return logMessage;
        }

        private string LevelUp()
        {
            int prevLevel = Level;
            Level++;

            Level = Math.Min(99, Level);

            int prevMaxHP = MaxHP;
            MaxHP += 50;
            MaxHP = Math.Min(9999, MaxHP);

            int prevStrength = BaseStats.Strength;
            BaseStats.Strength += 10;
            BaseStats.Strength = Math.Min(999, BaseStats.Strength);

            int prevDefense = BaseStats.Defense;
            BaseStats.Defense += 10;
            BaseStats.Defense= Math.Min(999, BaseStats.Defense);

            /* ~~~~~~~~ Written for debugging purposes ~~~~~~~~ */
            return
            $"{Name} leveled up!\n" + 
            $"Previous Level: {prevLevel}, New Level: {Level}\n" +
            $"Previous Max HP: {prevMaxHP}, New Max HP: {MaxHP}\n" + 
            $"Previous Strength: {prevStrength}, New Strength: {BaseStats.Strength}\n" +
            $"Previous Defense: {prevDefense}, New Defense: {BaseStats.Defense}\n\n";
        }
        
        public (int FinalDamage, string Message) TakeDamage(double incomingDamage, Monster monster)
        {
            if (incomingDamage < 0)
            {
                return (0, $"(Error) {Name} cannot take negative damage! ({incomingDamage})");
            }

            double mitigationFactor = (255.0 - BaseStats.Defense) / 256;
            int finalDamage = (int)Math.Round(incomingDamage * mitigationFactor + 1);
            finalDamage = Math.Min(9999, finalDamage);
            CurrentHP = Math.Max(CurrentHP - finalDamage, 0);

            string message =
            $"{monster.Name} attacks {Name} for {finalDamage} damage!\n" +
            $"{Name}'s HP: {CurrentHP}/{MaxHP}\n" + 
            $"{monster.Name}'s HP: {monster.CurrentHP}/{monster.MaxHP}";

            return (finalDamage, message);
        }

        /*
        public (int HealAmount, string Message) HealHP(int amount)
        {
            if (amount < 0)
            {
                return (0, $"(Error) {Name} cannot heal a negative amount! ({amount})");
            }


            int finalHealAmount = Math.Min(9999, amount);
            CurrentHP = Math.Min(CurrentHP + amount, MaxHP);

            string message =
            $"\n{Name} heals {amount} HP!\n" +
            $"{Name}'s HP: {CurrentHP}/{MaxHP}\n";

            // Console.WriteLine($"{Name} healed {amount} HP. HP: {CurrentHP}/{MaxHP}");

            return (amount, message);
        }
        */
        public (int HealAmount, string Message) HealHP()
        {
            // Spell Power: 10
            MagicList cure = new(); 
            int healAmount = cure.Cure * 4 + (Level * BaseStats.Magic * 10 / 32); // (40) + (10.9) = 50.9

            if (healAmount < 0)
            {
                return (0, $"(Error) {Name} cannot heal a negative amount! ({healAmount})");
            }

            CurrentHP = Math.Min(CurrentHP + healAmount, MaxHP);

            string message =
                $"\n{Name} heals {healAmount} HP!\n" +
                $"{Name}'s HP: {CurrentHP}/{MaxHP}\n";

            return (healAmount, message);
        }

        public string KillMonster(Monster monster)
        {
            string logMessage = $"{Name} defeated {monster.Name}\n";
            logMessage += ExpUp(monster.ExpGiven);
            return logMessage;
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

        public (int FinalDamage, string Message) TakeDamage(int incomingDamage, Player player)
        {
            if (incomingDamage < 0)
            {
                return (0, $"(Error) {Name} cannot take negative damage! ({incomingDamage})");
            }

            // Bat: (150, 5, 5, 6)
            //Wolf: (150, 7, 6, 7)
            double mitigationFactor = (255.0 - BaseStats.Defense) / 256;
            int finalDamage = (int)Math.Round(incomingDamage * mitigationFactor + 1);
            finalDamage = Math.Min(9999, finalDamage);
            CurrentHP = Math.Max(CurrentHP - finalDamage, 0);

            string message =
            $"\n{player.Name} attacks {Name} for {finalDamage} damage!\n" +
            $"{player.Name}'s HP: {player.CurrentHP}/{player.MaxHP}\n" +
            $"{Name}'s HP: {CurrentHP}/{MaxHP}\n";

            return (finalDamage, message);
        }

        public void HealHP(int amount)
        {
            if (amount < 0)
            {
                Console.WriteLine($"(Error) {Name} cannot heal by a negative amount!({amount})");
                return;
            }

            int prevHP = CurrentHP;
            // amount = Math.Min(9999, MaxHP);
            amount = Math.Min(amount, MaxHP);
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
            1 => new("Bat", 150, 5, new Stats(5, 6, 7)),
            2 => new("Wolf", 150, 7, new Stats(6, 7, 8)),
            _ => throw new ArgumentException("Invalid MonsterID")
        };
    }

    /* ~~~~~~~~~~~~ Game Manager (Handles Combat) ~~~~~~~~~~~~ */
    public static class Game
    {
        public static void SaveCombatLog(string fileName, string folderName = "Logs")
        {
            try
            {
                Directory.CreateDirectory(folderName);
                string fullPath = Path.Combine(folderName, fileName);
                File.WriteAllLines(fullPath, combatLog);
                // Console.WriteLine($"\nCombat log saved to {Path.GetFullPath(fullPath)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving log: {ex.Message}");
            }
        }

        public static string GenerateLogFilename(string monsterName)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return $"{monsterName.ToLower()}_battle_log_{timestamp}.txt";
        }

        private static string GetPlayerChoice()
        {
            string? choice;
            do
            {
                Console.WriteLine("Attack (A), Defend (D), or Heal (H)? ");
                // Console.WriteLine("Attack (A) or Defend (D)? ");
                choice = (Console.ReadLine() ?? "").ToUpper();

                if (choice != "A" && choice != "D" && choice != "H")
                // if (choice != "A" && choice != "D")
                {
                    Console.WriteLine("\nInvalid choice!\n");
                }
            } while (choice != "A" && choice != "D" && choice != "H");
            // } while (choice != "A" && choice != "D");

            return choice;
        }

        private static readonly List<string> combatLog = new();

        public static void Log(string message)
        {
            combatLog.Add(message);
            Console.WriteLine(message);
        }
        
        /*
        public static void ShowCombatLog()
        {
            Console.WriteLine("\n======== Start Combat Log ========");
            foreach (var entry in combatLog)
            {
                Console.WriteLine(entry);
            }
           Console.WriteLine("======== End Combat Log ========\n"); 
        }
        */

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

                        var (_, damageMessage) = monster.TakeDamage(playerDamageDealt, player);
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
                        Log($"\n{player.Name} defends!\n");
                    }
                    else if (processedBattleChoice == "H")
                    {
                        var (_, healMessage) = player.HealHP();
                        Log(healMessage);
                    }

                    /* ~~~~ Monster Attacks ~~~~ */
                    int monsterDamageDealt;
                    
                    if (processedBattleChoice == "D")
                    {
                        monsterDamageDealt = rng.Next(3, 9) + monster.BaseStats.Strength;
                        double reducedDamage = monsterDamageDealt / 2;
                        var (_, damageMessage) = player.TakeDamage(reducedDamage, monster);

                        Log(damageMessage);
                    }
                    else
                    {
                        monsterDamageDealt = rng.Next(3, 9) + monster.BaseStats.Strength;
                        var (_, damageMessage) = player.TakeDamage(monsterDamageDealt, monster);

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

            Stats playerStats = new(29, 52, 35);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(1);
            Monster wolf = MonsterFactory.CreateMonster(2);

            Game.Battle(player, bat);
            // Game.ShowCombatLog();

            string logName = Game.GenerateLogFilename(bat.Name ?? "");
            Game.SaveCombatLog(logName);

        }
    }

}