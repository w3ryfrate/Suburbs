using System;
using System.Collections.Generic;
using System.Linq;

namespace Suburbs.Dialogues;

public struct TextProperties
{
    public readonly static TextProperties Default = new(Color.White, 0f, 0f, null);

    public string Speaker;
    public Color Color;
    public float Delay;
    public float ShakingValue;

    public TextProperties(Color color, float shakingValue, float delay, string speaker)
    {
        Color = color;
        ShakingValue = shakingValue;
        Speaker = speaker;
    }

    public static TextProperties ParseTags(ref string str)
    {
        TextProperties info = Default;
        List<(int index, int count)> toRemove = [];

        float delay = 0f;
        float shakingValue = 0;
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            char? next = (i != str.Length - 1) ? str[i + 1] : null;

            switch (c)
            {
                case '&':
                    if (next == 'R') info.Color = Color.Red;
                    else if (next == 'G') info.Color = Color.Green;
                    else if (next == 'B') info.Color = Color.Blue;
                    else if (next == 'Y') info.Color = Color.Yellow;
                    else throw new Exception($"ERROR: Color code not recognized: {next}");
                    toRemove.Add((i, 2));
                    break;

                case '$':
                    shakingValue += 0.5f;
                    toRemove.Add((i, 1));
                    break;

                case '%':
                    delay += 0.5f;
                    toRemove.Add((i, 1));
                    break;

                default:
                    continue;
            }
        }

        foreach (var (index, length) in toRemove.OrderByDescending(x => x.index))
        {
            str = str.Remove(index, length);
        }

        info.Delay = delay;
        info.ShakingValue = shakingValue;
        return info;
    }
}
