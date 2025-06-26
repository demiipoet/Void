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
            int playerStrength = 29;
            int playerDefense = 52;
            int playerMagicAttack = 35;
            int playerMagicDefense = 36;
            int batMonsterID = 1;
            Stats playerStats = new(playerStrength, playerDefense, playerMagicAttack, playerMagicDefense);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);

            // Arrange
            // Simulate RNG = 5
            int basePlayerDamage = 5 + playerStats.Strength;

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
            int playerStrength = 29;
            int playerDefense = 52;
            int playerMagicAttack = 35;
            int playerMagicDefense = 36;
            int batMonsterID = 1;
            Stats playerStats = new(playerStrength, playerDefense, playerMagicAttack, playerMagicDefense);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);

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
            int playerStrength = 29;
            int playerDefense = 52;
            int playerMagicAttack = 35;
            int playerMagicDefense = 36;
            int batMonsterID = 1;
            Stats playerStats = new(playerStrength, playerDefense, playerMagicAttack, playerMagicDefense);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);

            // Simulates RNG + Strength
            int baseDamage = 100;

            // Act
            int actualDamage = CombatCalculator.CalculatePhysicalDamageToPlayer(player, "D", baseDamage);

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
            int playerStrength = 29;
            int playerDefense = 52;
            int playerMagicAttack = 35;
            int playerMagicDefense = 36;
            int batMonsterID = 1;
            int incomingDamage = 125;
            int expectedRemainingHP = 251; // See [Calculator] for heal amount
            Stats playerStats = new(playerStrength, playerDefense, playerMagicAttack, playerMagicDefense);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);

            // Act
            player.TakePhysicalDamage(incomingDamage, bat); // Simulate prior damage
            var (healAmount, _) = player.CastSpell("Cure", bat, player);

            // Assert
            Assert.Equal(expectedRemainingHP, player.CurrentHP);
            Assert.True(healAmount > 0);
        }

        [Fact]
        public void Battle_PlayerSelectsHealAtFullHP_HealsZero()
        {
            // Arrange
            int playerStrength = 29;
            int playerDefense = 52;
            int playerMagicAttack = 35;
            int playerMagicDefense = 36;
            int wolfHealth = 150;
            int wolfExp = 10;
            int wolfStrength = 6;
            int wolfDefense = 7;
            int wolfMagicAttack = 8;
            int wolfMagicDeffense = 9;

            Stats playerStats = new(playerStrength, playerDefense, playerMagicAttack, playerMagicDefense);
            Player player = new("Freya", playerStats);
            Monster wolf = new("Wolf", wolfHealth, wolfExp, new Stats(wolfStrength, wolfDefense, wolfMagicAttack, wolfMagicDeffense));

            // Act
            var (healAmount, healMessage) = player.CastSpell("Cure", wolf, player);

            // Assert
            Assert.Equal(0, healAmount);
            Assert.Equal(player.MaxHP, player.CurrentHP);
        }
    }
}