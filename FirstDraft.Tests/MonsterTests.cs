using System.Threading.Tasks.Dataflow;
using FirstDraft;
using Xunit;

// Added the namespace because VSC wanted me to
namespace FirstDraft.Tests
{
    public class MonsterTests
    {
        /* ~~~~~~~~~~~ Monster Factory ~~~~~~~~~~~ */
        [Fact]
        public void MonsterCreateMonster_CreateMonsterInstance_Correctly()
        {
            // Arrange + Act
            Monster wolf = MonsterFactory.CreateMonster(2);
            
            // Assert
            Assert.Equal("[Monster] Wolf", wolf.Name);
            Assert.Equal(150, wolf.MaxHP);
            Assert.Equal(150, wolf.CurrentHP);
            Assert.Equal(7, wolf.ExpGiven);
        }
        
        [Fact]
        public void MonsterCreateMonster_DisplayArgumentException_OnInvalidMonsterID()
        {
            // Arrange + Act + Assert
            Assert.Throws<ArgumentException>(() => MonsterFactory.CreateMonster(99999));
        }

        [Fact]
        public void MonsterCreateMonster_DisplaysCorrectMessage_OnInvalidMonsterID()
        {
            // Arrange + Act
            var exception = 
                Assert.Throws<ArgumentException>(() => MonsterFactory.CreateMonster(99999));
            
            // Assert
            Assert.Equal("Invalid MonsterID", exception.Message);
        }
        

         /* ~~~~~~~~~~~ Damage ~~~~~~~~~~~ */
        [Fact]
        public void MonsterTakeDamage_Over9999_DamageStopsAt9999()
        {
            // Arrange
            Monster bat = MonsterFactory.CreateMonster(1);
            Stats playerStats = new(29, 52);
            Player player = new("Freya", playerStats);
            
            // Act
            var (finalDamage, _) = bat.TakeDamage(999999, player);
            
            // Assert
            Assert.Equal(9999, finalDamage);
        }

        [Fact]
        public void MonsterTakeDamage_MonsterTakeDamage_CorrectAmount()
        {
            // Arrange
            Monster bat = MonsterFactory.CreateMonster(1);
            Stats playerStats = new(29, 52);
            Player player = new ("Zaza", playerStats);
            
            // Act
            bat.TakeDamage(5, player);

            // Assert
            // Expected: 20 - 5 = 15
            Assert.Equal(144, bat.CurrentHP);
        }

        [Fact]
        public void MonsterTakeDamage_TakingNegativeDamage_DoesNotChangeHP()
        {
            // Arrange
            Monster bat = MonsterFactory.CreateMonster(1); 
            Stats playerStats = new(29, 52);
            Player player = new ("Zaza", playerStats);

            // Act
            // Negative damage should not reduce HP
            bat.TakeDamage(-5, player);

            // Assert
            // HP should remain unchanged
            Assert.Equal(150, bat.CurrentHP);
        }

        [Fact]
        public void MonsterBattle_MonsterDies_HPReachesZeroExactly()
        {
            // Arrange
            // Starts with 20 HP
            Monster bat = MonsterFactory.CreateMonster(1);
            Stats playerStats = new(29, 52);
            Player player = new ("Zaza", playerStats);

            // Act
            // More than current HP
            bat.TakeDamage(153, player);

            // Assert
            // Should not go negative
            Assert.Equal(0, bat.CurrentHP);
        }


        /* ~~~~~~~~~~~ HP ~~~~~~~~~~~ */
        [Fact]
        public void MonsterHealHP_Heal_CorrectAmount()
        {
            // Arrange
            // HP: 20
            Monster bat = MonsterFactory.CreateMonster(1);
            Stats playerStats = new(29, 52);
            Player player = new ("Zaza", playerStats);

            // Act
            bat.TakeDamage(10, player);
            bat.HealHP(5);

            // Assert
            Assert.Equal(144, bat.CurrentHP);
        }

        [Fact]
            public void Monster_CreateInstance_CurrentHPMatchesMaxHP()
        {
            // Arrange + Act
            // Starts with 20 HP
            Monster bat = MonsterFactory.CreateMonster(1);

            // Assert
            // Starts with full HP
            Assert.Equal(bat.CurrentHP, bat.MaxHP);
            Assert.Equal(150, bat.CurrentHP);
            Assert.Equal(150, bat.MaxHP);
        }

        [Fact]
        public void MonsterTakeDamage_TakeMoreDamageThanCurrentHP_CurrentHPDoesNotGoNegative()
        {
            // Arrange
            Monster wolf = MonsterFactory.CreateMonster(2);
            Stats playerStats = new(29, 52);
            Player player = new ("Zaza", playerStats);
            
            // Act
            wolf.TakeDamage(99999, player);
            
            // Assert
            Assert.Equal(0, wolf.CurrentHP);
        }

       [Fact]
       public void MonsterTakeDamage_NegativeDamage_DoesNotChangeCurrentHP()
       {
           // Arrange
           Monster wolf = MonsterFactory.CreateMonster(2);
           Stats playerStats = new(29, 52);
           Player player = new ("Zaza", playerStats);
           
           // Act
           wolf.TakeDamage(-99999, player);
           
           // Assert
           Assert.Equal(150, wolf.CurrentHP);
       } 

      [Fact]
      public void MonsterHealHP_HealMoreThanMaxHP_CurrentHPMatchesMaxHP()
      {
          // Arrange
          Monster bat = MonsterFactory.CreateMonster(1);
          Stats playerStats = new(29, 52);
          Player player = new ("Zaza", playerStats);

          bat.TakeDamage(15, player);
          
          // Act
          bat.HealHP(99999);
          
          // Assert
          Assert.Equal(bat.MaxHP, bat.CurrentHP);
      } 
        
      [Fact]
      public void MonsterHealHP_NegativeHeal_DoesNotChangeCurrentHP()
      {
          // Arrange
          Monster wolf = MonsterFactory.CreateMonster(2);
          int hpBefore = wolf.CurrentHP;
          
          // Act
          wolf.HealHP(999999);
          
          // Assert
          Assert.Equal(hpBefore, wolf.CurrentHP);
      }
      
      [Fact]
      public void MonsterHealHP_HealExactlyToMaxHP_CurrentHPMatchesMaxHP()
      {
          // Arrange
          Monster bat = MonsterFactory.CreateMonster(1);
          Stats playerStats = new(29, 52);
          Player player = new ("Zaza", playerStats);

          bat.TakeDamage(10, player);
          
          // Act
          bat.HealHP(11);
          
          // Assert
          Assert.Equal(150, bat.CurrentHP);
      }

        /* ~~~~~~~~~~~ EXP ~~~~~~~~~~~ */
        [Fact]
        public void MonsterKillMonster_PlayerKillMonster_PlayerReceivesCorrectExp()
        {
            // Arrange
            Monster wolf = MonsterFactory.CreateMonster(2);
            Stats playerStats = new(29, 52);
            Player player = new("Zaza", playerStats);
             
            // Act
            player.KillMonster(wolf);
            
            // Assert
            Assert.Equal(7, player.Experience);
        }
    }
}