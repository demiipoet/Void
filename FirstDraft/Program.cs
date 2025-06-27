using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Security;
using System.IO;
using System.Collections;
using System.Reflection.Metadata.Ecma335;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace FirstDraft
{
    /* ~~~~~~~~~~~~ Section: MagicAttack ~~~~~~~~~~~~ */
    public enum SpellType
    {
        Heal,
        Damage,
        Buff,
        Debuff
    }

    // Dunno if we'll need to change this to a traditional constructor
    // Maybe not if there's no logic here
    public class Spell(string name, SpellType type, int spellPower, int mpCost)
    {
        public string Name { get; } = name;
        public SpellType Type { get; } = type;
        public int SpellPower { get; } = spellPower;
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
            { Protect.Name, Protect },
            { Slow.Name, Slow }
        };
    }

    /* ~~~~~~~~~~~~ Section: Stats ~~~~~~~~~~~~ */
    public class Stats(int strength, int defense, int magicAttack, int magicDefense)
    {
        public int Strength { get; set; } = strength;
        public int Defense { get; set; } = defense;
        public int MagicAttack { get; set; } = magicAttack;
        public int MagicDefense { get; set; } = magicDefense;
    }

    /* ~~~~~~~~~~~~ Section: Player ~~~~~~~~~~~~ */
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
            KnownSpells.Add(SpellBook.Fire);
        }

        public (int EffectValue, string Message) CastSpell(string spellName, Monster monster, Player player)
        {
            var spell = KnownSpells.FirstOrDefault(s => s.Name == spellName);
            if (spell == null)
                return (0, $"\n(Error) {Name} does not know the spell '{spellName}'!\n");

            switch (spell.Type)
            {
                case SpellType.Heal:
                    double baseHealing = spell.SpellPower * 4;
                    double scalingHealing = Level * BaseStats.MagicAttack * 10.0 / 32;
                    int finalHealAmount;

                    if (CurrentHP == MaxHP)
                        finalHealAmount = 0;
                    else
                        finalHealAmount = (int)Math.Round(baseHealing + scalingHealing);

                    finalHealAmount = Math.Max(0, finalHealAmount);
                    CurrentHP = Math.Min(CurrentHP + finalHealAmount, MaxHP);

                    string healSpellMessage =
                        $"\n{Name} casts {spell.Name} and heals {finalHealAmount} HP!\n" +
                        $"{Name}'s HP: {CurrentHP}/{MaxHP}\n" +
                        $"{monster.Name}'s HP: {monster.CurrentHP}/{monster.MaxHP}\n";

                    return (finalHealAmount, healSpellMessage);

                case SpellType.Damage:
                    int spellDamage = CombatCalculator.CalculateMagicDamageToMonster(spell, player, monster);
                    var (returnSpellDamage, returnSpellMessage) = monster.TakeMagicDamage(spell, spellDamage, player);

                    return (returnSpellDamage, returnSpellMessage);

                default:
                    return (0, $"(Error) Spell type not implemented.");
            }
        }

        public string SelectSpell(string spellNumber, Player player, Monster monster)
        {
            switch (spellNumber)
            {
                case "1":
                    var (_, healMagicMessage) = player.CastSpell("Cure", monster, player);
                    return healMagicMessage;

                case "2":
                    var (_, damageMagicMessage) = player.CastSpell("Fire", monster, player);
                    return damageMagicMessage;

                default:
                    return "Something went wrong in the [SelectSpell] method.";
            }
        }

        // public string LearnSpell(int Level)
        // {
        //     if (Level >= 2)
        //     {
        //         KnownSpells.Add(SpellBook.Fire);
        //         return
        //             "\n~~~~~~~~~~~~~~~~~~~~~~~~~\n" +
        //             $"\n{Name} learned [Fire]!\n" +
        //             "\n~~~~~~~~~~~~~~~~~~~~~~~~~";
        //     }
        //     return "";
        // }

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
                // logMessage += $"Previous EXP: {prevExp}, Current EXP: {Experience}\n";
                // logMessage += LearnSpell(Level);
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

            int prevMagicAttack = BaseStats.MagicAttack;
            BaseStats.MagicAttack += 10;
            BaseStats.MagicAttack = Math.Min(999, BaseStats.MagicAttack);

            int prevMagicDefense = BaseStats.MagicDefense;
            BaseStats.MagicDefense += 10;
            BaseStats.MagicDefense = Math.Min(999, BaseStats.MagicDefense);

            // string learnSpellMessage = LearnSpell(Level);

            string levelUpMessage =
                $"\n{Name} leveled up!\n" +
                $"Previous Level: {prevLevel}, New Level: {Level}\n" +
                $"Previous Max HP: {prevMaxHP}, New Max HP: {MaxHP}\n" +
                $"Previous Strength: {prevStrength}, New Strength: {BaseStats.Strength}\n" +
                $"Previous Defense: {prevDefense}, New Defense: {BaseStats.Defense}\n" +
                $"Previous Magic Attack: {prevMagicAttack}, New Magic Strength: {BaseStats.MagicAttack}\n" +
                $"Previous Magic Defense: {prevMagicDefense}, New Magic Defense: {BaseStats.MagicDefense}\n";

            // levelUpMessage += learnSpellMessage;

            return levelUpMessage;
        }

        public (int FinalDamage, string Message) TakePhysicalDamage(double incomingDamage, Monster monster)
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

        public (int FinalDamage, string Message) TakeMagicDamage(Spell spell, int incomingDamage, Monster monster)
        {
            if (incomingDamage < 0)
            {
                return (0, $"(Error) {Name} cannot take negative damage! ({incomingDamage})");
            }

            double mitigationFactor = (255.0 - BaseStats.MagicDefense) / 256;
            int finalDamage = (int)Math.Round(incomingDamage * mitigationFactor + 1);
            finalDamage = Math.Min(9999, finalDamage);
            CurrentHP = Math.Max(CurrentHP - finalDamage, 0);

            string damageSpellMessage =
                $"\n{Name} casts {spell.Name} for {finalDamage} damage!\n" +
                $"{Name}'s HP: {CurrentHP}/{MaxHP}\n" +
                $"{monster.Name}'s HP: {monster.CurrentHP}/{monster.MaxHP}\n";

            return (finalDamage, damageSpellMessage);
        }

        public string KillMonster(Monster monster)
        {
            string logMessage = $"{Name} defeated {monster.Name}!\n";
            logMessage += ExpUp(monster.ExpGiven);

            return logMessage;
        }
    }

    /* ~~~~~~~~~~~~ Section: Monster ~~~~~~~~~~~~ */
    public class Monster(string name, int maxHP, int expGiven, Stats baseStats)
    {
        public string? Name { get; } = $"[Monster] {name}";
        public int MaxHP { get; } = maxHP;
        public int CurrentHP { get; private set; } = maxHP;
        public int ExpGiven { get; } = expGiven;
        public Stats BaseStats { get; private set; } = baseStats;

        public (int FinalDamage, string Message) TakePhysicalDamage(int incomingDamage, Player player)
        {
            if (incomingDamage < 0)
            {
                return (0, $"(Error) {Name} cannot take negative damage! ({incomingDamage})");
            }

            double mitigationFactor = (255.0 - BaseStats.Defense) / 256;
            int finalDamage = (int)Math.Round(incomingDamage * mitigationFactor + 1);
            finalDamage = Math.Min(9999, finalDamage);
            CurrentHP = Math.Max(CurrentHP - finalDamage, 0);

            string physAttackMessage =
            $"\n{player.Name} attacks {Name} for {finalDamage} damage!\n" +
            $"{player.Name}'s HP: {player.CurrentHP}/{player.MaxHP}\n" +
            $"{Name}'s HP: {CurrentHP}/{MaxHP}\n";

            return (finalDamage, physAttackMessage);
        }

        public (int FinalDamage, string Message) TakeMagicDamage(Spell spell, int incomingDamage, Player player)
        {
            if (incomingDamage < 0)
            {
                return (0, $"(Error) {Name} cannot take negative damage! ({incomingDamage})");
            }

            double mitigationFactor = (255.0 - BaseStats.MagicDefense) / 256;
            int finalDamage = (int)Math.Round(incomingDamage * mitigationFactor + 1);
            finalDamage = Math.Min(9999, finalDamage);
            CurrentHP = Math.Max(CurrentHP - finalDamage, 0);

            string damageSpellMessage =
                $"\n{Name} casts {spell.Name} for {finalDamage} damage!\n" +
                $"{player.Name}'s HP: {CurrentHP}/{MaxHP}\n" +
                $"{Name}'s HP: {CurrentHP}/{MaxHP}\n";

            return (finalDamage, damageSpellMessage);
        }
    }

    /* ~~~~~~~~~~~~ Section: Monster Factory ~~~~~~~~~~~~ */
    public static class MonsterFactory
    {
        public static Monster CreateMonster(int monsterID) =>
        monsterID switch
        {
            1 => new("Bat", 150, 5, new Stats(5, 6, 7, 8)),
            2 => new("Wolf", 150, 7, new Stats(6, 7, 8, 9)),
            3 => new("Wyvern", 150, 9, new Stats(7, 8, 9, 10)),
            _ => throw new ArgumentException("Invalid MonsterID")
        };
    }

    /* ~~~~~~~~~~~~ Section: Game Manager (Handles Combat) ~~~~~~~~~~~~ */
    public class CombatCalculator
    {
        public static int CalculatePhysicalDamageToPlayer(Player player, string playerChoice, int baseDamage)
        {
            double incomingDamage = playerChoice == "D"
                ? baseDamage / 2.0
                : baseDamage;

            double mitigationFactor = (255.0 - player.BaseStats.Defense) / 256;
            int finalDamage = (int)Math.Round(incomingDamage * mitigationFactor + 1);

            finalDamage = Math.Min(9999, finalDamage);

            return finalDamage;
        }

        public static int CalculatePhysicalDamageToMonster(Monster monster, int baseDamage)
        {
            double mitigationFactor = (255.0 - monster.BaseStats.Defense) / 256;
            int finalDamage = (int)Math.Round(baseDamage * mitigationFactor + 1);
            finalDamage = Math.Min(9999, finalDamage);

            return finalDamage;
        }

        public static int CalculateMagicDamageToMonster(Spell spell, Player player, Monster monster)
        {
            int baseSpellDamage = spell.SpellPower * 4 + (player.Level * player.BaseStats.MagicAttack * spell.SpellPower / 32);
            double mitigationFactor = (255.0 - monster.BaseStats.Defense) / 256;
            int finalDamage = (int)Math.Round(baseSpellDamage * mitigationFactor + 1);
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
                Console.WriteLine("Attack (A), Defend (D), Fire (F), Cure (C), Magic (M), or Run (R)? ");
                choice = (Console.ReadLine() ?? "").ToUpper();

                if (choice != "A" && choice != "D" && choice != "C" && choice != "F" && choice != "M" && choice != "R")
                {
                    Console.WriteLine("\nInvalid choice!\n");
                }
            } while (choice != "A" && choice != "D" && choice != "C" && choice != "F" && choice != "M" && choice != "R");

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
            int finalDamage = CombatCalculator.CalculatePhysicalDamageToPlayer(player, playerChoice, baseMonsterDamage);
            var (_, damageMessage) = player.TakePhysicalDamage(finalDamage, monster);


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
                            // Update when we introduce weapons / [BattlePower]
                            int basePlayerDamage = rng.Next(3, 8) + player.BaseStats.Strength;
                            int finalDamage = CombatCalculator.CalculatePhysicalDamageToMonster(monster, basePlayerDamage);
                            var (_, attackMessage) = monster.TakePhysicalDamage(finalDamage, player);
                            Log(attackMessage);

                            if (monster.CurrentHP <= 0)
                            {
                                string killMonsterMessage = player.KillMonster(monster);
                                Log(killMonsterMessage);

                                continue;
                            }
                            break;

                        case "D":
                            string defendMessage =
                                $"\n{player.Name} defends!\n" +
                                $"{player.Name}'s HP: {player.CurrentHP}/{player.MaxHP}\n" +
                                $"{monster.Name}'s HP: {monster.CurrentHP}/{monster.MaxHP}\n";

                            Log(defendMessage);
                            break;

                        case "C":
                            var (_, healMagicMessage) = player.CastSpell("Cure", monster, player);

                            Log(healMagicMessage);
                            break;

                        case "F":
                            var (_, damageMagicMessage) = player.CastSpell("Fire", monster, player);

                            Log(damageMagicMessage);

                            if (monster.CurrentHP <= 0)
                            {
                                string killMonsterMessage = player.KillMonster(monster);
                                Log(killMonsterMessage);

                                continue;
                            }
                            break;

                        case "M":
                            Console.WriteLine();
                            for (var i = 0; i < player.KnownSpells.Count; i++)
                            {
                                Console.WriteLine($"{player.KnownSpells[i].Name} ({i + 1})");
                            }
                            string spellSelection;
                            Console.WriteLine("Select a Spell");
                            spellSelection = Console.ReadLine() ?? "";

                            Log(player.SelectSpell(spellSelection, player, monster));

                            break;

                        case "R":
                            string runMessage =
                            $"\n{player.Name} runs! \n" +
                            player.ExpUp(0);

                            Log(runMessage);
                            run = true;

                            continue;

                        default:
                            Console.WriteLine("\nInput Error\n");
                            continue;
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

    /* ~~~~~~~~~~~~ Section: Main Program ~~~~~~~~~~~~ */
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nWelcome to Void.");

            // Stats playerStats = new(29, 52, 35, 25);
            Stats playerStats = new(999999, 52, 35, 25);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(1);
            Monster wolf = MonsterFactory.CreateMonster(2);
            Monster wyvern = MonsterFactory.CreateMonster(3);

            // Test iterating through [KnownSpells]
            // foreach (Spell spell in player.KnownSpells)
            // {
            //     Console.WriteLine($"Spell: {spell.Name}");
            // }


            // for (var i = 0; i < player.KnownSpells.Count; i++)
            // {
            //     // Console.WriteLine($"Spell: {player.KnownSpells[i].Name} ({i + 1})");
            //     Console.WriteLine($"{player.KnownSpells[i].Name} ({i + 1})");
            // }

            // string spellSelection;
            // Console.WriteLine("Select a Spell");
            // spellSelection = Console.ReadLine() ?? "";


            // Console.WriteLine($"Straight Print: {player.KnownSpells[0].Name}");


            Game.Battle(player, bat);
            // Game.Battle(player, wolf);
            // Game.Battle(player, wyvern);

            // Game.ShowCombatLog();

            string logName = Game.GenerateLogFilename("Bat");
            Game.SaveCombatLog(logName);

        }
    }

}