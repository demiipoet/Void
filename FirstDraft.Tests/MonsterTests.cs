using System.Threading.Tasks.Dataflow;
using FirstDraft;
using Microsoft.VisualStudio.TestPlatform.Common.Exceptions;
using Xunit;

namespace FirstDraft.Tests
{
    public class MonsterTests
    {
        /* ~~~~~~~~~~~ Monster Factory ~~~~~~~~~~~ */
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
            Assert.Equal("[Monster] Wolf", wolf.Name);
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
        

         /* ~~~~~~~~~~~ Damage ~~~~~~~~~~~ */
        [Fact]
        public void MonsterTakeDamage_Over9999_DamageStopsAt9999()
        {
            // Arrange
            int batMonsterID = 1;
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int incomingDamage = 999999;
            int maxDamage = 9999;
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
            
            // Act
            var (finalDamage, _) = bat.TakeDamage(incomingDamage, player);
     
            // Assert
            Assert.Equal(maxDamage, finalDamage);
        }

        [Fact]
        public void MonsterTakeDamage_MonsterTakeDamage_CorrectAmount()
        {
            // Arrange
            int batMonsterID = 1;
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int incomingDamage = 5;
            int expectedRemainingHP = 144;
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new ("Freya", playerStats);
            
            // Act
            bat.TakeDamage(incomingDamage, player);

            // Assert
            Assert.Equal(expectedRemainingHP, bat.CurrentHP);
        }

        [Fact]
        public void MonsterTakeDamage_TakingNegativeDamage_DoesNotChangeHP()
        {
            // Arrange
            int batMonsterID = 1;
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int incomingDamage = -5;
            int expectedRemainingHP = 150;
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new ("Freya", playerStats);

            // Act
            // Negative damage should not reduce HP
            bat.TakeDamage(incomingDamage, player);

            // Assert
            // HP should remain unchanged
            Assert.Equal(expectedRemainingHP, bat.CurrentHP);
        }

        [Fact]
        public void MonsterBattle_MonsterDies_HPReachesZeroExactly()
        {
            // Arrange
            // Starts with 20 HP
            int batMonsterID = 1;
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int incomingDamage = 153;
            int zeroHP = 0;
            Monster bat = MonsterFactory.CreateMonster(batMonsterID);
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new ("Freya", playerStats);

            // Act
            // More than current HP
            bat.TakeDamage(incomingDamage, player);

            // Assert
            // Should not go negative
            Assert.Equal(zeroHP, bat.CurrentHP);
        }


        /* ~~~~~~~~~~~ HP ~~~~~~~~~~~ */
        /* --- Temporary Comment --- */
        // [Fact]
        // public void MonsterHealHP_Heal_CorrectAmount()
        // {
        //     // Arrange
        //     // HP: 20
        //     Monster bat = MonsterFactory.CreateMonster(1);
        //     Stats playerStats = new(29, 52, 35);
        //     Player player = new ("Freya", playerStats);

        //     // Act
        //     bat.TakeDamage(100, player);
        //     bat.HealHP(5);

        //     // Assert
        //     Assert.Equal(57, bat.CurrentHP);
        // }

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
            int wolfMonsterID = 2;
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int zeroHP = 0;
            int incomingDamage = 999999;
            Monster wolf = MonsterFactory.CreateMonster(wolfMonsterID);
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new ("Freya", playerStats);
            
            // Act
            wolf.TakeDamage(incomingDamage, player);
            
            // Assert
            Assert.Equal(zeroHP, wolf.CurrentHP);
        }

       [Fact]
       public void MonsterTakeDamage_NegativeDamage_DoesNotChangeCurrentHP()
       {
           // Arrange
            int wolfMonsterID = 2;
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int incomingDamage = -999999;
            int expectedCurrentHP = 150;
            Monster wolf = MonsterFactory.CreateMonster(wolfMonsterID);
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
           Player player = new ("Freya", playerStats);
           
           // Act
           wolf.TakeDamage(incomingDamage, player);
           
           // Assert
           Assert.Equal(expectedCurrentHP, wolf.CurrentHP);
       } 

       /* --- Temporary Comment --- */
    //   [Fact]
    //   public void MonsterHealHP_HealMoreThanMaxHP_CurrentHPMatchesMaxHP()
    //   {
    //       // Arrange
    //       Monster bat = MonsterFactory.CreateMonster(1);
    //       Stats playerStats = new(29, 52, 35);
    //       Player player = new ("Freya", playerStats);

    //       bat.TakeDamage(15, player);
          
    //       // Act
    //       bat.HealHP(99999);
          
    //       // Assert
    //       Assert.Equal(bat.MaxHP, bat.CurrentHP);
    //   } 
        
       /* --- Temporary Comment --- */
    //   [Fact]
    //   public void MonsterHealHP_NegativeHeal_DoesNotChangeCurrentHP()
    //   {
    //       // Arrange
    //       Monster wolf = MonsterFactory.CreateMonster(2);
    //       int hpBefore = wolf.CurrentHP;
          
    //       // Act
    //       wolf.HealHP(999999);
          
    //       // Assert
    //       Assert.Equal(hpBefore, wolf.CurrentHP);
    //   }
      
       /* --- Temporary Comment --- */
    //   [Fact]
    //   public void MonsterHealHP_HealExactlyToMaxHP_CurrentHPMatchesMaxHP()
    //   {
    //       // Arrange
    //       Monster bat = MonsterFactory.CreateMonster(1);
    //       Stats playerStats = new(29, 52, 35);
    //       Player player = new ("Freya", playerStats);

    //       bat.TakeDamage(10, player);
          
    //       // Act
    //       bat.HealHP(11);
          
    //       // Assert
    //       Assert.Equal(150, bat.CurrentHP);
    //   }

        /* ~~~~~~~~~~~ EXP ~~~~~~~~~~~ */
        [Fact]
        public void MonsterKillMonster_PlayerKillMonster_PlayerReceivesCorrectExp()
        {
            // Arrange
            int wolfMonsterID = 2;
            int strengthStat = 29;
            int defenseStat = 52;
            int magicStat = 35;
            int expectedExpGiven = 7;
            Monster wolf = MonsterFactory.CreateMonster(wolfMonsterID);
            Stats playerStats = new(strengthStat, defenseStat, magicStat);
            Player player = new("Freya", playerStats);
             
            // Act
            player.KillMonster(wolf);
            
            // Assert
            Assert.Equal(expectedExpGiven, player.Experience);
        }
    }
}