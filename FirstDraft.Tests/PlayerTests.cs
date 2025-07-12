using System.Runtime.ConstrainedExecution;
using FirstDraft;
using Xunit;

namespace FirstDraft.Tests
{
    public class PlayerTests
    {
        /* ~~~~~~~~~~~ Section: Helper Methods ~~~~~~~~~~~ */
        private static Player CreateTestPlayer()
        {
            Stats stats = new(29, 52, 35, 36);

            return new("Freya", stats);
        }

        private static Monster CreateTestBat()
        {
            return MonsterFactory.CreateMonster(1);
        }

        private static Monster CreateTestWolf()
        {
            return MonsterFactory.CreateMonster(2);
        }

        private static Monster CreateTestWyvern()
        {
            return MonsterFactory.CreateMonster(3);
        }

        /* ~~~~~~~~~~~ Section: EXP ~~~~~~~~~~~ */

        [Fact]
        public void PlayerLevelUp_GainMoreThan99Levels_LevelStopsAt99()
        {
            // Arrange
            Player player = CreateTestPlayer();
            int over9kExp = 999999;
            int levelCap = 99;
            int statsCap = 999;

            // Act
            player.ExpUp(over9kExp);

            // Assert
            Assert.Equal(levelCap, player.Level);
            Assert.Equal(statsCap, player.BaseStats.Strength);
            Assert.Equal(statsCap, player.BaseStats.Defense);
            Assert.Equal(statsCap, player.BaseStats.MagicAttack);
            Assert.Equal(statsCap, player.BaseStats.MagicDefense);
        }

        // Assuming EXP doesn't cause ding; for that, use [PlayerExpUp_MoreExpThanNeededToLevelUp_ExtraExpCarriesOver]
        [Fact]
        public void PlayerExpUp_GainOneExp_ExperienceIncreasesByOne()
        {
            // Arrange
            Player player = CreateTestPlayer();
            int oneExp = 1;
            int expBefore = player.Experience;

            // Act
            player.ExpUp(oneExp);

            // Assert
            Assert.Equal(expBefore + oneExp, player.Experience);
        }

        [Fact]
        public void PlayerExpUp_MoreExpThanNeededToLevelUp_ExtraExpCarriesOver()
        {
            // Arrange
            Player player = CreateTestPlayer();
            int extraExp = 15;
            int newExp = 5;

            // Act
            player.ExpUp(extraExp); // 10 should ding, 5 should carry over (for LVL = 1)

            // Assert
            Assert.Equal(newExp, player.Experience); // Only the leftover EXP should remain
        }

        [Fact]
        public void PlayerExpUp_GainEnoughExpToLevelUpMultipleTimes_LevelsUpMultipleTimes()
        {
            // Arrange
            Player player = CreateTestPlayer();
            int testExp = 65;
            int expectedExp = 5;
            int expectedLevel = 4;

            // Act
            player.ExpUp(testExp); // If math is correct, should level up 3 times with [5] [Experience] leftover

            // Assert
            Assert.Equal(expectedExp, player.Experience);
            Assert.Equal(expectedLevel, player.Level);
        }

        [Fact]
        public void PlayerExpUp_GainMultipleLevels_CurrentHPRemainsTheSame()
        {
            // Arrange
            Player player = CreateTestPlayer();
            int incomingExp = 65;
            int expectedLevel = 4;
            int expectedMaxHP = 450;
            int expectedCurrentHP = 300;

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
            Player player = CreateTestPlayer();
            int incomingExp = 10;
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
            Player player = CreateTestPlayer();
            int negativeExp = -9;
            int expectedExp = 0;

            // Act
            player.ExpUp(negativeExp);

            // Assert
            Assert.Equal(expectedExp, player.Experience);
        }

        [Fact]
        public void PlayerExpUp_ZeroExpGained_ExperienceDoesNotChange()
        {
            // Arrange
            Player player = CreateTestPlayer();
            int zeroExp = 0;
            int initialLevel = player.Level;

            // Act
            player.ExpUp(zeroExp);

            // Assert
            Assert.Equal(initialLevel, player.Level);
            Assert.Equal(0, player.Experience);
        }

        [Fact]
        public void PlayerExpUp_MonsterKilled_CorrectExpGained()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster wolf = CreateTestWolf();

            // Act
            player.KillMonster(wolf);

            // Assert
            Assert.Equal(wolf.ExpGiven, player.Experience);
        }

        [Fact]
        public void PlayerExpUp_KillMonsterWithEnoughExpToLevelUp_LevelUp()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster wyvern = CreateTestWyvern();
            int expectedPlayerLevel = 2;

            // Act
            player.KillMonster(wyvern);

            // Assert
            Assert.Equal(expectedPlayerLevel, player.Level);
        }

        [Fact]
        public void PlayerExpUp_KillMultipleMonsters_GainMultipleLevels()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster bat = CreateTestBat();
            int expectedPlayerLevel = 3;

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
            int playerStrength = 29;
            int playerDefense = 52;
            int playerMagicAttack = 35;
            int playerMagicDefense = 36;
            Stats playerStats = new(playerStrength, playerDefense, playerMagicAttack, playerMagicDefense);
            Player player = new("Freya", playerStats);

            // Assert
            Assert.Equal(playerStrength, player.BaseStats.Strength);
            Assert.Equal(playerDefense, player.BaseStats.Defense);
            Assert.Equal(playerMagicAttack, player.BaseStats.MagicAttack);
            Assert.Equal(playerMagicDefense, player.BaseStats.MagicDefense);
        }

        [Fact]
        public void PlayerExplUp_LevelUp_BaseStatsIncreaseAppropriately()
        {
            // Arrange
            Player player = CreateTestPlayer();
            int incomingExp = 10;
            int newStrength = 39;
            int newDefense = 62;
            int newMagicAttack = 45;
            int newMagicDefense = 46;

            // Act
            player.ExpUp(incomingExp);

            // Assert
            Assert.Equal(newStrength, player.BaseStats.Strength);
            Assert.Equal(newDefense, player.BaseStats.Defense);
            Assert.Equal(newMagicAttack, player.BaseStats.MagicAttack);
            Assert.Equal(newMagicDefense, player.BaseStats.MagicDefense);
        }

        /* ~~~~~~~~~~~ Section: Damage ~~~~~~~~~~~ */

        [Fact]
        public void PlayerTakeDamage_Over9999_DamageStopsAt9999()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster bat = CreateTestBat();
            int maxHP = 9999;
            int overkill = 999999;

            // Act
            var (finalDamage, _) = player.TakePhysicalDamage(overkill, bat);

            // Assert
            Assert.Equal(maxHP, finalDamage);
        }

        [Fact]
        public void PlayerTakeDamage_PlayerTakeDamage_CorrectAmount()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster bat = CreateTestBat();
            int incomingDamage = 99;
            int remainingHP = 220;

            // Act
            player.TakePhysicalDamage(incomingDamage, bat);

            // Assert
            // HP: 300
            Assert.Equal(remainingHP, player.CurrentHP);
        }

        [Fact]
        public void PlayerTakeDamage_TakeMoreDamageThanCurrentHP_CurrentHPDoesNotGoNegative()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster bat = CreateTestBat();
            int incomingDamage = 99999;
            int zeroHP = 0;

            // Act
            // Take more damage than HP
            player.TakePhysicalDamage(incomingDamage, bat);

            // Asssert
            // HP should be 0, not negative
            Assert.Equal(zeroHP, player.CurrentHP);
        }

        [Fact]
        public void PlayerTakeDamage_NegativeDamage_DoesNotChangeCurrentHP()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster bat = CreateTestBat();
            int incomingDamage = -9;
            int hpBefore = player.CurrentHP;

            // Act
            player.TakePhysicalDamage(incomingDamage, bat);

            // Assert
            Assert.Equal(hpBefore, player.CurrentHP);

        }

        [Fact]
        public void PlayerTakeDamage_DefenseFormula_CorrectDamageTaken()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster bat = CreateTestBat();
            int incomingDamage = 10;
            int expectedRemainingHP = 291;

            // Act
            player.TakePhysicalDamage(incomingDamage, bat);

            // Assert
            Assert.Equal(expectedRemainingHP, player.CurrentHP);
        }

        /* ~~~~~~~~~~~ Section: HP ~~~~~~~~~~~ */

        [Fact]
        public void PlayerHealHP_Heal_CorrectAmount()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster bat = CreateTestBat();
            int incomingDamage = 125;
            int expectedRemainingHP = 251;

            // Act
            player.TakePhysicalDamage(incomingDamage, bat);
            player.CastSpell("Cure", player, bat);

            // Assert
            Assert.Equal(expectedRemainingHP, player.CurrentHP);
        }

        [Fact]
        public void Player_CreateInstance_CurrentHPMatchesMaxHP()
        {
            // Arrange + Act
            Player player = CreateTestPlayer();
            int expectedCurrentHP = 300;
            int expectedMaxHP = 300;

            // Assert
            Assert.Equal(player.CurrentHP, player.MaxHP);
            Assert.Equal(expectedCurrentHP, player.CurrentHP);
            Assert.Equal(expectedMaxHP, player.MaxHP);
        }

        [Fact]
        public void PlayerHealHP_HealMoreThanMaxHP_CurrentHPMatchesMaxHP()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster bat = CreateTestBat();
            int expectedCurrentHP = 300;
            int expectedMaxHP = 300;
            int incomingDamage = 50;

            // Reduce HP first
            player.TakePhysicalDamage(incomingDamage, bat);

            // Act
            player.CastSpell("Cure", player, bat);

            // Assert
            Assert.Equal(expectedCurrentHP, expectedMaxHP);
        }

        [Fact]
        public void PlayerLevelUp_MaxHP_Increase()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster bat = CreateTestBat();
            int expectedMaxHP = 350;
            int incomingDamage = 50;
            int incomingExp = 10;
            int expectedLevel = 2;
            player.TakePhysicalDamage(incomingDamage, bat); // [finalDamage] = [41]

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
            Player player = CreateTestPlayer();
            Monster bat = CreateTestBat();
            int expectedCurrentHP = 259;
            int expectedMaxHP = 350;
            int incomingDamage = 50;
            int incomingExp = 10;
            player.TakePhysicalDamage(incomingDamage, bat); // [finalDamage] = [41]

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
            Player player = CreateTestPlayer();
            Monster bat = CreateTestBat();
            int incomingDamage = 63; // Final Damage: 51

            player.TakePhysicalDamage(incomingDamage, bat);

            // Act
            player.CastSpell("Cure", player, bat);

            // Assert
            Assert.Equal(player.MaxHP, player.CurrentHP);
        }

        [Fact]
        public void Player_InstantiateWithCustomMaxHP_InstanceHasCustomMaxHP()
        {
            // Arrange + Act
            Stats stats = new(29, 52, 35, 36);
            int customMaxHP = 200;
            Player player = new("Freya", stats, customMaxHP);

            // Assert
            Assert.Equal(customMaxHP, player.MaxHP);
            Assert.Equal(customMaxHP, player.CurrentHP);
        }

        /* ~~~~~~~~~~~ Section: Magic ~~~~~~~~~~~ */

        [Fact]
        public void Player_Initialized_KnowsOnlyCure()
        {
            // Arrange + Act
            Player player = CreateTestPlayer();

            // Assert
            Assert.Contains(SpellBook.Cure, player.KnownSpells);
            Assert.Contains(SpellBook.Fire, player.KnownSpells);
        }


        /* ~~~~~~~~~~~ Section: Text ~~~~~~~~~~~ */

        [Fact]
        public void PlayerTakeDamage_ReturnMessage_IsCorrect()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster wolf = CreateTestWolf();
            double incomingDamage = 8;


            var (finalDamage, damageMessage) = player.TakePhysicalDamage(incomingDamage, wolf);

            string expectedMessage =
            $"\n{wolf.Name} attacks {player.Name} for {finalDamage} damage!\n" +
            $"{player.Name}'s HP: {player.CurrentHP}/{player.MaxHP}\n" +
            $"{wolf.Name}'s HP: {wolf.CurrentHP}/{wolf.MaxHP}";

            // Act
            player.TakePhysicalDamage(incomingDamage, wolf);

            // Assert
            Assert.Equal(expectedMessage, damageMessage);
        }

        [Fact]
        public void PlayerCastSpell_ReturnMessage_IsCorrect()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster wolf = CreateTestWolf();

            // Act
            var (EffectValue, healMessage) = player.CastSpell("Cure", player, wolf);

            string expectedMessage =
                $"\n{player.Name} casts Cure and heals {EffectValue} HP!\n" +
                $"{player.Name}'s HP: {player.CurrentHP}/{player.MaxHP}\n" +
                $"{wolf.Name}'s HP: {wolf.CurrentHP}/{wolf.MaxHP}\n";

            // Assert
            Assert.Equal(expectedMessage, healMessage);
        }

        [Fact]
        public void PlayerKillMonster_ReturnMessage_IsCorrect()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster wolf = CreateTestWolf();

            // Act
            string expectedMessage = $"{player.Name} defeated {wolf.Name}!\n";
            expectedMessage +=
            "\n~~~~~~~~~~~~~~~~~~~~~~~~~\n" +
            $"\n{player.Name} gained 7 EXP!\n";
            expectedMessage += $"Current EXP: 7\n" +
            "\n~~~~~~~~~~~~~~~~~~~~~~~~~";

            string actualMessage = player.KillMonster(wolf);

            // Assert
            Assert.Equal(expectedMessage, actualMessage);
        }
    }
}