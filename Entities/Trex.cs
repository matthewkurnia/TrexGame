using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TrexGame.Graphics;

namespace TrexGame.Entities
{
    public class Trex : IGameEntity
    {
        private const int TrexSpriteOriginX = 848;
        private const int TrexSpriteWidth = 44;
        private const int TrexSpriteHeight = 52;
        private const int TrexDuckingSpriteWidth = 59;
        private const int TrexDeadSpriteOriginX = 1068;

        private Sprite _idleSprite;
        private Sprite _deadSprite;

        private SpriteAnimation _blinkAnimation;
        private SpriteAnimation _runAnimation;
        private SpriteAnimation _duckAnimation;

        private Texture2D _spriteSheet;
        private SoundEffect _jumpSound;
        
        private float _verticalVelocity;
        private Vector2 _startPosition;

        public const float StartSpeed = 400f;
        public const float MaxSpeed = 800f;

        private const float Acceleration = 3f;
        private const float Gravity = 1400f;

        public event EventHandler JumpComplete;
        public event EventHandler Died;

        public Sprite Sprite { get; private set; }

        public TrexState State { get; private set; }

        public Vector2 Position { get; set; }

        public bool IsAlive { get; private set; } = true;

        public float Speed { get; private set; } = 0;

        public int DrawOrder { get; set; }

        public Rectangle CollisionBox
        {
            get
            {
                var rect = new Rectangle((int)Position.X, (int)Position.Y, TrexSpriteWidth, TrexSpriteHeight);
                rect.Inflate(-13, -13);

                if (State == TrexState.Ducking)
                {
                    rect.Y += 20;
                    rect.Height -= 20;
                }
                
                return rect;
            }
        }

        public Trex(Texture2D spriteSheet, Vector2 position, SoundEffect jumpSound)
        {
            Position = position;
            State = TrexState.Idle;
            _spriteSheet = spriteSheet;
            _jumpSound = jumpSound;

            _idleSprite = new Sprite(spriteSheet, TrexSpriteOriginX, 0, TrexSpriteWidth, TrexSpriteHeight);
            _deadSprite = new Sprite(spriteSheet, TrexDeadSpriteOriginX, 0, TrexSpriteWidth, TrexSpriteHeight);

            _blinkAnimation = new SpriteAnimation();
            _blinkAnimation.ShouldLoop = false;
            PlayBlinkAnimation();

            _runAnimation = new SpriteAnimation();
            _runAnimation.AddFrame(new Sprite(spriteSheet, TrexSpriteOriginX + TrexSpriteWidth * 2, 0, TrexSpriteWidth, TrexSpriteHeight), 0f);
            _runAnimation.AddFrame(new Sprite(spriteSheet, TrexSpriteOriginX + TrexSpriteWidth * 3, 0, TrexSpriteWidth, TrexSpriteHeight), 0.1f);
            _runAnimation.AddFrame(new Sprite(spriteSheet, TrexSpriteOriginX + TrexSpriteWidth * 3, 0, TrexSpriteWidth, TrexSpriteHeight), 0.2f);
            _runAnimation.Play();

            _duckAnimation = new SpriteAnimation();
            _duckAnimation.AddFrame(new Sprite(spriteSheet, TrexSpriteOriginX + TrexSpriteWidth * 6, 0, TrexDuckingSpriteWidth, TrexSpriteHeight), 0f);
            _duckAnimation.AddFrame(new Sprite(spriteSheet, TrexSpriteOriginX + TrexSpriteWidth * 6 + TrexDuckingSpriteWidth, 0, TrexDuckingSpriteWidth, TrexSpriteHeight), 0.1f);
            _duckAnimation.AddFrame(new Sprite(spriteSheet, TrexSpriteOriginX + TrexSpriteWidth * 6, 0, TrexDuckingSpriteWidth, TrexSpriteHeight), 0.2f);
            _duckAnimation.Play();

            _startPosition = position;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!IsAlive)
            {
                _deadSprite.Draw(spriteBatch, Position);
                return;
            }

            if (State == TrexState.Idle)
            {
                _idleSprite.Draw(spriteBatch, Position);
                _blinkAnimation.Draw(spriteBatch, Position);
            }
            else if (State == TrexState.Jumping || State == TrexState.Falling)
            {
                _idleSprite.Draw(spriteBatch, Position);
            }
            else if (State == TrexState.Running)
            {
                _runAnimation.Draw(spriteBatch, Position);
            }
            else if (State == TrexState.Ducking)
            {
                _duckAnimation.Draw(spriteBatch, Position);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (State == TrexState.Idle)
            {
                _blinkAnimation.Update(gameTime);
                if (!_blinkAnimation.IsPlaying) PlayBlinkAnimation();
            }
            else if (State == TrexState.Jumping || State == TrexState.Falling)
            {
                Position = new Vector2(Position.X, Position.Y + _verticalVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
                _verticalVelocity += Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (State == TrexState.Jumping && _verticalVelocity > 0) State = TrexState.Falling;
                
                if (Position.Y >= _startPosition.Y)
                {
                    Position = new Vector2(Position.X, _startPosition.Y);
                    _verticalVelocity = 0;
                    State = TrexState.Running;
                    OnJumpComplete();
                }
            }
            else if (State == TrexState.Running)
            {
                _runAnimation.Update(gameTime);
            }
            else if (State == TrexState.Ducking)
            {
                _duckAnimation.Update(gameTime);
            }

            if (State != TrexState.Idle)
            {
                Speed += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public void Initialize()
        {
            Position = _startPosition;
            Speed = StartSpeed;
            State = TrexState.Running;
            IsAlive = true;
        }

        private void PlayBlinkAnimation()
        {
            Random random = new Random();
            double blinkTimeStamp = 2f + random.NextDouble() * 8f;
            _blinkAnimation.Clear();
            _blinkAnimation.AddFrame(new Sprite(_spriteSheet, TrexSpriteOriginX, 0, TrexSpriteWidth, TrexSpriteHeight), 0);
            _blinkAnimation.AddFrame(new Sprite(_spriteSheet, TrexSpriteOriginX + TrexSpriteWidth, 0, TrexSpriteWidth, TrexSpriteHeight), (float)blinkTimeStamp);
            _blinkAnimation.AddFrame(new Sprite(_spriteSheet, TrexSpriteOriginX, 0, TrexSpriteWidth, TrexSpriteHeight), (float)blinkTimeStamp + 0.5f);
            _blinkAnimation.Play();

        }

        public bool BeginJump()
        {
            if (State == TrexState.Jumping || State == TrexState.Falling) return false;

            _jumpSound.Play();
            State = TrexState.Jumping;
            _verticalVelocity = -480f;

            return true;
        }

        public bool CancelJump()
        {
            if (State != TrexState.Jumping) return false;

            _verticalVelocity *= 0.4f;
            return true;
        }

        public bool Duck()
        {
            if (State != TrexState.Running) return false;

            State = TrexState.Ducking;
            return true;
        }

        public bool Drop(GameTime gameTime)
        {
            if (!(State == TrexState.Jumping || State == TrexState.Falling)) return false;

            _verticalVelocity += 10000f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            return true;
        }

        public bool Stand()
        {
            if (State != TrexState.Ducking) return false;

            State = TrexState.Running;
            return true;
        }

        protected virtual void OnDied()
        {
            EventHandler handler = Died;
            handler?.Invoke(this, EventArgs.Empty);
        }

        public bool Die()
        {
            if (!IsAlive)
                return false;

            State = TrexState.Idle;
            Speed = 0;
            IsAlive = false;
            OnDied();
            return true;
        }

        protected virtual void OnJumpComplete()
        {
            EventHandler handler = JumpComplete;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }

    public enum TrexState
    {
        Idle,
        Running,
        Jumping,
        Ducking,
        Falling
    }
}
