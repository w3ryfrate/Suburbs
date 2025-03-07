using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Suburbs;

public static class ActorManager
{
    private static readonly List<Actor> _actors = new();
    private static readonly List<Actor> _toRemove = new();

    public static void Add(Actor actor) => _actors.Add(actor);
    public static void Remove(Actor actor) => _toRemove.Add(actor);

    public static void Update()
    {
        if (_toRemove.Count > 0)
        {
            foreach (var actor in _toRemove)
                _actors.Remove(actor);
            _toRemove.Clear();
        }

        for (int i = 0; i < _actors.Count; i++)
        {
            if (_actors[i].Active)
                _actors[i].Update();
        }
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < _actors.Count; i++)
        {
            if (_actors[i].Visible)
                _actors[i].Draw(spriteBatch);
        }
    }
}