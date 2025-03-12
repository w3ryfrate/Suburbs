using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Suburbs.Data;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Suburbs.Map;

public class Tilemap
{
    [JsonPropertyName("height")]
    public int Height { get; init; }

    [JsonPropertyName("width")]
    public int Width { get; init; }

    [JsonPropertyName("infinite")]
    public bool Infinite { get; init; }

    [JsonPropertyName("tilewidth")]
    public int TileWidth { get; init; }

    [JsonPropertyName("tileheight")]
    public int TileHeight { get; init; }

    [JsonPropertyName("layers")]
    public Layer[] Layers { get; init; }

    [JsonPropertyName("tilesets")]
    public Tileset[] Tilesets { get; init; }

    public class Layer
    {
        [JsonPropertyName("id")]
        public int Id { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("data")]
        public int[] Data { get; init; }

        [JsonPropertyName("height")]
        public int Height { get; init; }

        [JsonPropertyName("width")]
        public int Width { get; init; }

        [JsonPropertyName("visible")]
        public bool Visible { get; init; }

        [JsonPropertyName("x")]
        public float X { get; init; }

        [JsonPropertyName("y")]
        public float Y { get; init; }
    }

    public class Tileset
    {
        [JsonPropertyName("firstgid")]
        public int FirstGid { get; init; }

        [JsonPropertyName("source")]
        public string Source { get; init; }
    }
}

public static class TilemapManager
{
    public static Tilemap Load(string path) => IDeserializeableObject.TryDeserializeObject<Tilemap>(path);
    public static Dictionary<string, Texture2D> TilesetsTextures = new();

    public static void LoadAssets(ContentManager content)
    {
        if (TilesetsTextures.Count > 0)
        {
            GameDebug.LogWarning("All the tilesets have already been loaded");
            return;
        }

        // Add more tilesets later!!
        TilesetsTextures.Add("../Textures/Tilesets/Room_Builder.json", content.Load<Texture2D>("Textures\\Tilesets\\Room_Builder"));
    }

    // DRAW THE FUCKEN MAP!!!
    public static void Draw(SpriteBatch spriteBatch, Tilemap map)
    {
        float tileX = 0;
        float tileY = 0;

        foreach (Tilemap.Tileset tileset in map?.Tilesets)
        {
            Texture2D texture = TilesetsTextures[tileset.Source];

            foreach (Tilemap.Layer layer in map.Layers)
            {
                if (layer is null || !layer.Visible || layer.Data.Length == 0)
                    continue;

                for (int i = 0; i < layer.Data.Length; i++)
                {
                    tileX += i * map.TileWidth;
                    if (tileX == map.TileWidth)
                    {
                        tileX = 0;
                        tileY += map.TileHeight;
                    }
                } 
            }
        }
    }
}

