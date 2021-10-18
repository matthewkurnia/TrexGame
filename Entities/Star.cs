using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TrexGame.Graphics;

namespace TrexGame.Entities
{
    public class Star : SkyObject
    {
        private const int TextureOriginX = 644;
        private const int TextureOriginY = 2;
        private const int SpriteWidth = 9;
        private const int SpriteHeight = 9;

        private SpriteAnimation _animation;
        private IDayNightCycle _dayNightCycle;

        public override float Speed => _trex.Speed * 0.2f;
        
        public Star(IDayNightCycle dayNightCycle, Texture2D spriteSheet, Trex trex, Vector2 position) : base(trex, position)
        {
            _dayNightCycle = dayNightCycle;
            _animation = SpriteAnimation.CreateSimpleAnimation(
                spriteSheet,
                new Point(TextureOriginX, TextureOriginY),
                SpriteWidth,
                SpriteHeight,
                new Point(0, SpriteHeight),
                3,
                0.4f
            );
            _animation.ShouldLoop = true;
            _animation.Play();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_trex.IsAlive) _animation.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_dayNightCycle.IsNight)
                _animation.Draw(spriteBatch, Position);
        }
    }
}
