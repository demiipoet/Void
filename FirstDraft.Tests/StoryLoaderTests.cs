using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using FirstDraft;
using Xunit;

namespace FirstDraft.Tests;

public class StoryLoaderTests

/* ~~~~~~~~~~~ Section: JSON ~~~~~~~~~~~ */
{
    [Fact]
    public void StoryLoader_LoadStoryFromFile_ParsesCorrectly()
    {
        // Arrange
        var storyNodes = StoryLoader.LoadStoryFromJson("story.json");

        // Act
        var start = storyNodes["start"];
        var end1 = storyNodes["fightWyvern"];
        var end2 = storyNodes["light"];

        // Assert
        Assert.Equal("Freya finds a creek she doesn't recognize.", start.Text);
        Assert.Equal(2, start.Choices.Count);
        Assert.True(end1.IsEnding);
        Assert.True(end2.IsEnding);
    }

    [Fact]
    public void StoryLoader_StoryNavigation_StopsAtEnding()
    {
        // Arrange
        var storyNodes = StoryLoader.LoadStoryFromJson("story.json");

        // Act
        var current = storyNodes["start"];
        // Simulate the player choosing the second option
        current = current.Choices[0].NextNode;

        // Assert
        Assert.Equal("fightWyvern", current.Id);
        Assert.True(current.IsEnding);
    }

}