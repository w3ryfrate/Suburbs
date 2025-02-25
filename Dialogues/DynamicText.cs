using Microsoft.Xna.Framework.Graphics;
using System;

namespace Suburbs.Dialogues;

public class DynamicText
{
    public const float MAX_WIDTH = GameRoot.WINDOW_WIDTH;
    public const float SHAKING_VALUE = 1.15f;
    public static float Scale { get; private set; } = 1.5f;
    public static int Kerning { get; private set; } = 12;

    public string Text = string.Empty;
    public Vector2 Position = Vector2.Zero;
    public TextProperties Properties = TextProperties.Default;
    public readonly Vector2 Size;

    private Color _color;

    public DynamicText(string text, Vector2 position, TextProperties info)
    {
        Text = text;
        Position = position;
        Properties = info;
        _color = info.Color;
        Size = GameRoot.StandardRegularFont.MeasureString(text);
    }

    internal int DrawSpeech(int remainingCharacters)
    {
        int drawnCharacters = 0;

        if (remainingCharacters <= 0)
            return 0;

        float offsetX = 0f;
        float offsetY = 0f;

        if (Properties.ShakingValue > 0f)
        {
            double angle = GameRoot.RNG.NextDouble() * 2 * Math.PI;
            offsetX += (float)Math.Cos(angle) * Properties.ShakingValue;
            offsetY += (float)Math.Sin(angle) * Properties.ShakingValue;
        }

        for (int i = 0; i < Text.Length && i < remainingCharacters; i++)
        {
            string character = Text[i].ToString();

            if (!char.IsLetter(Text[i]))
                Properties.Color = TextProperties.Default.Color;
            else
                Properties.Color = _color;

            GameRoot.SpriteBatch.DrawString(
                GameRoot.StandardRegularFont,
                character,
                new(Position.X + offsetX * Scale, Position.Y + offsetY * Scale),
                Properties.Color,
                0f,
                Vector2.Zero,
                Scale,
                0,
                1f
            );

            offsetX += GameRoot.StandardRegularFont.MeasureString(character).X + GameRoot.StandardRegularFont.Spacing;
            drawnCharacters++;
        }

        return drawnCharacters;
    }
}