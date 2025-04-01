using System.Runtime.ConstrainedExecution;
using FirstDraft;
using Xunit;

namespace FirstDraft.Tests
{
    public class PlayerTests
    {
        /* ~~~~~~~~~~~ EXP ~~~~~~~~~~~ */

        [Fact]
        public void PlayerLevelUp_GainMoreThan99Levels_LevelStopsAt99()
        {
            // Arrange
            Stats baseStats = new(29, 52);
            Player player = new("Freya", baseStats);
            
            // Act
            player.ExpUp(99999);
            
            // Assert
            Assert.Equal(99, player.Level);
        }

        // Assuming EXP doesn't cause ding; for that, use [PlayerExpUp_MoreExpThanNeededToLevelUp_ExtraExpCarriesOver]
        [Fact]
        public void PlayerExpUp_GainOneExp_ExperienceIncreasesByOne()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats);
            int expBefore = player.Experience;

            // Act
            // Gain 1 EXP
            player.ExpUp(1);

            // Assert
            Assert.Equal(expBefore + 1, player.Experience);
        }

        [Fact]
        public void PlayerExpUp_MoreExpThanNeededToLevelUp_ExtraExpCarriesOver() 
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats);

            // Act
            // 10 should ding, 5 should carry over (for LVL = 1)
            player.ExpUp(15);

            // Assert
            // the leftover EXP should remain
            Assert.Equal(5, player.Experience);
        }

        [Fact]
        public void PlayerExpUp_GainEnoughExpToLevelUpMultipleTimes_LevelsUpMultipleTimes()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats);

            // Act
            // If math is correct, should level up 3 times with [5] [Experience] leftover
            player.ExpUp(65);

            // Assert
            Assert.Equal(5, player.Experience);
            Assert.Equal(4, player.Level); 
        }

        [Fact]
        public void PlayerExpUp_GainMultipleLevels_CurrentHPRemainsTheSame()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats);

            // Act
            player.ExpUp(65);
            
            // Assert
            Assert.Equal(4, player.Level);
            Assert.Equal(450, player.MaxHP);
            Assert.Equal(300, player.CurrentHP);
        }

        [Fact]
        public void PlayerExpUp_GainExactAmountOfExpToLevelUp_LevelIncreaseByOne()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats);
            int initialLevel = player.Level;

            // Act
            // Should trigger ding at Level 50 (ding = [Level] * 10)
            player.ExpUp(10);

            // Assert
            Assert.Equal(initialLevel + 1, player.Level);
        }

        [Fact]
        public void PlayerExpUp_NegativeExp_ExperienceDoesNotChange()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats);

            // Act
            player.ExpUp(-9);

            // Assert
            Assert.Equal(0, player.Experience);
        }

        [Fact]
        public void PlayerExpUp_ZeroExpGained_ExperienceDoesNotChange()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats);
            int initialLevel = player.Level;

            // Act
            player.ExpUp(0);

            // Assert
            Assert.Equal(initialLevel, player.Level);
        }
        
        [Fact]
        public void PlayerExpUp_MonsterKilled_CorrectExpGained()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats);
            Monster wolf = MonsterFactory.CreateMonster(2);

            // Act
            player.KillMonster(wolf);

            // Assert
            Assert.Equal(7, player.Experience);
        }

        [Fact]
        public void PlayerExpUp_KillMonsterWithEnoughExpToLevelUp_LevelUp()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats);
            // Enough to level up from 1 to 2 with 5 leftover
            Monster wolf = new("Wolf", 30, 15, new Stats(3, 3));
            
            // Act
            player.KillMonster(wolf);
            
            // Assert
            Assert.Equal(2, player.Level);
            Assert.Equal(5, player.Experience);
        }

        [Fact]
        public void PlayerExpUp_KillMultipleMonsters_GainMultipleLevels()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats);
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
        public void Player_InstanceCreated_InitializedStatsAreCorrect()
        {
            // Arrange + Act
            Stats playerStats = new(29, 52);
            Player player = new("Zazu", playerStats);
            
            // Assert
            Assert.Equal(29, playerStats.Strength);
            Assert.Equal(52, playerStats.Defense);
        }

        [Fact]
        public void PlayerExplUp_LevelUp_BaseStatsIncreaseAppropriately()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new ("Freya", playerStats);
            
            // Act
            player.ExpUp(15);
            
            // Assert
            Assert.Equal(39, playerStats.Strength);
            Assert.Equal(62, playerStats.Defense);
        }

        /* ~~~~~~~~~~~ Damage ~~~~~~~~~~~ */
        [Fact]
        public void PlayerTakeDamage_Over9999_DamageStopsAt9999()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new ("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(1);
            
            // Act
            var (finalDamage, _) = player.TakeDamage(999999, bat);
            
            // Assert
            Assert.Equal(9999, finalDamage);
        }

        [Fact]
        public void PlayerTakeDamage_PlayerTakeDamage_CorrectAmount()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new ("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(1);
            
            // Act
            player.TakeDamage(99, bat);

            // Assert
            // HP: 300
            Assert.Equal(220, player.CurrentHP);
        }

        [Fact]
        public void PlayerTakeDamage_TakeMoreDamageThanCurrentHP_CurrentHPDoesNotGoNegative()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(1);

            // Act
            // Take more damage than HP
            player.TakeDamage(99999, bat);

            // Asssert
            // HP should be 0, not negative
            Assert.Equal(0, player.CurrentHP);
        }
        
        [Fact]
        public void PlayerTakeDamage_NegativeDamage_DoesNotChangeCurrentHP()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(1);
            int hpBefore = player.CurrentHP;

            // Act
            player.TakeDamage(-9, bat);

            // Assert
            Assert.Equal(hpBefore, player.CurrentHP);

        }

        [Fact]
        public void PlayerTakeDamage_DefenseFormula_CorrectDamageTaken()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(1);
            
            // Act
            player.TakeDamage(10, bat);
            
            // Assert
            Assert.Equal(291, player.CurrentHP);
        }

        /* ~~~~~~~~~~~ HP ~~~~~~~~~~~ */
        [Fact]
        public void PlayerHealHP_Heal_CorrectAmount()
        {
            // Arrange
            // HP: 20
            Monster bat = MonsterFactory.CreateMonster(1);
            Stats playerStats = new(29, 52);
            Player player = new ("Freya", playerStats);

            // Act
            bat.TakeDamage(10, player);
            bat.HealHP(5);

            // Assert
            Assert.Equal(144, bat.CurrentHP);
        }

        [Fact]
        public void Player_CreateInstance_CurrentHPMatchesMaxHP()
        {
            // Arrange + Act
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats);

            // Assert
            Assert.Equal(player.CurrentHP, player.MaxHP);
            Assert.Equal(300, player.CurrentHP);
            Assert.Equal(300, player.MaxHP);
        }

        [Fact]
        public void PlayerHealHP_HealMoreThanMaxHP_CurrentHPMatchesMaxHP()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(1);

            // Reduce HP first
            player.TakeDamage(50, bat);

            // Act
            player.HealHP(99999);

            // Assert
            Assert.Equal(player.MaxHP, player.CurrentHP);
        }

        [Fact]
        public void PlayerLevelUp_MaxHP_Increase()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats);
            Monster bat = MonsterFactory.CreateMonster(1);
            player.TakeDamage(50, bat); // [finalDamage] = [41]

            // Act
            // Level up player to Lv 2
            player.ExpUp(10);

            // Assert
            Assert.Equal(2, player.Level);
            Assert.Equal(350, player.MaxHP);
            Assert.Equal(259, player.CurrentHP); // [300] - [41]

        }
        [Fact]
        public void PlayerLevelUp_MaxHPIncreasesBeyond9999_MaxHPStopsAt9999()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats);
            
            // Act
            
            // Assert
        }

        [Fact]
        public void PlayerHealHP_HealExactlyToMaxHP_CurrentHPMatchesMaxHP()
        {
            // Arrange
            Monster bat = MonsterFactory.CreateMonster(1);
            Stats playerStats = new(29, 52);
            Player player = new ("Freya", playerStats);

            player.TakeDamage(13, bat);

            // Act
            player.HealHP(11);

            // Assert
            Assert.Equal(300, player.CurrentHP);
        }

        [Fact]
        public void PlayerHealHP_NegativeHeal_DoesNotChangeCurrentHP() 
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats);
            int hpBefore = player.CurrentHP;

            // Act
            // Negative heal should be ignored
            player.HealHP(-50);

            // Assert
            // HP should not change
            Assert.Equal(hpBefore, player.CurrentHP);
        }

        [Fact]
        public void Player_InstantiateWithCustomMaxHP_InstanceHasCustomMaxHP()
        {
            // Arrange + Act
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats, 200);
            
            // Assert
            Assert.Equal(200, player.MaxHP);
            Assert.Equal(200, player.CurrentHP);
        }
    }
}