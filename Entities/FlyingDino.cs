using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TrexGame.Graphics;

namespace TrexGame.Entities
{
    public class FlyingDino : Obstacle
    {
        private const int TextureOriginX = 134;
        private const int TextureOriginY = 0;
        private const int SpriteWidth = 46;
        private const int SpriteHeight = 42;

        private const float Speed = 80;

        private SpriteAnimation _animation;

        private Trex _trex;

        public override Rectangle CollisionBox
        {
            get
            {
                var rect = new Rectangle((int)Position.X, (int)Position.Y, SpriteWidth, SpriteHeight);
                rect.Inflate(-6, -8);
                return rect;
            }
        }

        public FlyingDino(Trex trex, Vector2 position, Texture2D spriteSheet) : base(trex, position)
        {
            _trex = trex;
            _animation = new SpriteAnimation();
            _animation.AddFrame(new Sprite(spriteSheet, TextureOriginX, TextureOriginY, SpriteWidth, SpriteHeight), 0f);
            _animation.AddFrame(new Sprite(spriteSheet, TextureOriginX + SpriteWidth, TextureOriginY, SpriteWidth, SpriteHeight), 0.5f);
            _animation.AddFrame(new Sprite(spriteSheet, TextureOriginX, TextureOriginY, SpriteWidth, SpriteHeight), 1f);
            _animation.ShouldLoop = true;
            _animation.Play();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _animation.Draw(spriteBatch, Position);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_trex.IsAlive)
                Position = new Vector2(Position.X - Speed * (float) gameTime.ElapsedGameTime.TotalSeconds, Position.Y);
                _animation.Update(gameTime);
        }
    }
}
