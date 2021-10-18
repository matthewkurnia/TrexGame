using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrexGame.Entities
{
    public class ScoreBoard : IGameEntity
    {
        private const int CharacterTexturePositionX = 655;
        private const int CharacterTexturePositionY = 0;
        private const int CharacterTextureWidth = 10;
        private const int CharacterTextureHeight = 13;

        private Texture2D _texture;

        private Trex _trex;

        private SoundEffect _scoreSfx;

        private const float FlashAnimationFrameLength = 0.4f;
        private const int FlashAnimationFrameCount = 4;
        private bool _isPlayingFlashAnimation;
        private float _flashAnimationTime;

        public double Score { get; set; }

        public int DisplayScore => Math.Min((int)Math.Floor(Score), 99999);

        public int HighScore { get; set; }

        public bool HasHighScore => HighScore > 0;

        public int DrawOrder => 100;

        public Vector2 Position { get; set; }

        public ScoreBoard(Texture2D texture, Vector2 position, Trex trex, SoundEffect scoreSfx)
        {
            _texture = texture;
            Position = position;
            _trex = trex;
            _scoreSfx = scoreSfx;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!_isPlayingFlashAnimation || (int)(_flashAnimationTime / FlashAnimationFrameLength) % 2 != 0)
            {
                int score = (int)Score;
                if (_isPlayingFlashAnimation) score = score - score % 100;
                DrawScore(spriteBatch, score, Position.X + CharacterTextureWidth * 7);
            }
            if (HasHighScore)
            {
                DrawScore(spriteBatch, HighScore, Position.X);
                spriteBatch.Draw(
                    _texture,
                    new Vector2(Position.X - CharacterTextureWidth * 3, Position.Y),
                    new Rectangle(
                        CharacterTexturePositionX + 10 * CharacterTextureWidth,
                        CharacterTexturePositionY,
                        CharacterTextureWidth * 2,
                        CharacterTextureHeight),
                    Color.White);
            }
        }

        public void Update(GameTime gameTime)
        {
            int oldScore = DisplayScore;
            Score += _trex.Speed * 0.05 * gameTime.ElapsedGameTime.TotalSeconds;
            if (!_isPlayingFlashAnimation && (DisplayScore / 100 != oldScore / 100))
            {
                // Animation starts.
                _scoreSfx.Play();
                _isPlayingFlashAnimation = true;
                _flashAnimationTime = 0;
            }
            if (_isPlayingFlashAnimation)
            {
                _flashAnimationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_flashAnimationTime >= FlashAnimationFrameLength * FlashAnimationFrameCount)
                {
                    // Animation finished.
                    _isPlayingFlashAnimation = false;
                }
            }
        }


        private void DrawScore(SpriteBatch spriteBatch, int score, float StartPosX)
        {
            const int MaxNumberOfDigits = 5;
            int[] digits = new int[MaxNumberOfDigits];

            // Split the score to array of digits.
            for (int i = MaxNumberOfDigits - 1; i >= 0; i--)
            {
                digits[i] = score % 10;
                score /= 10;
            }

            // Draws the score.
            for (int i = 0; i < MaxNumberOfDigits; i++)
            {
                var textureRect = new Rectangle(
                    CharacterTexturePositionX + digits[i] * CharacterTextureWidth,
                    CharacterTexturePositionY,
                    CharacterTextureWidth,
                    CharacterTextureHeight);
                spriteBatch.Draw(_texture, new Vector2(StartPosX + i * CharacterTextureWidth, Position.Y), textureRect, Color.White);
            }
        }
    }
}
