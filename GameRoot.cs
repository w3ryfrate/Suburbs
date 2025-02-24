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

        public static GameTime Time { get; private set; }
        public static SpriteFont StandardRegularFont { get; private set; }
        public static SpriteFont DebugFont { get; private set; }
        public static readonly Random RNG = new();
        public static readonly int IdentityValue = RNG.Next(101);

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

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
            Input.OnKeyPressed += OnInput;

            base.Initialize();

            _textBoxPosition = new(30, WINDOW_HEIGHT - 200);
            Speech.FireDialogue(Dialogue.INTRO);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
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

            Time = gameTime;
            Input.Update();
            Speech.Update(Time);

            if (IdentityValue == 66 && Speech.CurrentDialogue is null && !Speech.SeenDialogues.Contains(Dialogue.EVENT_66)) {
                Speech.FireDialogue(Dialogue.EVENT_66, 0.01f);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_clearColor);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            if (Speech.CurrentDialogue is not null)
            {
                if (Speech.SpeakerName is not null)
                {
                    Vector2 pos = new(_textBoxPosition.X + 25f, _textBoxPosition.Y - _nameBox.Height);
                    _spriteBatch.Draw(_nameBox, pos, Color.White);
                    _spriteBatch.DrawString(StandardRegularFont, Speech.SpeakerName, pos + new Vector2(_nameBox.Width / 2, _nameBox.Height / 2), Color.White, 0f, StandardRegularFont.MeasureString(Speech.SpeakerName) / 2, DynamicText.Scale, 0, 1f);
                }
                _spriteBatch.Draw(_textBox, _textBoxPosition, Color.White);
            }
            Speech.Draw(_spriteBatch);
            if (_debugEnabled)
            {
                _spriteBatch.DrawString(DebugFont, _debugText, _debugTextPosition, _debugTextColor, 0f, Vector2.Zero, 1, 0, 1f);
                _spriteBatch.DrawString(DebugFont, IdentityValue.ToString(), new(0, _debugTextPosition.Y), _debugTextColor);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
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

        private void OnInput(Keys key)
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
