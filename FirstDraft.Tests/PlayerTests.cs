using FirstDraft;
using Xunit;

namespace FirstDraft.Tests
{
    public class PlayerTests
    {
        /* ~~~~~~~~~~~ EXP ~~~~~~~~~~~ */

        // Assuming EXP doesn't cause ding; for that, use [Player_ExpCarriesOver_AfterLevelUp()]
        [Fact]
        public void Player_GainsEXP_Correctly()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Hero", playerStats);
            int expBefore = player.Experience;

            // Act
            // Gain 1 EXP
            player.ExpUp(1);

            // Assert
            Assert.Equal(expBefore + 1, player.Experience);
        }

        [Fact]
        public void Player_ExpCarriesOver_AfterLevelUp() 
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Hero", playerStats);

            // Act
            // 10 should ding, 5 should carry over (for LVL = 1)
            player.ExpUp(15);

            // Assert
            // the leftover EXP should remain
            Assert.Equal(5, player.Experience);
        }

        [Fact]
        public void Player_LevelsUp_MultipleTimes()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Hero", playerStats);

            // Act
            // If math is correct, should level up 3 times with [5] [Experience] leftover
            player.ExpUp(65);

            // Assert
            Assert.Equal(5, player.Experience);
            Assert.Equal(4, player.Level); 
        }

        [Fact]
        public void Player_ExpUpCausesMultipleLevelUps_CorrectHP()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Hero", playerStats);

            // Act
            // Level: 1 > 4, MaxHP: 100 > 250
            player.ExpUp(65);
            
            // Assert
            Assert.Equal(4, player.Level);
            Assert.Equal(250, player.MaxHP);
            Assert.Equal(100, player.CurrentHP);
        }

        [Fact]
        public void Player_LevelsUp_WhenExactExpThreasholdReached()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Hero", playerStats);
            int initialLevel = player.Level;

            // Act
            // Should trigger ding at Level 50 (ding = [Level] * 10)
            player.ExpUp(10);

            // Assert
            Assert.Equal(initialLevel + 1, player.Level);
        }

        [Fact]
        public void Player_CannotGain_NegativeExp()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Hero", playerStats);

            // Act
            player.ExpUp(-9);

            // Assert
            Assert.Equal(0, player.Experience);
        }

        [Fact]
        public void Player_ZeroExperience_DoesNothing()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Hero", playerStats);
            int initialLevel = player.Level;

            // Act
            player.ExpUp(0);

            // Assert
            Assert.Equal(initialLevel, player.Level);
        }
        
        [Fact]
        public void Player_KillMonster_GivesCorrectExp()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Hero", playerStats);
            Monster wolf = MonsterFactory.CreateMonster(2);

            // Act
            player.KillMonster(wolf);

            // Assert
            Assert.Equal(7, player.Experience);
        }

        [Fact]
        public void Player_KillMonster_CausesLevelUp()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Hero", playerStats);
            // Enough to level up from 1 to 2 with 5 leftover
            Monster wolf = new("Wolf", 30, 15, new Stats(3, 3));
            
            // Act
            player.KillMonster(wolf);
            
            // Assert
            Assert.Equal(2, player.Level);
            Assert.Equal(5, player.Experience);
        }

        [Fact]
        public void Player_MultipleKillMonster_CausesMultiplePlayerLevelUPs()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Hero", playerStats);
            Monster bat = MonsterFactory.CreateMonster(1);

            // Act
            player.KillMonster(bat); // Level 1 (0 > 5 EXP)
            player.KillMonster(bat); // Level 1 > 2 ( 5 > 10 EXP)
            player.KillMonster(bat); // Level 2 (0 > 5 EXP)
            player.KillMonster(bat); // Level 2 (5 > 10 EXP)
            player.KillMonster(bat); // Level 2 ( 10 > 15 EXP)
            player.KillMonster(bat); // Level 3 (15 > 20 EXP)

            // Assert
            Assert.Equal(3, player.Level);

        }
        
        /* ~~~~~~~~~~~ Stats ~~~~~~~~~~~ */
        [Fact]
        public void Player_StatsInitialize_Correctly()
        {
            // Arrange + Act
            Stats playerStats = new(29, 52);
            Player player = new("Zazu", playerStats);
            
            // Assert
            Assert.Equal(29, playerStats.Strength);
            Assert.Equal(52, playerStats.Defense);
        }

        [Fact]
        public void Player_LevelUp_CorrectlyIncreaseStats()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new ("Zaza", playerStats);
            
            // Act
            player.ExpUp(15);
            
            // Assert
            Assert.Equal(39, playerStats.Strength);
            Assert.Equal(62, playerStats.Defense);
        }

        /* ~~~~~~~~~~~ Damage ~~~~~~~~~~~ */
        [Fact]
        public void Player_TakesDamage_HPCannotGoBelowZero()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Hero", playerStats);
            Monster bat = MonsterFactory.CreateMonster(1);

            // Act
            // Take more damage than HP
            player.TakeDamage(150, bat);

            // Asssert
            // HP should be 0, not negative
            Assert.Equal(0, player.CurrentHP);
        }
        
        [Fact]
        public void Player_TakeNegativeDamage_DoesNotChangeHP()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Hero", playerStats);
            Monster bat = MonsterFactory.CreateMonster(1);
            int hpBefore = player.CurrentHP;

            // Act
            player.TakeDamage(-9, bat);

            // Assert
            Assert.Equal(hpBefore, player.CurrentHP);

        }

        [Fact]
        public void Player_DefenseFormula_CorrectlyMitigatesDamage()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Hero", playerStats);
            Monster bat = MonsterFactory.CreateMonster(1);
            
            // Act
            player.TakeDamage(10, bat);
            
            // Assert
            Assert.Equal(91, player.CurrentHP);
        }

        /* ~~~~~~~~~~~ HP ~~~~~~~~~~~ */
        [Fact]
        public void Player_InitializesCurrentHP_MatchesMaxHP()
        {
            // Arrange + Act
            Stats playerStats = new(29, 52);
            Player player = new("Hero", playerStats);

            // Assert
            Assert.Equal(player.CurrentHP, player.MaxHP);
            Assert.Equal(100, player.CurrentHP);
            Assert.Equal(100, player.MaxHP);
        }

        [Fact]
        public void Player_HealsCannotExceed_MaxHP()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Hero", playerStats);
            Monster bat = MonsterFactory.CreateMonster(1);

            // Reduce HP first
            player.TakeDamage(50, bat);

            // Act
            // Try to heal more than max HP
            player.HealHP(200);

            // Assert
            // HP should not exceed [MaxHP]
            Assert.Equal(player.MaxHP, player.CurrentHP);
        }

        [Fact]
        public void Player_LevelUp_IncreasesMaxHP()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Hero", playerStats);
            Monster bat = MonsterFactory.CreateMonster(1);
            player.TakeDamage(50, bat); // [finalDamage] = [41]

            // Act
            // Level up player to Lv 2
            player.ExpUp(10);

            // Assert
            Assert.Equal(2, player.Level);
            Assert.Equal(150, player.MaxHP);
            Assert.Equal(59, player.CurrentHP); // [100] - [41]

        }

        [Fact]
        public void Player_NegativeHeal_DoesNotChangeHP() 
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Hero", playerStats);
            int hpBefore = player.CurrentHP;

            // Act
            // Negative heal should be ignored
            player.HealHP(-50);

            // Assert
            // HP should not change
            Assert.Equal(hpBefore, player.CurrentHP);
        }

        [Fact]
        public void Player_Constructor_CustomMaxHP_InitializesCorrectly()
        {
            // Arrange + Act
            Stats playerStats = new(29, 52);
            Player player = new("Hero", playerStats, 200);
            
            // Assert
            Assert.Equal(200, player.MaxHP);
            Assert.Equal(200, player.CurrentHP);
        }
    }
}