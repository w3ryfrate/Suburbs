global using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Suburbs.Data;
using Suburbs.Dialogues;
using System;

namespace Suburbs
{
    public class GameRoot : Game
    {
        public const int WINDOW_WIDTH = 1280;
        public const int WINDOW_HEIGHT = 750;

        public static SpriteBatch SpriteBatch { get; private set; }
        public static GameTime Time { get; private set; }
        public static SpriteFont StandardRegularFont { get; private set; }
        public static SpriteFont DebugFont { get; private set; }
        public static readonly Random RNG = new();

        private readonly GraphicsDeviceManager _graphics;

        private bool _debugEnabled = false;
        private Color _debugTextColor = Color.White;
        private string _debugText = string.Empty;
        private Vector2 _debugTextPosition = Vector2.Zero;

        private Texture2D _textBox;
        private Texture2D _nameBox;
        private Vector2 _textBoxPosition;

        private Color _clearColor = Color.CornflowerBlue;

        public GameRoot()
        {
            _graphics = new(this)
            {
                SynchronizeWithVerticalRetrace = true,
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            _graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            _graphics.ApplyChanges();

            Speech.OnDialogueFired += OnDialogue;
            Input.OnKeyPressed += OnKeyInput;

            base.Initialize();

            _textBoxPosition = new(30, WINDOW_HEIGHT - 200);
            Speech.FireDialogue(Dialogue.INTRO_DEV);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            StandardRegularFont = Content.Load<SpriteFont>("Fonts\\standard_font");
            DebugFont = Content.Load<SpriteFont>("Fonts\\debugFont");

            _textBox = Content.Load<Texture2D>("Textures\\UI\\textbox-0");
            _nameBox = Content.Load<Texture2D>("Textures\\UI\\namebox-0");

            Voice.LoadFromFile(Content);
            Dialogue.LoadFromFile();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed)
                Speech.NextLine();

            Time = gameTime;
            Input.Update();
            Speech.Update(Time);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_clearColor);

            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            if (Speech.CurrentDialogue is not null)
            {
                if (Speech.SpeakerName is not null)
                {
                    Vector2 pos = new(_textBoxPosition.X + 25f, _textBoxPosition.Y - _nameBox.Height);
                    SpriteBatch.Draw(_nameBox, pos, Color.White);
                    SpriteBatch.DrawString(StandardRegularFont, Speech.SpeakerName, pos + new Vector2(_nameBox.Width / 2, _nameBox.Height / 2), Color.White, 0f, StandardRegularFont.MeasureString(Speech.SpeakerName) / 2, DynamicText.Scale, 0, 1f);
                }
                SpriteBatch.Draw(_textBox, _textBoxPosition, Color.White);
            }
            Speech.Draw();
            if (_debugEnabled)
            {
                SpriteBatch.DrawString(DebugFont, _debugText, _debugTextPosition, _debugTextColor, 0f, Vector2.Zero, 1, 0, 1f);
            }

            base.Draw(gameTime);
            SpriteBatch.End();

        }

        private void OnDialogue(int? dialogueId)
        {
            if (dialogueId is null)
            {
                _debugText = string.Empty;
                return;
            }

            _debugText = $"Dialogue \"{Dialogue.Get(dialogueId.Value).DebugName}\" currently displayed.";
            _debugTextPosition = new(WINDOW_WIDTH - DebugFont.MeasureString(_debugText).X, DebugFont.MeasureString(_debugText).Y);
        }

        private void OnKeyInput(Keys key)
        {
            switch (key)
            {
                case Keys.Z:
                case Keys.Enter:
                    Speech.NextLine();
                    break;

                case Keys.X:
                    Speech.SkipAnimation();
                    break;

                case Keys.Tab:
                    _debugEnabled = !_debugEnabled;
                    break;
            }
        }
    }
}
