using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Security;
using System.IO;
using System.Collections;

namespace FirstDraft
{
    /*
    int strengthStat = 29;
    int defenseStat = 52;
    int magicStat = 35;
    Stats playerStats = new(strengthStat, defenseStat, magicStat);
    Player player = new ("Freya", playerStats);

   */ 

    /* ~~~~~~~~~~~~ Magic ~~~~~~~~~~~~ */
    public enum SpellType
    { 
        Heal,
        Damage,
        Buff,
        Debuff
    }

    // Dunno if we'll need to change this to a traditional constructor
    // Maybe not if there's no logic here
    public class Spell(string name, SpellType type, int power, int mpCost)
    {
        public string Name { get; } = name;
        public SpellType Type { get; } = type;
        public int Power { get; } = power;
        public int MPCost { get; } = mpCost;
    }

    public static class SpellBook
    {
        public static readonly Spell Cure = new("Cure", SpellType.Heal, 10, 5);
        public static readonly Spell Fire = new("Fire", SpellType.Damage, 12, 4);
        public static readonly Spell Protect = new("Protect", SpellType.Buff, 0, 6);
        public static readonly Spell Slow = new("Slow", SpellType.Debuff, 0, 7);

        public static readonly Dictionary<string, Spell> AllSpells = new()
        {
            { Cure.Name, Cure },
            { Fire.Name, Fire },
            { Protect.Name, Protect},
            { Slow.Name, Slow}
        };
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
    /* Converting to traditional constructor */
    // public class Player(string name, Stats baseStats, int maxHP = 300)
    public class Player
    {
        public string? Name { get; }
        public int Level { get; private set; }
        public int MaxHP { get; private set; }
        public int CurrentHP { get; private set; }
        public Stats BaseStats { get; private set; }
        public int Experience { get; private set; }
        public List<Spell> KnownSpells { get; } = [];
        private int ExpThreshold => Level * 10;

        public Player(string name, Stats baseStats, int maxHP = 300)
        {
            Name = $"[Player] {name}";
            Level = 1;
            MaxHP = maxHP;
            CurrentHP = maxHP;
            BaseStats = baseStats;
            Experience = 0;

            // Every player begins with [Cure]
            KnownSpells.Add(SpellBook.Cure);
        }

        public (int EffectValue, string Message) CastSpell(string spellName, Monster monster)
        {
            var spell = KnownSpells.FirstOrDefault(s => s.Name == spellName);
            if (spell == null)
                return (0, $"(Error) {Name} does not know the spell '{spellName}'.");

            switch (spell.Type)
            {
                case SpellType.Heal:
                    double baseHealing = spell.Power * 4;
                    double scalingHealing = Level * BaseStats.Magic * 10.0 / 32;
                    // int finalHealAmount = (int)Math.Round(baseHealing + scalingHealing);
                    int finalHealAmount;

                    if (CurrentHP == MaxHP)
                        finalHealAmount = 0;
                    else
                        finalHealAmount = (int)Math.Round(baseHealing + scalingHealing);
                        finalHealAmount = Math.Max(0, finalHealAmount);
                        CurrentHP = Math.Min(CurrentHP + finalHealAmount, MaxHP);

                    string message =
                        $"\n{Name} casts {spell.Name} and heals {finalHealAmount} HP!\n" +
                        $"{Name}'s HP: {CurrentHP}/{MaxHP}\n" +
                        $"{monster.Name}'s HP: {monster.CurrentHP}/{monster.MaxHP}\n";

                    return (finalHealAmount, message);

                case SpellType.Damage:
                    /* ~~~~ Update formula ~~~~ */
                    int baseDamage = (int)Math.Round(spell.Power * (BaseStats.Magic / 10.0));
                    return (baseDamage, $"{Name} casts {spell.Name} and deals {baseDamage} damage!");

                default:
                    return (0, $"(Error) Spell type not implemented.");
            }
        }

        public string ExpUp(int exp)
        {
            if (exp < 0)
            {
                return $"(Error) {Name} cannot receive negative EXP! ({exp})";
            }

            string logMessage =
            "\n~~~~~~~~~~~~~~~~~~~~~~~~~\n" + 
            $"\n{Name} gained {exp} EXP!\n";
            int prevExp;
            Experience += exp;
            logMessage += $"Current EXP: {Experience}\n" +
            "\n~~~~~~~~~~~~~~~~~~~~~~~~~\n";

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
            BaseStats.Defense = Math.Min(999, BaseStats.Defense);

            int prevMagic = BaseStats.Magic;
            BaseStats.Magic += 10;
            BaseStats.Magic = Math.Min(999, BaseStats.Magic);

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

        public string KillMonster(Monster monster)
        {
            string logMessage =  $"{Name} defeated {monster.Name}!\n";
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
        public Stats BaseStats { get; private set; } = baseStats;

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
    }

    /* ~~~~~~~~~~~~ Monster Factory ~~~~~~~~~~~~ */
    public static class MonsterFactory
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
        public class CombatCalculator
    {
        public static int CalculateDamageToPlayer(Player player, string playerChoice, int baseDamage)
        { 
            double incomingDamage = playerChoice == "D"
                ? baseDamage / 2.0
                : baseDamage;

            double mitigationFactor = (255.0 - player.BaseStats.Defense) / 256;
            int finalDamage = (int)Math.Round(incomingDamage * mitigationFactor + 1);
            
            finalDamage = Math.Min(9999, finalDamage);

            return finalDamage;
        }

        public static int CalculateDamageToMonster(Monster monster, int baseDamage)
        { 
            double mitigationFactor = (255.0 - monster.BaseStats.Defense) / 256;
            int finalDamage = (int)Math.Round(baseDamage * mitigationFactor + 1);

            finalDamage = Math.Min(9999, finalDamage);

            return finalDamage;
        }
    }
    
    public static class Game
    {
        public static void SaveCombatLog(string fileName, string folderName = "Logs")
        {
            try
            {
                Directory.CreateDirectory(folderName);
                string fullPath = Path.Combine(folderName, fileName);
                File.WriteAllLines(fullPath, combatLog);
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
                Console.WriteLine("Attack (A), Defend (D), Heal (H), or Run (R)? ");
                choice = (Console.ReadLine() ?? "").ToUpper();

                if (choice != "A" && choice != "D" && choice != "H" && choice != "R")
                {
                    Console.WriteLine("\nInvalid choice!\n");
                }
            } while (choice != "A" && choice != "D" && choice != "H" && choice != "R");

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
            Console.WriteLine("======== End Combat Log ========\n"); 
        }

        private static bool ResolveEnemyTurn(Player player, Monster monster, string playerChoice, Random rng)
        {
            bool isPlayerAlive = true;
            int baseMonsterDamage = rng.Next(3, 9) + monster.BaseStats.Strength;
            int finalDamage = CombatCalculator.CalculateDamageToPlayer(player, playerChoice, baseMonsterDamage);
            var (_, damageMessage) = player.TakeDamage(finalDamage, monster);

            Log(damageMessage);

            if (player.CurrentHP <= 0)
            {
                Log($"\n{player.Name} has been defeated!\n");
                isPlayerAlive = false;
            }

            return isPlayerAlive;
        }

        public static void Battle(Player player, Monster monster)
        {
            int turnNumber = 1;
            combatLog.Clear();

            if (player.CurrentHP > 0)
            {
                Random rng = new();
                bool run = false;

                string battleStartMessage = 
                $"\nA wild {monster.Name} appears!\n" +
                $"{player.Name}'s HP: {player.CurrentHP}/{player.MaxHP}\n" +
                $"{monster.Name}'s HP: {monster.CurrentHP}/{monster.MaxHP}\n";

                Log(battleStartMessage);
                while (player.CurrentHP > 0 && monster.CurrentHP > 0 && run == false)
                {
                    /* ~~~~ Player Attacks ~~~~ */
                    Log($"\n--- Turn {turnNumber} ---");
                    string? processedBattleChoice = GetPlayerChoice();
                    
                    switch (processedBattleChoice)
                    {
                        case "A":
                            int basePlayerDamage = rng.Next(3, 8) + player.BaseStats.Strength;
                            int finalDamage = CombatCalculator.CalculateDamageToMonster(monster, basePlayerDamage);
                            var (_, attackMessage) = monster.TakeDamage(finalDamage, player);
                            Log(attackMessage);

                            if (monster.CurrentHP <= 0)
                            {
                                string killMonsterMessage = player.KillMonster(monster);
                                Log(killMonsterMessage);

                                continue;
                            }
                            break;

                        case "D":
                            string message = 
                                $"\n{player.Name} defends!\n" +
                                $"{player.Name}'s HP: {player.CurrentHP}/{player.MaxHP}\n" +
                                $"{monster.Name}'s HP: {monster.CurrentHP}/{monster.MaxHP}\n";

                            Log(message);
                            break;

                        case "H":
                            var (_, healMessage) = player.CastSpell("Cure", monster);
                            
                            Log(healMessage);
                            break;
                        
                        case "R":
                            string runMessage =
                            $"{player.Name} runs! \n" +
                            $"{player.Name} gained {player.Experience} experience!";
                            // Log($"{player.Name} runs!");
                            Log(runMessage);
                            run = true;
                            continue;
                            
                        default:
                            Console.WriteLine("\nInput Error\n");
                            break;
                    }

                    /* ~~~~ Monster Attacks ~~~~ */
                    if (monster.CurrentHP > 0)
                    {
                        ResolveEnemyTurn(player, monster, processedBattleChoice, rng);
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

            string logName = Game.GenerateLogFilename("Bat");
            Game.SaveCombatLog(logName);

        }
    }

}