using Suburbs.Map;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Suburbs.Data;

public interface IDeserializeableObject
{
    public int Id { get; init; }

    public readonly static JsonSerializerOptions DefaultSerializeOptions = new()
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public static T TryDeserializeObject<T>(string filePath)
    {
        T obj = default;

        try
        {
            obj = JsonSerializer.Deserialize<T>(File.ReadAllText(filePath), DefaultSerializeOptions);
        }
        catch (JsonException)
        {
            GameDebug.LogFatal($"Parsing JSON failed: '{filePath}' with type: {typeof(T).Name}");
            Environment.Exit(1);
        }
        catch (IOException)
        {
            GameDebug.LogFatal($"Reading file failed: '{filePath}' with type: {typeof(T).Name}");
            Environment.Exit(1);
        }

        return obj;
    }

    protected static void TryDeserializeData<T>(ref T[] data, string filePath)
    {
        try
        {
            data = JsonSerializer.Deserialize<T[]>(File.ReadAllText(filePath), DefaultSerializeOptions)
                      ?? throw new Exception("Deserialization returned null");
        }
        catch (JsonException)
        {
            GameDebug.LogFatal($"Parsing JSON failed: '{filePath}' with type: {typeof(T).Name}");
            Environment.Exit(1);
        }
        catch (IOException)
        {
            GameDebug.LogFatal($"Reading file failed: '{filePath}' with type: {typeof(T).Name}");
            Environment.Exit(1);
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
            GameDebug.LogFatal($"Failed to get IDeserializeableObject of type: {typeof(T).Name} with Id: {id}");
            Environment.Exit(1);
            return default;
        }
    }
}
