using System.Text.Json.Serialization;

namespace Suburbs.Data;

public class Dialogue : IDeserializeableObject
{
    public const int INTRO_DEV = 0;

    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("debugName")]
    public string DebugName { get; init; }

    [JsonPropertyName("text")]
    public string[] Text { get; init; }

    private static Dialogue[] _data;

    public static Dialogue Get(int id) => IDeserializeableObject.Get(id, _data);

    public static void LoadFromFile(string filePath = "Content\\Data\\dialogues.json")
    {
        IDeserializeableObject.TryDeserialize(ref _data, filePath);
    }
}
