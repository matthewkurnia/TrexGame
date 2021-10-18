using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using TrexGame.Graphics;

namespace TrexGame.Entities
{
    public class GameOverScreen : IGameEntity
    {
        private const int GameOverSpriteOriginX = 655;
        private const int GameOverSpriteOriginY = 14;
        public const int GameOverSpriteWidth = 192;
        private const int GameOverSpriteHeight = 14;

        private const int ButtonSpriteOriginX = 1;
        private const int ButtonSpriteOriginY = 1;
        private const int ButtonSpriteWidth = 38;
        private const int ButtonSpriteHeight = 34;

        private Sprite _textSprite;
        private Sprite _buttonSprite;

        private KeyboardState _previousKeyboardState;

        private TrexRunnerGame _game;

        public int DrawOrder => 100;

        public Vector2 Position { get; set; }

        public bool IsEnabled { get; set; } = false;

        private Vector2 ButtonPosition => Position + new Vector2((GameOverSpriteWidth - ButtonSpriteWidth) * 0.5f, GameOverSpriteHeight + 20);

        private Rectangle ButtonBounds => new Rectangle(
            (ButtonPosition * _game.ZoomFactor).ToPoint(),
            new Point((int)(ButtonSpriteWidth * _game.ZoomFactor), (int)(ButtonSpriteHeight * _game.ZoomFactor))
        );

        public GameOverScreen(Texture2D spriteSheet, TrexRunnerGame game)
        {
            _textSprite = new Sprite(spriteSheet, GameOverSpriteOriginX, GameOverSpriteOriginY, GameOverSpriteWidth, GameOverSpriteHeight);
            _buttonSprite = new Sprite(spriteSheet, ButtonSpriteOriginX, ButtonSpriteOriginY, ButtonSpriteWidth, ButtonSpriteHeight);
            _game = game;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!IsEnabled)
                return;

            _textSprite.Draw(spriteBatch, Position);
            _buttonSprite.Draw(spriteBatch, ButtonPosition);
        }

        public void Update(GameTime gameTime)
        {
            if (!IsEnabled)
                return;

            var mouseState = Mouse.GetState();
            var keyboardState = Keyboard.GetState();
            bool mouseClicked = ButtonBounds.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed;
            bool keyboardJustPressed = !keyboardState.IsKeyDown(Keys.Up) && _previousKeyboardState.IsKeyDown(Keys.Up);
            if (mouseClicked || keyboardJustPressed)
            {
                _game.Replay();
            }
            _previousKeyboardState = keyboardState;
        }
    }
}
