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
            int expectedRemainingHP = 251; // See [Calculator] for heal amount
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);

            // Act
            player.TakeDamage(incomingDamage, bat); // Simulate prior damage
            var (healAmount, _) = player.CastSpell("Cure", bat);

            // Assert
            Assert.Equal(expectedRemainingHP, player.CurrentHP);
            Assert.True(healAmount > 0);
        }

        [Fact]
        public void Battle_PlayerSelectsHealAtFullHP_HealsZero()
        {
            // Arrange
            int playerStrengthStat = 29;
            int playerDefenseStat = 52;
            int playerMagicStat = 35;
            int wolfHealth = 150;
            int wolfExp = 10;
            int wolfStrength = 6;
            int wolfDefense = 7;
            int wolfMagic = 8;

            Stats playerStats = new(playerStrengthStat, playerDefenseStat, playerMagicStat);
            Player player = new("Freya", playerStats);
            Monster wolf = new("Wolf", wolfHealth, wolfExp, new Stats(wolfStrength, wolfDefense, wolfMagic));

            // Act
            var (healAmount, healMessage) = player.CastSpell("Cure", wolf);

            // Assert
            Assert.Equal(0, healAmount);
            Assert.Equal(player.MaxHP, player.CurrentHP);
        }
    }
}