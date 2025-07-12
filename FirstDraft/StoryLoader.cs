using System.Text.Json;

namespace FirstDraft
{
    public class RawStoryNode
    {
        public required string Id { get; set; }
        public required string MainText { get; set; }
        public string? PreBattleText { get; set; }
        public required List<RawStoryChoice> Choices { get; set; }
        public bool IsEnding { get; set; } = false;
        public int? MonsterID { get; set; }
    }

    public class RawStoryChoice
    {
        public required string Description { get; set; }
        public required string NextNodeId { get; set; }
    }

    public static class StoryLoader
    {
        public static Dictionary<string, StoryNode> LoadStoryFromJson(string filePath)
        {
            string json = File.ReadAllText(filePath);

            var rawNodes = JsonSerializer.Deserialize<List<RawStoryNode>>(json)
                ?? throw new Exception("Failed to load or parse story JSON.");

            // First, convert [RawStoryNode] > [StoryNode] (empty choices fornow)
            var nodeDict = new Dictionary<string, StoryNode>();
            foreach (var raw in rawNodes)
            {
                nodeDict[raw.Id] = new StoryNode(raw.Id, raw.MainText, raw.PreBattleText, raw.IsEnding, raw.MonsterID);
            }

            // Now wire up the choices
            foreach (var raw in rawNodes)
            {
                var currentNode = nodeDict[raw.Id];
                foreach (var rawChoice in raw.Choices)
                {
                    if (nodeDict.TryGetValue(rawChoice.NextNodeId, out var nextNode))
                    {
                        currentNode.AddChoice(new StoryChoice(rawChoice.Description, nextNode));
                    }
                    else
                    {
                        throw new Exception($"Invalid next node ID: {rawChoice.NextNodeId}");
                    }
                }
            }
            return nodeDict;
        }
    }
}
