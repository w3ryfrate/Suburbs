using Microsoft.Xna.Framework.Graphics;
using Suburbs.Dialogues;

namespace Suburbs;

public class Text
{
    public string DisplayedString;
    public TextProperties Properties = TextProperties.Default;
    public readonly SpriteFont Font;

    public Text(SpriteFont font, string displayedString)
    {
        Font = font;
        DisplayedString = displayedString;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        spriteBatch.DrawString(Font, DisplayedString, position, Properties.Color, Properties.Rotation, Properties.Origin, Properties.Scale, 0, 1f);
    }
}
