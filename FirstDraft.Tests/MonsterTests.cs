using FirstDraft;
using Xunit;

namespace FirstDraft.Tests
{
    public class MonsterTests
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
            Assert.Equal("Wolf", wolf.Name);
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
            Player player = CreateTestPlayer();
            Monster bat = CreateTestBat();
            int incomingDamage = 999999;
            int maxDamage = 9999;

            // Act
            var (finalDamage, _) = bat.TakePhysicalDamage(incomingDamage, player);
            // Assert
            Assert.Equal(maxDamage, finalDamage);
        }

        [Fact]
        public void MonsterTakeDamage_MonsterTakeDamage_CorrectAmount()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster bat = CreateTestBat();
            int incomingDamage = 5;
            int expectedRemainingHP = 144;

            // Act
            bat.TakePhysicalDamage(incomingDamage, player);

            // Assert
            Assert.Equal(expectedRemainingHP, bat.CurrentHP);
        }

        [Fact]
        public void MonsterTakeDamage_TakingNegativeDamage_DoesNotChangeHP()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster bat = CreateTestBat();
            int incomingDamage = -5;
            int expectedRemainingHP = 150;

            // Act
            bat.TakePhysicalDamage(incomingDamage, player); // Negative damage should not reduce HP

            // Assert
            // HP should remain unchanged
            Assert.Equal(expectedRemainingHP, bat.CurrentHP);
        }

        [Fact]
        public void MonsterBattle_MonsterDies_HPReachesZeroExactly()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster bat = CreateTestBat();
            int incomingDamage = 153;
            int zeroHP = 0;

            // Act
            // More than current HP
            bat.TakePhysicalDamage(incomingDamage, player);

            // Assert
            // Should not go negative
            Assert.Equal(zeroHP, bat.CurrentHP);
        }

        [Fact]
        public void MonsterTakeDamage_MultipleHits_HPReducesCorrectly()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster wolf = CreateTestWolf();
            int incomingDamage = 5;
            int expectedRemainingHP = 138; // See [Calculators] for damage total

            // Act
            wolf.TakePhysicalDamage(incomingDamage, player);
            wolf.TakePhysicalDamage(incomingDamage, player);

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
            Player player = CreateTestPlayer();
            Monster wolf = CreateTestWolf();
            int zeroHP = 0;
            int incomingDamage = 999999;

            // Act
            wolf.TakePhysicalDamage(incomingDamage, player);

            // Assert
            Assert.Equal(zeroHP, wolf.CurrentHP);
        }

        [Fact]
        public void MonsterTakeDamage_NegativeDamage_DoesNotChangeCurrentHP()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster wolf = CreateTestWolf();
            int incomingDamage = -999999;
            int expectedCurrentHP = 150;

            // Act
            wolf.TakePhysicalDamage(incomingDamage, player);

            // Assert
            Assert.Equal(expectedCurrentHP, wolf.CurrentHP);
        }

        /* ~~~~~~~~~~~ Section: EXP ~~~~~~~~~~~ */
        [Fact]
        public void MonsterKillMonster_PlayerKillMonster_PlayerReceivesCorrectExp()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster wolf = CreateTestWolf();
            int expectedExpGiven = 7;

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
            Assert.StartsWith("Wolf", wolf.Name);
        }
    }
}