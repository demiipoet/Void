using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using FirstDraft;
using Xunit;

namespace FirstDraft.Tests;

public class StoryLoaderTests

/* ~~~~~~~~~~~ Section: JSON ~~~~~~~~~~~ */
{
    private const string sampleJson = """
    [
        {
            "Id": "start",
            "Text": "You stand at a crossroads.",
            "IsEnding": false,
            "Choices": [
                {"Description": "Go left", "NextNodeId": "end1"},
                {"Description": "Go right", "NextNodeId": "end2"}
            ]
        },
        {
            "Id": "end1",
            "Text": "You fall into a pit.",
            "IsEnding": true,
            "Choices": []
        },
        {
            "Id": "end2",
            "Text": "You find treasure.",
            "IsEnding": true,
            "Choices": []
        }
    ]
    """;

    [Fact]
    public void LoadStoryFromJsonString_ParsesCorrectly()
    {
        // Arrange + Act
        var rawNodes = JsonSerializer.Deserialize<List<RawStoryNode>>(sampleJson)!;
        var nodeDict = new Dictionary<string, StoryNode>();

        foreach (var raw in rawNodes)
        {
            nodeDict[raw.Id] = new StoryNode(raw.Id, raw.Text, raw.PreBattleText, raw.IsEnding, raw.MonsterID);
        }

        foreach (var raw in rawNodes)
        {
            var node = nodeDict[raw.Id];
            foreach (var choice in raw.Choices)
            {
                node.AddChoice(new StoryChoice(choice.Description, nodeDict[choice.NextNodeId]));
            }
        }
        var start = nodeDict["start"];

        // Assert
        Assert.Equal("You stand at a crossroads.", start.Text);
        Assert.Equal(2, start.Choices.Count);
        Assert.True(nodeDict["end1"].IsEnding);
        Assert.True(nodeDict["end2"].IsEnding);
    }

    [Fact]
    public void StoryNavigation_StopsAtEnding()
    {
        // Arrange + Act
        var rawNodes = JsonSerializer.Deserialize<List<RawStoryNode>>(sampleJson)!;
        var nodeDict = new Dictionary<string, StoryNode>();

        foreach (var raw in rawNodes)
        {
            nodeDict[raw.Id] = new StoryNode(raw.Id, raw.Text, raw.PreBattleText, raw.IsEnding, raw.MonsterID);
        }

        foreach (var raw in rawNodes)
        {
            var node = nodeDict[raw.Id];
            foreach (var choice in raw.Choices)
            {
                node.AddChoice(new StoryChoice(choice.Description, nodeDict[choice.NextNodeId]));
            }
        }

        var current = nodeDict["start"];
        // Simulate the player choosing the second option
        current = current.Choices[1].NextNode;

        // Assert
        Assert.Equal("end2", current.Id);
        Assert.True(current.IsEnding);
    }
}