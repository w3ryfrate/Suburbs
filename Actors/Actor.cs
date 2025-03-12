using Microsoft.Xna.Framework.Graphics;

namespace Suburbs;

public class Actor
{
    public bool Active { get; set; } = true;
    public bool Visible { get; set; } = true;

    public Vector2 WorldPosition { get; set; }
    public Sprite Sprite { get; }

    public Actor(Sprite sprite, Vector2 worldPosition)
    {
        ActorManager.Add(this);
        WorldPosition = worldPosition;
        Sprite = sprite;
        if (Sprite is null) GameDebug.LogWarning($"Sprite component of Actor: {typeof(Actor).Name} is null");
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        Sprite?.Draw(spriteBatch, WorldPosition);
    }

    public virtual void Update() { }

    public void Destroy()
    {
        ActorManager.Remove(this);
    }
}
