using Xunit;
using FirstDraft;

namespace FirstDraft.Tests
{
    public class BattleTests
    {
        /* ~~~~~~~~~~~ Section: Attack ~~~~~~~~~~~ */
        [Fact]
        public void Battle_PlayerAttacksMonster_CorrectDamageDealt()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int batMonsterID = 1;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);

            // Arrange
            // Simulate RNG = 5
            int basePlayerDamage = 5 + playerStats.Strength;
            int expectedDamage = CombatCalculator.CalculateDamageToMonster(bat, basePlayerDamage);
            int expectedRemainingHP = bat.MaxHP - expectedDamage;

            // Act
            var (actualDamage, _) = bat.TakeDamage(basePlayerDamage, player);

            // Assert
            Assert.Equal(expectedDamage, actualDamage);
            Assert.Equal(expectedRemainingHP, bat.CurrentHP);
        }

        [Fact]
        public void Battle_MonsterAttacksPlayer_CorrectDamageDealt()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int batMonsterID = 1;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);

            // Arrange
            int basePlayerDamage = bat.BaseStats.Strength;
            int expectedDamage = CombatCalculator.CalculateDamageToPlayer(player, "A", basePlayerDamage);
            int expectedRemainingHP = player.MaxHP - expectedDamage;

            // Act
            var (actualDamage, _) = player.TakeDamage(basePlayerDamage, bat);

            // Assert
            Assert.Equal(expectedDamage, actualDamage);
            Assert.Equal(expectedRemainingHP, player.CurrentHP);
        }

        /* ~~~~~~~~~~~ Section: Defend ~~~~~~~~~~~ */
        [Fact]
        public void Battle_PlayerSelectsDefend_CorrectDamageReceived()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int batMonsterID = 1;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);

            // Simulates RNG + Strength
            int baseDamage = 100;

            // Act
            int actualDamage = CombatCalculator.CalculateDamageToPlayer(player, "D", baseDamage);

            // Expected:
            double expectedReducedDamage = baseDamage / 2.0;
            double mitigationFactor = (255.0 - playerStats.Defense) / 256;
            int expectedFinalDamage = (int)Math.Round(expectedReducedDamage * mitigationFactor + 1);

            // Assert
            Assert.Equal(expectedFinalDamage, actualDamage);
        }

        /* ~~~~~~~~~~~ Section: Heal ~~~~~~~~~~~ */
        [Fact]
        public void Battle_PlayerSelectsHeal_CorrectHPAmountRestored()
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

            player.TakeDamage(incomingDamage, bat); // Simulate prior damage
            player.CastSpell("Cure", bat); // Simulates player selecting ["H"] 

            // Assert
            Assert.Equal(expectedRemainingHP, player.CurrentHP);
        }
    }
}