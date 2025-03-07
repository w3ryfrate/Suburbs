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

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Sprite _textBox;
        private Sprite _nameBox;

        private Texture2D _textBoxTxt;
        private Texture2D _nameBoxTxt;
        private Vector2 _textBoxPosition;

        private Texture2D _pixel;

        private Color _clearColor = Color.LightSkyBlue;

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

            Input.OnKeyPressed += OnKeyInput;

            GameDebug.Initialize();

            base.Initialize();

            _textBoxPosition = new(30, WINDOW_HEIGHT - 200);
            Speech.FireDialogue(Dialogue.INTRO_DEV);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _spriteBatch = new(GraphicsDevice);
            StandardRegularFont = Content.Load<SpriteFont>("Fonts\\standard_font");
            DebugFont = Content.Load<SpriteFont>("Fonts\\debugFont");

            _pixel = new(GraphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.Black });

            _textBoxTxt = Content.Load<Texture2D>("Textures\\UI\\textbox-0");
            _nameBoxTxt = Content.Load<Texture2D>("Textures\\UI\\namebox-0");

            _textBox = new(_textBoxTxt);
            _nameBox = new(_nameBoxTxt);

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
            ActorManager.Update();
            Speech.Update(Time);

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
                    Vector2 pos = new(_textBoxPosition.X + 25f, _textBoxPosition.Y - _nameBox.Texture.Height);
                    _nameBox.Draw(_spriteBatch, pos);
                    _spriteBatch.DrawString(StandardRegularFont, Speech.SpeakerName, pos + new Vector2(_nameBox.Texture.Width / 2, _nameBox.Texture.Height / 2), Color.White, 0f, StandardRegularFont.MeasureString(Speech.SpeakerName) / 2, DynamicText.Scale, 0, 1f);
                }
                _textBox.Draw(_spriteBatch, _textBoxPosition);
            }

            Speech.Draw(_spriteBatch);
            ActorManager.Draw(_spriteBatch);
            GameDebug.Draw(_spriteBatch, _pixel);

            base.Draw(gameTime);
            _spriteBatch.End();
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
                    GameDebug.Toggle();
                    break;
            }
        }
    }
}
