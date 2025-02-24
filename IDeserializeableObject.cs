using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Suburbs.Data;

public interface IDeserializeableObject
{
    public int Id { get; init; }

    protected readonly static JsonSerializerOptions SerializeOptions = new()
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    protected static void TryDeserialize<T>(ref T[] data, string filePath)
    {
        try
        {
            data = JsonSerializer.Deserialize<T[]>(File.ReadAllText(filePath), SerializeOptions)
                      ?? throw new Exception("ERROR: Deserialization returned null");
        }
        catch (JsonException ex)
        {
            throw new Exception($"ERROR: Parsing JSON failed: '{filePath}': {ex.Message}");
        }
        catch (IOException ex)
        {
            throw new Exception($"ERROR: Reading file failed: '{filePath}': {ex.Message}");
        }
    }

    protected static T Get<T>(int id, T[] data) where T : IDeserializeableObject
    {
        try
        {
            return data.Single(d => d.Id == id);
        }
        catch (Exception)
        {
            throw new Exception($"ERROR: Failed to return deserialized object of type: {typeof(T).Name}.");
        }
    }
}
