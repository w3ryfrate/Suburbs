using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Suburbs.Dialogues;

using Serilog;

namespace Suburbs;

public static class GameDebug
{
    private static readonly Serilog.Core.Logger _logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

    private static bool _active = false;

    private static Rectangle _debugUI = Rectangle.Empty;
    private static Text _debugText;
    private static Vector2 _debugTextPosition = Vector2.Zero;
    private static SpriteFont _font;

    public static void Initialize(ContentManager content)
    {
        Speech.OnDialogueFired += OnDialogueFired;
        _font = content.Load<SpriteFont>("Fonts\\debugFont");
        _debugText = new(_font, string.Empty);
    }

    public static void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        if (!_active)
            return;

        _debugUI = new((_debugTextPosition - new Vector2(2, 2)).ToPoint(), _font.MeasureString(_debugText.DisplayedString).ToPoint());
        spriteBatch.Draw(pixel, _debugUI, new Color(1, 1, 1, 0.34f));
        _debugText.Draw(spriteBatch, _debugTextPosition);
    }

    public static void Toggle() => _active = !_active;

    public static void LogInformation(string formattedMsg) => _logger.Information(formattedMsg);
    public static void LogWarning(string formattedMsg) => _logger.Warning(formattedMsg);
    public static void LogError(string formattedMsg) => _logger.Error(formattedMsg);
    public static void LogFatal(string formattedMsg) => _logger.Fatal(formattedMsg);

    private static void OnDialogueFired(int? id)
    {
        if (id is null)
        {
            _debugText.DisplayedString = string.Empty;
            return;
        }

        string dName = Speech.CurrentDialogue.DebugName;
        _debugText.DisplayedString = $"Dialogue \"{dName}\" currently displayed.";
        _debugTextPosition = new(GameRoot.WINDOW_WIDTH - _font.MeasureString(_debugText.DisplayedString).X, _font.MeasureString(_debugText.DisplayedString).Y);
        LogInformation($"Dialogue fired: {dName}");
    }
}
