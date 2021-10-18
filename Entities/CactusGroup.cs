using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrexGame.Graphics;

namespace TrexGame.Entities
{
    public class CactusGroup : Obstacle
    {
        public enum GroupSize
        {
            Small,
            Medium,
            Large
        }

        private const int SmallCactusSpriteHeight = 36;
        private const int SmallCactusSpriteWidth = 17;
        private const int SmallCactusSpriteOriginX = 228;
        private const int SmallCactusSpriteOriginY = 0;

        private const int LargeCactusSpriteHeight = 51;
        private const int LargeCactusSpriteWidth = 25;
        private const int LargeCactusSpriteOriginX = 332;
        private const int LargeCactusSpriteOriginY = 0;

        public override Rectangle CollisionBox => new Rectangle((int)Position.X, (int)Position.Y, Sprite.Width, Sprite.Height);

        public bool IsLarge { get; }
        public GroupSize Size { get; }

        public Sprite Sprite { get; private set; }

        public CactusGroup(Texture2D spriteSheet, bool isLarge, GroupSize size, Trex trex, Vector2 position) : base(trex, position)
        {
            IsLarge = isLarge;
            Size = size;
            Sprite = GenerateSprite(spriteSheet);
        }

        private Sprite GenerateSprite(Texture2D spriteSheet)
        {
            Sprite sprite = null;

            int spriteWidth;
            int spriteHeight;
            int originX;
            int originY;

            if (IsLarge)
            {
                spriteHeight = LargeCactusSpriteHeight;
                spriteWidth = LargeCactusSpriteWidth;
                originX = LargeCactusSpriteOriginX;
                originY = LargeCactusSpriteOriginY;
            }
            else
            {
                spriteHeight = SmallCactusSpriteHeight;
                spriteWidth = SmallCactusSpriteWidth;
                originX = SmallCactusSpriteOriginX;
                originY = SmallCactusSpriteOriginY;
            }

            int offsetX = 0;
            int width = spriteWidth;

            switch (Size)
            {
                case GroupSize.Small:
                    offsetX = 0;
                    width = spriteWidth;
                    break;
                case GroupSize.Medium:
                    offsetX = spriteWidth;
                    width = spriteWidth * 2;
                    break;
                case GroupSize.Large:
                    offsetX = spriteWidth * 3;
                    width = spriteWidth * 3;
                    break;
            }

            sprite = new Sprite
            (
                spriteSheet,
                originX + offsetX,
                originY,
                width,
                spriteHeight
            );

            return sprite;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, Position);
        }
    }
}
