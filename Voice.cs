using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using System;
using System.Text.Json.Serialization;

namespace Suburbs.Data;

public class Voice : IDeserializeableObject
{
    public const float COOLDOWN = 1f;
    public const float COOLDOWN_DECREASE = 0.75f;
    public const int NONE = -1;
    public const int BAD_TIME = 0;
    public const int GENERIC_VOICE_1 = 1;

    private static Voice[] _data;  

    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("file")]
    public string FilePath { get; init; }

    public  TimeSpan Duration => _sfx.Duration;

    private SoundEffect _sfx;
    private float _cooldown = COOLDOWN;

    public static Voice Get(int id)
    {
        if (id == NONE) return null;
        return IDeserializeableObject.Get(id, _data);
    }

    public static void LoadFromFile(ContentManager content, string filePath = "Content\\Data\\voices.json")
    {
        IDeserializeableObject.TryDeserialize(ref _data, filePath);
        foreach (Voice voice in _data)
        {
            voice._sfx = content.Load<SoundEffect>(voice.FilePath);
        }
    }

    public void Play(float volume = 1.0f, float pitch = 0f, float pan = 0f)
    {
        _cooldown -= COOLDOWN_DECREASE;
        if (_cooldown > 0) return;

        volume = Math.Clamp(volume, 0f, 1f);
        pitch = Math.Clamp(pitch, -1f, 1f);
        pan = Math.Clamp(pan, -1f, 1f);

        if (!_sfx.Play(volume, pitch, pan) || _sfx is null)
            throw new System.Exception($"ERROR: Failed to play sound effect: {FilePath}");

        _cooldown = COOLDOWN;
    }

}
