using Microsoft.Xna.Framework.Graphics;

namespace Suburbs;

public class Sprite
{
    public Texture2D Texture { get; private set; }
    public Rectangle? Source { get; private set; }

    public readonly Vector2 Origin;
    public float Rotation;
    public float Scale;

    public Sprite(Texture2D texture, Rectangle? sourceRect = null)
    {
        Texture = texture;
        Source = sourceRect;

        Origin = Vector2.Zero;
        Rotation = 0f;
        Scale = 1f;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 worldPos)
    {
        spriteBatch.Draw(Texture, worldPos, Source, Color.White, Rotation, Origin, Scale, 0, 1f);
    }
}
