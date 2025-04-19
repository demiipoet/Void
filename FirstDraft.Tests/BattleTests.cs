using Xunit;
using FirstDraft;

namespace FirstDraft.Tests
{
    public class BattleTests
    {
        [Fact]
        public void Battle_PlayerSelectsAttack_CorrectDamageDealt()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int batMonsterID = 1;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new ("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);
            
            int simulatedRng = 5;
            int expectedRawDamage = simulatedRng + playerStats.Strength;

            // Using the actual [TakeDamage] formula to calculate expected final damage
            double mitigationFactor = (255.0 - bat.BaseStats.Defense) / 256;
            int expectedFinalDamage = (int)Math.Round(expectedRawDamage * mitigationFactor + 1);
            int expectedHPAfterAttack = bat.MaxHP - expectedFinalDamage;
            
            // Act
            var (finalDamage, _) = bat.TakeDamage(expectedRawDamage, player);
            
            // Assert
            Assert.Equal(expectedFinalDamage, finalDamage);
            Assert.Equal(expectedHPAfterAttack, bat.CurrentHP);
            
        }

        [Fact]
        public void Battle_PlayerSelectsDefend_CorrectDamageReceived()
        {
            // Arrange
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int batMonsterID = 1;
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new ("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);
            
            // Act
            
            // Assert
        }
        
        [Fact]
        public void Battle_PlayerSelectsHeal_HealsCorrectAmount()
        {
            // Arrange
            
            // Act
            
            // Assert
        }
    }
}