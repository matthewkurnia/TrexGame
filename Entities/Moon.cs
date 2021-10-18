using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TrexGame.Graphics;

namespace TrexGame.Entities
{
    public class Moon : SkyObject
    {
        private const int SpriteOriginX = 624;
        private const int SpriteOriginY = 2;

        private const int SpriteWidth = 20;
        private const int SpriteHeight = 40;

        private const int SpriteCount = 7;

        private readonly IDayNightCycle _dayNightCycle;
        private Sprite _sprite;

        public override float Speed => _trex.Speed * 0.1f;
        
        public Moon(IDayNightCycle dayNightCycle, Texture2D spriteSheet, Trex trex, Vector2 position) : base(trex, position)
        {
            _dayNightCycle = dayNightCycle;
            _sprite = new Sprite(spriteSheet, SpriteOriginX, SpriteOriginY, SpriteWidth, SpriteHeight);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!_dayNightCycle.IsNight) return;

            UpdateSprite();
            _sprite.Draw(spriteBatch, Position);
        }

        private void UpdateSprite()
        {
            int spriteIndex = _dayNightCycle.NightCount % SpriteCount;
            int spriteWidth = spriteIndex == 3 ? SpriteWidth * 2 : SpriteWidth;

            if (spriteIndex >= 3) spriteIndex ++;

            _sprite.Height = SpriteHeight;
            _sprite.Width = spriteWidth;
            _sprite.X = SpriteOriginX - spriteIndex * SpriteWidth;
            _sprite.Y = SpriteOriginY;
        }
    }
}
