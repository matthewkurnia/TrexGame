using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TrexGame.Graphics;

namespace TrexGame.Entities
{
    public class Cloud : SkyObject
    {
        private const int TextureOriginX = 87;
        private const int TextureOriginY = 0;
        private const int SpriteWidth = 46;
        private const int SpriteHeight = 17;

        private Sprite _sprite;

        public override float Speed => _trex.Speed * 0.5f;

        public Cloud(Texture2D spriteSheet, Trex trex, Vector2 position) : base(trex, position)
        {
            _sprite = new Sprite(spriteSheet, TextureOriginX, TextureOriginY, SpriteWidth, SpriteHeight);

        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _sprite.Draw(spriteBatch, Position);
        }
    }
}
