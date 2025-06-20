using System.Runtime.ConstrainedExecution;
using FirstDraft;
using Xunit;

namespace FirstDraft.Tests
{
    public class PlayerTests
    {
        /* ~~~~~~~~~~~ Section: EXP ~~~~~~~~~~~ */

        [Fact]
        public void PlayerLevelUp_GainMoreThan99Levels_LevelStopsAt99()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int over9K = 999999;
            int levelCap = 99;
            int statsCap = 999;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);

            // Act
            player.ExpUp(over9K);

            // Assert
            Assert.Equal(levelCap, player.Level);
            Assert.Equal(statsCap, player.BaseStats.Strength);
            Assert.Equal(statsCap, player.BaseStats.Defense);
            Assert.Equal(statsCap, player.BaseStats.Magic);
        }

        // Assuming EXP doesn't cause ding; for that, use [PlayerExpUp_MoreExpThanNeededToLevelUp_ExtraExpCarriesOver]
        [Fact]
        public void PlayerExpUp_GainOneExp_ExperienceIncreasesByOne()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int oneExp = 1;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            int expBefore = player.Experience;

            // Act
            // Gain 1 EXP
            player.ExpUp(oneExp);

            // Assert
            Assert.Equal(expBefore + oneExp, player.Experience);
        }

        [Fact]
        public void PlayerExpUp_MoreExpThanNeededToLevelUp_ExtraExpCarriesOver()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int extraExp = 15;
            int newExp = 5;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);

            // Act
            // 10 should ding, 5 should carry over (for LVL = 1)
            player.ExpUp(extraExp);

            // Assert
            // the leftover EXP should remain
            Assert.Equal(newExp, player.Experience);
        }

        [Fact]
        public void PlayerExpUp_GainEnoughExpToLevelUpMultipleTimes_LevelsUpMultipleTimes()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int testExp = 65;
            int expectedExp = 5;
            int expectedLevel = 4;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);

            // Act
            // If math is correct, should level up 3 times with [5] [Experience] leftover
            player.ExpUp(testExp);

            // Assert
            Assert.Equal(expectedExp, player.Experience);
            Assert.Equal(expectedLevel, player.Level);
        }

        [Fact]
        public void PlayerExpUp_GainMultipleLevels_CurrentHPRemainsTheSame()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int incomingExp = 65;
            int expectedLevel = 4;
            int expectedMaxHP = 450;
            int expectedCurrentHP = 300;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);

            // Act
            player.ExpUp(incomingExp);

            // Assert
            Assert.Equal(expectedLevel, player.Level);
            Assert.Equal(expectedMaxHP, player.MaxHP);
            Assert.Equal(expectedCurrentHP, player.CurrentHP);
        }

        [Fact]
        public void PlayerExpUp_GainExactAmountOfExpToLevelUp_LevelIncreaseByOne()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int incomingExp = 10;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            int initialLevel = player.Level;

            // Act
            // Should trigger ding at Level 50 (ding = [Level] * 10)
            player.ExpUp(incomingExp);

            // Assert
            Assert.Equal(initialLevel + 1, player.Level);
        }

        [Fact]
        public void PlayerExpUp_NegativeExp_ExperienceDoesNotChange()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int negativeExp = -9;
            int expectedExp = 0;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);

            // Act
            player.ExpUp(negativeExp);

            // Assert
            Assert.Equal(expectedExp, player.Experience);
        }

        [Fact]
        public void PlayerExpUp_ZeroExpGained_ExperienceDoesNotChange()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int zeroExp = 0;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            int initialLevel = player.Level;

            // Act
            player.ExpUp(zeroExp);

            // Assert
            Assert.Equal(initialLevel, player.Level);
        }

        [Fact]
        public void PlayerExpUp_MonsterKilled_CorrectExpGained()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int wolfMonsterID = 2;
            int wolfExp = 7;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            Monster wolf = MonsterFactory.CreateMonster(wolfMonsterID);

            // Act
            player.KillMonster(wolf);

            // Assert
            Assert.Equal(wolfExp, player.Experience);
        }

        [Fact]
        public void PlayerExpUp_KillMonsterWithEnoughExpToLevelUp_LevelUp()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int wolfHealth = 150;
            int wolfExp = 10;
            int wolfStrength = 6;
            int wolfDefense = 7;
            int wolfMagic = 8;
            int expectedPlayerLevel = 2;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);

            // Enough to level up from 1 to 2 with 5 leftover
            Monster wolf = new("Wolf", wolfHealth, wolfExp, new Stats(wolfStrength, wolfDefense, wolfMagic));

            // Act
            player.KillMonster(wolf);

            // Assert
            Assert.Equal(expectedPlayerLevel, player.Level);
        }

        [Fact]
        public void PlayerExpUp_KillMultipleMonsters_GainMultipleLevels()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int batMonsterID = 1;
            int expectedPlayerLevel = 3;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);

            // Act
            player.KillMonster(bat); // Level 1 (0 > 5 EXP)
            player.KillMonster(bat); // Level 1 > 2 ( 5 > 10 EXP)
            player.KillMonster(bat); // Level 2 (0 > 5 EXP)
            player.KillMonster(bat); // Level 2 (5 > 10 EXP)
            player.KillMonster(bat); // Level 2 ( 10 > 15 EXP)
            player.KillMonster(bat); // Level 3 (15 > 20 EXP)

            // Assert
            Assert.Equal(expectedPlayerLevel, player.Level);

        }

        /* ~~~~~~~~~~~ Section: Stats ~~~~~~~~~~~ */

        [Fact]
        public void Player_InstanceCreated_InitializedStatsAreCorrect()
        {
            // Arrange + Act
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);

            // Assert
            Assert.Equal(strengthStat, playerStats.Strength);
            Assert.Equal(defenseStat, playerStats.Defense);
            Assert.Equal(magicStat, playerStats.Magic);
        }

        [Fact]
        public void PlayerExplUp_LevelUp_BaseStatsIncreaseAppropriately()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int incomingExp = 10;
            int newStrength = 39;
            int newDefense = 62;
            int newMagic = 45;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);

            // Act
            player.ExpUp(incomingExp);

            // Assert
            Assert.Equal(newStrength, playerStats.Strength);
            Assert.Equal(newDefense, playerStats.Defense);
            Assert.Equal(newMagic, playerStats.Magic);
        }

        /* ~~~~~~~~~~~ Section: Damage ~~~~~~~~~~~ */

        [Fact]
        public void PlayerTakeDamage_Over9999_DamageStopsAt9999()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int batMonsterID = 1;
            int maxHP = 9999;
            int overkill = 999999;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);

            // Act
            var (finalDamage, _) = player.TakeDamage(overkill, bat);

            // Assert
            Assert.Equal(maxHP, finalDamage);
        }

        [Fact]
        public void PlayerTakeDamage_PlayerTakeDamage_CorrectAmount()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int batMonsterID = 1;
            int incomingDamage = 99;
            int remainingHP = 220;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);

            // Act
            player.TakeDamage(incomingDamage, bat);

            // Assert
            // HP: 300
            Assert.Equal(remainingHP, player.CurrentHP);
        }

        [Fact]
        public void PlayerTakeDamage_TakeMoreDamageThanCurrentHP_CurrentHPDoesNotGoNegative()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int batMonsterID = 1;
            int incomingDamage = 99999;
            int zeroHP = 0;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);

            // Act
            // Take more damage than HP
            player.TakeDamage(incomingDamage, bat);

            // Asssert
            // HP should be 0, not negative
            Assert.Equal(zeroHP, player.CurrentHP);
        }

        [Fact]
        public void PlayerTakeDamage_NegativeDamage_DoesNotChangeCurrentHP()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int batMonsterID = 1;
            int incomingDamage = -9;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);
            int hpBefore = player.CurrentHP;

            // Act
            player.TakeDamage(incomingDamage, bat);

            // Assert
            Assert.Equal(hpBefore, player.CurrentHP);

        }

        [Fact]
        public void PlayerTakeDamage_DefenseFormula_CorrectDamageTaken()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int batMonsterID = 1;
            int incomingDamage = 10;
            int expectedRemainingHP = 291;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);

            // Act
            player.TakeDamage(incomingDamage, bat);

            // Assert
            Assert.Equal(expectedRemainingHP, player.CurrentHP);
        }

        /* ~~~~~~~~~~~ Section: HP ~~~~~~~~~~~ */

        [Fact]
        public void PlayerHealHP_Heal_CorrectAmount()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int batMonsterID = 1;
            int incomingDamage = 125;
            int expectedRemainingHP = 251;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);

            // Act
            player.TakeDamage(incomingDamage, bat);
            player.CastSpell("Cure", bat);

            // Assert
            Assert.Equal(expectedRemainingHP, player.CurrentHP);
        }

        [Fact]
        public void Player_CreateInstance_CurrentHPMatchesMaxHP()
        {
            // Arrange + Act
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int expectedCurrentHP = 300;
            int expectedMaxHP = 300;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);

            // Assert
            Assert.Equal(player.CurrentHP, player.MaxHP);
            Assert.Equal(expectedCurrentHP, player.CurrentHP);
            Assert.Equal(expectedMaxHP, player.MaxHP);
        }

        [Fact]
        public void PlayerHealHP_HealMoreThanMaxHP_CurrentHPMatchesMaxHP()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int expectedCurrentHP = 300;
            int expectedMaxHP = 300;
            int incomingDamage = 50;
            int batMonsterID = 1;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);

            // Reduce HP first
            player.TakeDamage(incomingDamage, bat);

            // Act
            player.CastSpell("Cure", bat);

            // Assert
            Assert.Equal(expectedCurrentHP, expectedMaxHP);
        }

        [Fact]
        public void PlayerLevelUp_MaxHP_Increase()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int expectedMaxHP = 350;
            int incomingDamage = 50;
            int incomingExp = 10;
            int expectedLevel = 2;
            int batMonsterID = 1;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);
            player.TakeDamage(incomingDamage, bat); // [finalDamage] = [41]

            // Act
            // Level up player to Lv 2
            player.ExpUp(incomingExp);

            // Assert
            Assert.Equal(expectedLevel, player.Level);
            Assert.Equal(expectedMaxHP, player.MaxHP);
        }

        [Fact]
        public void PlayerLevelUp_CurrentHP_DoesNotChange()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int expectedCurrentHP = 259;
            int expectedMaxHP = 350;
            int incomingDamage = 50;
            int incomingExp = 10;
            int batMonsterID = 1;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);
            player.TakeDamage(incomingDamage, bat); // [finalDamage] = [41]

            // Act
            // Level up player to Lv 2
            player.ExpUp(incomingExp);

            // Assert
            Assert.Equal(expectedMaxHP, player.MaxHP);
            Assert.Equal(expectedCurrentHP, player.CurrentHP); // [300] - [41]

        }

        [Fact]
        public void PlayerHealHP_HealExactlyToMaxHP_CurrentHPMatchesMaxHP()
        {
            // Arrange
            int monsterFactoryID = 1;
            Monster bat = MonsterFactory.CreateMonster(monsterFactoryID);

            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int incomingDamage = 63; // Final Damage: 51
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);

            player.TakeDamage(incomingDamage, bat);

            // Act
            player.CastSpell("Cure", bat);

            // Assert
            Assert.Equal(player.MaxHP, player.CurrentHP);
        }

        [Fact]
        public void Player_InstantiateWithCustomMaxHP_InstanceHasCustomMaxHP()
        {
            // Arrange + Act
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int customMaxHP = 200;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats, customMaxHP);

            // Assert
            Assert.Equal(customMaxHP, player.MaxHP);
            Assert.Equal(customMaxHP, player.CurrentHP);
        }

        /* ~~~~~~~~~~~ Section: Magic ~~~~~~~~~~~ */

        [Fact]
        public void Player_Initialized_KnowsOnlyCure()
        {
            // Arrange + Act
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int customMaxHP = 200;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats, customMaxHP);

            // Assert
            Assert.Single(player.KnownSpells);
            Assert.Contains(SpellBook.Cure, player.KnownSpells);
        }


        /* ~~~~~~~~~~~ Section: Magic ~~~~~~~~~~~ */

        [Fact]
        public void Player_TakeDamageReturnMessage_IsCorrect()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int wolfHealth = 150;
            int wolfExp = 10;
            int wolfStrength = 6;
            int wolfDefense = 7;
            int wolfMagic = 8;
            double incomingDamage = 8;

            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            Monster wolf = new("Wolf", wolfHealth, wolfExp, new Stats(wolfStrength, wolfDefense, wolfMagic));

            var (finalDamage, damageMessage) = player.TakeDamage(incomingDamage, wolf);

            string expectedMessage =
            $"{wolf.Name} attacks {player.Name} for {finalDamage} damage!\n" +
            $"{player.Name}'s HP: {player.CurrentHP}/{player.MaxHP}\n" +
            $"{wolf.Name}'s HP: {wolf.CurrentHP}/{wolf.MaxHP}";

            // Act
            player.TakeDamage(incomingDamage, wolf);

            // Assert
            Assert.Equal(expectedMessage, damageMessage);
        } 

        [Fact]
        public void Player_CastSpellReturnMessage_IsCorrect()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int wolfHealth = 150;
            int wolfExp = 10;
            int wolfStrength = 6;
            int wolfDefense = 7;
            int wolfMagic = 8;
            int finalHealAmount = 0;

            Spell cure = new("Cure", SpellType.Heal, 10, 5);
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            Monster wolf = new("Wolf", wolfHealth, wolfExp, new Stats(wolfStrength, wolfDefense, wolfMagic));

            string expectedMessage =
                $"\n{player.Name} casts {cure.Name} and heals {finalHealAmount} HP!\n" +
                $"{player.Name}'s HP: {player.CurrentHP}/{player.MaxHP}\n" +
                $"{wolf.Name}'s HP: {wolf.CurrentHP}/{wolf.MaxHP}\n";

            // Act
            var (EffectValue, healMessage) = player.CastSpell("Cure", wolf);

            // Assert
            Assert.Equal(expectedMessage, healMessage);
        } 
    }
}