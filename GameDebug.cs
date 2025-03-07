using Microsoft.Xna.Framework.Graphics;
using Suburbs.Data;
using Suburbs.Dialogues;

namespace Suburbs;

public static class GameDebug
{
    private static bool _active = false;

    private static Color _debugTextColor = Color.White;
    private static Rectangle _debugUI = Rectangle.Empty;
    private static string _debugText = string.Empty;
    private static Vector2 _debugTextPosition = Vector2.Zero;
    private static SpriteFont _font = GameRoot.DebugFont;

    public static void Initialize()
    {
        Speech.OnDialogueFired += OnDialogueFired;
    }

    public static void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        if (!_active)
            return;

        _debugUI = new((_debugTextPosition - new Vector2(2, 2)).ToPoint(), _font.MeasureString(_debugText).ToPoint());
        spriteBatch.Draw(pixel, _debugUI, new Color(1, 1, 1, 0.34f));
        spriteBatch.DrawString(_font, _debugText, _debugTextPosition, _debugTextColor, 0f, Vector2.Zero, 1, 0, 1f);
    }

    public static void Toggle() => _active = !_active;

    private static void OnDialogueFired(int? id)
    {
        if (id is null)
        {
            _debugText = string.Empty;
            return;
        }

        _debugText = $"Dialogue \"{Speech.CurrentDialogue.DebugName}\" currently displayed.";
        _debugTextPosition = new(GameRoot.WINDOW_WIDTH - _font.MeasureString(_debugText).X, _font.MeasureString(_debugText).Y);
    }
}
