using FirstDraft;
using Xunit;

namespace FirstDraft.Tests
{
    public class MonsterTests
    {
        /* ~~~~~~~~~~~ Section: Monster Factory ~~~~~~~~~~~ */
        [Fact]
        public void MonsterCreateMonster_CreateMonsterInstance_Correctly()
        {
            // Arrange + Act
            int wolfMonsterID = 2;
            int expectedMaxHP = 150;
            int expectedCurrentHP = 150;
            int expectedExpGiven = 7;
            Monster wolf = MonsterFactory.CreateMonster(wolfMonsterID);

            // Assert
            Assert.Equal("[Monster] Wolf", wolf.Name);
            Assert.Equal(expectedMaxHP, wolf.MaxHP);
            Assert.Equal(expectedCurrentHP, wolf.CurrentHP);
            Assert.Equal(expectedExpGiven, wolf.ExpGiven);
        }

        [Fact]
        public void MonsterCreateMonster_DisplayArgumentException_OnInvalidMonsterID()
        {
            // Arrange
            int invalidMonsterID = 999;

            // Act + Assert
            Assert.Throws<ArgumentException>(() => MonsterFactory.CreateMonster(invalidMonsterID));
        }

        [Fact]
        public void MonsterCreateMonster_DisplaysCorrectMessage_OnInvalidMonsterID()
        {
            // Arrange + Act
            int invalidMonsterID = 999;
            var exception =
            Assert.Throws<ArgumentException>(() => MonsterFactory.CreateMonster(invalidMonsterID));

            // Assert
            Assert.Equal("Invalid MonsterID", exception.Message);
        }


        /* ~~~~~~~~~~~ Section: Damage ~~~~~~~~~~~ */
        [Fact]
        public void MonsterTakeDamage_Over9999_DamageStopsAt9999()
        {
            // Arrange
            int batMonsterID = 1;
            int playerStrengthStat = 29;
            int playerDefenseStat = 52;
            int playerMagicStat = 35;
            int incomingDamage = 999999;
            int maxDamage = 9999;
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);
            Stats playerStats = new(playerStrengthStat, playerDefenseStat, playerMagicStat);
            Player player = new("Freya", playerStats);

            // Act
            var (finalDamage, _) = bat.TakeDamage(incomingDamage, player);
            // Assert
            Assert.Equal(maxDamage, finalDamage);
        }

        [Fact]
        public void MonsterTakeDamage_MonsterTakeDamage_CorrectAmount()
        {
            // Arrange
            int batMonsterID = 1;
            int playerStrengthStat = 29;
            int playerDefenseStat = 52;
            int playerMagicStat = 35;
            int incomingDamage = 5;
            int expectedRemainingHP = 144;
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);
            Stats playerStats = new(playerStrengthStat, playerDefenseStat, playerMagicStat);
            Player player = new("Freya", playerStats);

            // Act
            bat.TakeDamage(incomingDamage, player);

            // Assert
            Assert.Equal(expectedRemainingHP, bat.CurrentHP);
        }

        [Fact]
        public void MonsterTakeDamage_TakingNegativeDamage_DoesNotChangeHP()
        {
            // Arrange
            int batMonsterID = 1;
            int playerStrengthStat = 29;
            int playerDefenseStat = 52;
            int playerMagicStat = 35;
            int incomingDamage = -5;
            int expectedRemainingHP = 150;
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);
            Stats playerStats = new(playerStrengthStat, playerDefenseStat, playerMagicStat);
            Player player = new("Freya", playerStats);

            // Act
            // Negative damage should not reduce HP
            bat.TakeDamage(incomingDamage, player);

            // Assert
            // HP should remain unchanged
            Assert.Equal(expectedRemainingHP, bat.CurrentHP);
        }

        [Fact]
        public void MonsterBattle_MonsterDies_HPReachesZeroExactly()
        {
            // Arrange
            int batMonsterID = 1;
            int playerStrengthStat = 29;
            int playerDefenseStat = 52;
            int playerMagicStat = 35;
            int incomingDamage = 153;
            int zeroHP = 0;
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);
            Stats playerStats = new(playerStrengthStat, playerDefenseStat, playerMagicStat);
            Player player = new("Freya", playerStats);

            // Act
            // More than current HP
            bat.TakeDamage(incomingDamage, player);

            // Assert
            // Should not go negative
            Assert.Equal(zeroHP, bat.CurrentHP);
        }

        [Fact]
        public void MonsterTakeDamage_MultipleHits_HPReducesCorrectly()
        {
            // Arrange
            int wolfMonsterID = 2;
            int playerStrengthStat = 29;
            int playerDefenseStat = 52;
            int playerMagicStat = 35;
            int incomingDamage = 5;
            int expectedRemainingHP = 138; // See [Calculators] for damage total
            Monster wolf = MonsterFactory.CreateMonster(wolfMonsterID);
            Stats playerStats = new(playerStrengthStat, playerDefenseStat, playerMagicStat);
            Player player = new("Freya", playerStats);

            // Act
            wolf.TakeDamage(incomingDamage, player);
            wolf.TakeDamage(incomingDamage, player);

            // Assert
            Assert.Equal(expectedRemainingHP, wolf.CurrentHP);
        }


        /* ~~~~~~~~~~~ Section: HP ~~~~~~~~~~~ */

        [Fact]
        public void Monster_CreateInstance_CurrentHPMatchesMaxHP()
        {
            // Arrange + Act
            int batMonsterID = 1;
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);

            // Assert
            // Starts with full HP
            Assert.Equal(bat.CurrentHP, bat.MaxHP);
        }

        [Fact]
        public void MonsterTakeDamage_TakeMoreDamageThanCurrentHP_CurrentHPDoesNotGoNegative()
        {
            // Arrange
            int wolfMonsterID = 2;
            int playerStrengthStat = 29;
            int playerDefenseStat = 52;
            int playerMagicStat = 35;
            int zeroHP = 0;
            int incomingDamage = 999999;
            Monster wolf = MonsterFactory.CreateMonster(wolfMonsterID);
            Stats playerStats = new(playerStrengthStat, playerDefenseStat, playerMagicStat);
            Player player = new("Freya", playerStats);

            // Act
            wolf.TakeDamage(incomingDamage, player);

            // Assert
            Assert.Equal(zeroHP, wolf.CurrentHP);
        }

        [Fact]
        public void MonsterTakeDamage_NegativeDamage_DoesNotChangeCurrentHP()
        {
            // Arrange
            int wolfMonsterID = 2;
            int playerStrengthStat = 29;
            int playerDefenseStat = 52;
            int playerMagicStat = 35;
            int incomingDamage = -999999;
            int expectedCurrentHP = 150;
            Monster wolf = MonsterFactory.CreateMonster(wolfMonsterID);
            Stats playerStats = new(playerStrengthStat, playerDefenseStat, playerMagicStat);
            Player player = new("Freya", playerStats);

            // Act
            wolf.TakeDamage(incomingDamage, player);

            // Assert
            Assert.Equal(expectedCurrentHP, wolf.CurrentHP);
        }

        /* ~~~~~~~~~~~ Section: EXP ~~~~~~~~~~~ */
        [Fact]
        public void MonsterKillMonster_PlayerKillMonster_PlayerReceivesCorrectExp()
        {
            // Arrange
            int wolfMonsterID = 2;
            int playerStrengthStat = 29;
            int playerDefenseStat = 52;
            int playerMagicStat = 35;
            int expectedExpGiven = 7;
            Monster wolf = MonsterFactory.CreateMonster(wolfMonsterID);
            Stats playerStats = new(playerStrengthStat, playerDefenseStat, playerMagicStat);
            Player player = new("Freya", playerStats);

            // Act
            player.KillMonster(wolf);

            // Assert
            Assert.Equal(expectedExpGiven, player.Experience);
        }

        /* ~~~~~~~~~~~ Section: Text ~~~~~~~~~~~ */
        [Fact]
        public void Monster_Name_FormattedCorrectly()
        {
            // Arrange
            int wolfMonsterID = 2;

            // Act
            Monster wolf = MonsterFactory.CreateMonster(wolfMonsterID);

            // Assert
            Assert.StartsWith("[Monster]", wolf.Name);
        }
    }
}