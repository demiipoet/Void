using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using FirstDraft;
using Xunit;

namespace FirstDraft.Tests;

public class StoryLoaderTests

{
    private static StoryNode TraversePath(Dictionary<string, StoryNode> story, string startId, params int[] choiceIndices)
    {
        var current = story[startId];

        foreach (var index in choiceIndices)
        {
            if (index < 0 || index >= current.Choices.Count)
            {
                throw new ArgumentOutOfRangeException($"Choice index {index} is invalid for node {current.Id}.");
            }
            current = current.Choices[index].NextNode;
        }
        return current;
    }

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

    [Fact]
    public void StoryLoader_PathStartToEnd_Works()
    {
        // Arrange
        var story = StoryLoader.LoadStoryFromJson("story.json");

        // Act Path = start > continueHiking > fightBat > deadEnd
        var finalNode = TraversePath(story, "start", 1, 1, 0);

        // Assert
        Assert.Equal("deadEnd", finalNode.Id);
        Assert.True(finalNode.IsEnding);
        Assert.Equal("Your vision fades. Everything goes dark.", finalNode.Text);
    }

    [Fact]
    public void StoryLoader_PathStartToFightWyvern_Works()
    {
        // Arrange
        var story = StoryLoader.LoadStoryFromJson("story.json");

        // Act Path = start > fightWyvern
        var finalNode = TraversePath(story, "start", 0);

        // Assert
        Assert.Equal("fightWyvern", finalNode.Id);
        Assert.True(finalNode.IsEnding);
        Assert.Equal("Freya lives to fight another day.", finalNode.Text);
    }

    [Fact]
    public void StoryLoader_PathStartToLight_Works()
    {
        // Arrange
        var story = StoryLoader.LoadStoryFromJson("story.json");

        // Act Path = start > fightWyvern
        var finalNode = TraversePath(story, "start", 1, 1, 1);

        // Assert
        Assert.Equal("light", finalNode.Id);
        Assert.True(finalNode.IsEnding);
        Assert.Equal("Freya sees a light in the distance.", finalNode.Text);
    }

    [Fact]
    public void StoryLoader_PathStopsAtFindSapphires_Works()
    {
        // Arrange
        var story = StoryLoader.LoadStoryFromJson("story.json");

        // Act Path = start > fightWyvern
        var finalNode = TraversePath(story, "start", 1, 0);

        // Assert
        Assert.Equal("findSapphires", finalNode.Id);
        Assert.False(finalNode.IsEnding);
        Assert.Equal("Freya finds a cluster of blue sapphires, then feels a precense above and below her.", finalNode.Text);
    }

}