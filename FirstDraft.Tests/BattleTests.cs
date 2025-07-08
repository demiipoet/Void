using Xunit;
using FirstDraft;

namespace FirstDraft.Tests
{
    public class BattleTests
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

        /* ~~~~~~~~~~~ Section: Attack ~~~~~~~~~~~ */
        [Fact]
        public void Battle_PlayerAttacksMonster_CorrectDamageDealt()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster bat = CreateTestBat();

            int basePlayerDamage = 5 + player.BaseStats.Strength; // Simulate RNG = 5

            int expectedDamage = CombatCalculator.CalculatePhysicalDamageToMonster(bat, basePlayerDamage);
            int expectedRemainingHP = bat.MaxHP - expectedDamage;

            // Act
            var (actualDamage, _) = bat.TakePhysicalDamage(basePlayerDamage, player);

            // Assert
            Assert.Equal(expectedDamage, actualDamage);
            Assert.Equal(expectedRemainingHP, bat.CurrentHP);
        }

        [Fact]
        public void Battle_MonsterAttacksPlayer_CorrectDamageDealt()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster bat = CreateTestBat();

            // Arrange
            int basePlayerDamage = bat.BaseStats.Strength;
            int expectedDamage = CombatCalculator.CalculatePhysicalDamageToPlayer(player, "A", basePlayerDamage);
            int expectedRemainingHP = player.MaxHP - expectedDamage;

            // Act
            var (actualDamage, _) = player.TakePhysicalDamage(basePlayerDamage, bat);

            // Assert
            Assert.Equal(expectedDamage, actualDamage);
            Assert.Equal(expectedRemainingHP, player.CurrentHP);
        }

        /* ~~~~~~~~~~~ Section: Defend ~~~~~~~~~~~ */
        [Fact]
        public void Battle_PlayerSelectsDefend_CorrectDamageReceived()
        {
            // Arrange
            Player player = CreateTestPlayer();

            int baseDamage = 100; // Simulates RNG + Strength

            // Act
            int actualDamage = CombatCalculator.CalculatePhysicalDamageToPlayer(player, "D", baseDamage);

            double expectedReducedDamage = baseDamage / 2.0;
            double mitigationFactor = (255.0 - player.BaseStats.Defense) / 256;
            int expectedFinalDamage = (int)Math.Round(expectedReducedDamage * mitigationFactor + 1);

            // Assert
            Assert.Equal(expectedFinalDamage, actualDamage);
        }

        /* ~~~~~~~~~~~ Section: Heal ~~~~~~~~~~~ */
        [Fact]
        public void Battle_PlayerSelectsCure_CorrectHPAmountRestored()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster bat = CreateTestBat();
            int incomingDamage = 125;
            int expectedRemainingHP = 251; // See [Calculator] for heal amount

            // Act
            player.TakePhysicalDamage(incomingDamage, bat);
            var (healAmount, _) = player.CastSpell("Cure", player, bat);

            // Assert
            Assert.Equal(expectedRemainingHP, player.CurrentHP);
            Assert.True(healAmount > 0);
        }

        [Fact]
        public void Battle_PlayerSelectsCureAtFullHP_CurrentHPDoesNotGoBeyondMaxHP()
        {
            // Arrange
            Player player = CreateTestPlayer();
            Monster wolf = CreateTestWolf();

            // Act
            player.CastSpell("Cure", player, wolf);

            // Assert
            Assert.Equal(player.CurrentHP, player.MaxHP);
        }
    }
}