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
        public void Monster_MonsterFactory_CreatesCorrectly()
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
        public void Monster_MonsterFactoryDisplaysArgumentException_OnInvalidMonsterID()
        {
            // Arrange + Act + Assert
            Assert.Throws<ArgumentException>(() => MonsterFactory.CreateMonster(999));
        }

        [Fact]
        public void Monster_MonsterFactoryDisplaysCorrectMessage_OnInvalidMonsterID()
        {
            // Arrange + Act
            var exception = 
                Assert.Throws<ArgumentException>(() => MonsterFactory.CreateMonster(999));
            
            // Assert
            Assert.Equal("Invalid MonsterID", exception.Message);
        }
        

         /* ~~~~~~~~~~~ Damage ~~~~~~~~~~~ */
        [Fact]
        public void Monster_TakesDamage_Correctly()
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
        public void Monster_TakesDamageAndHealsCorrectly()
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
        public void Monster_TakingNegativeDamage_DoesNotChangeHP()
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
        public void Monster_Dies_WhenHPReachesZeroExactly()
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
        public void Monster_InitializesCurrentHP_MatchesMaxHP()
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
        public void Monster_TakeDamage_HPCannotGoBelowZero()
        {
            // Arrange
            Monster wolf = MonsterFactory.CreateMonster(2);
            Stats playerStats = new(29, 52);
            Player player = new ("Zaza", playerStats);
            
            // Act
            wolf.TakeDamage(999, player);
            
            // Assert
            Assert.Equal(0, wolf.CurrentHP);
        }

       [Fact]
       public void Monster_TakeNegativeDamage_DoesNotChangeHP()
       {
           // Arrange
           Monster wolf = MonsterFactory.CreateMonster(2);
           Stats playerStats = new(29, 52);
           Player player = new ("Zaza", playerStats);
           
           // Act
           wolf.TakeDamage(-999, player);
           
           // Assert
           Assert.Equal(150, wolf.CurrentHP);
       } 

      [Fact]
      public void Monster_HealsCannotExceed_MaxHP()
      {
          // Arrange
          Monster bat = MonsterFactory.CreateMonster(1);
          Stats playerStats = new(29, 52);
          Player player = new ("Zaza", playerStats);

          bat.TakeDamage(15, player);
          
          // Act
          bat.HealHP(222);
          
          // Assert
          Assert.Equal(bat.MaxHP, bat.CurrentHP);
      } 
        
      [Fact]
      public void Monster_NegativeHeal_DoesNotChangeHP()
      {
          // Arrange
          Monster wolf = MonsterFactory.CreateMonster(2);
          int hpBefore = wolf.CurrentHP;
          
          // Act
          wolf.HealHP(-222);
          
          // Assert
          Assert.Equal(hpBefore, wolf.CurrentHP);
      }
      
      [Fact]
      public void Monster_HealExactly_ToFullHP()
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
        public void Monster_KillMonster_GivesCorrectExp()
        {
            // Arrange
            Stats playerStats = new(29, 52);
            Player player = new("Zaza", playerStats);
            Monster wolf = MonsterFactory.CreateMonster(2);
             
            // Act
            player.KillMonster(wolf);
            
            // Assert
            Assert.Equal(7, player.Experience);
        }
    }
}