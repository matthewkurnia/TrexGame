using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TrexGame.Graphics;

namespace TrexGame.Entities
{
    public class GroundTile : IGameEntity
    {
        private float _positionY;
        
        public float PositionX { get; set; }
        public Sprite Sprite { get; set; }

        public int DrawOrder { set; get; }

        public GroundTile(float positionX, float positionY, Sprite sprite)
        {
            PositionX = positionX;
            _positionY = positionY;
            Sprite = sprite;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, new Vector2(PositionX, _positionY));
        }

        public void Update(GameTime gameTime)
        {
        }
    }


    public class GroundManager : IGameEntity
    {
        private const float GroundTilePosY = 119f;

        private const int SpriteWidth = 600;
        private const int SpriteHeight = 14;

        private const int SpritePosX = 2;
        private const int SpritePosY = 54;

        private Texture2D _spriteSheet;
        private readonly EntityManager _entityManager;

        private Sprite[] _groundSprites = new Sprite[2];
        private List<GroundTile> _groundTiles;

        private Trex _trex;

        private Random _random = new Random();

        private GroundTile _newestGroundTile;

        public int DrawOrder { set; get; }

        public GroundManager(Texture2D spriteSheet, EntityManager entityManager, Trex trex)
        {
            _spriteSheet = spriteSheet;
            _groundSprites[0] = new Sprite(spriteSheet, SpritePosX, SpritePosY, SpriteWidth, SpriteHeight);
            _groundSprites[1] = new Sprite(spriteSheet, SpritePosX + SpriteWidth, SpritePosY, SpriteWidth, SpriteHeight);
            _groundTiles = new List<GroundTile>();
            _entityManager = entityManager;
            _trex = trex;
        }

        public void Initialize()
        {
            // Clears the current drawn ground tiles.
            _groundTiles.Clear();
            foreach (GroundTile gt in _entityManager.GetEntitiesOfType<GroundTile>())
            {
                _entityManager.RemoveEntity(gt);
            }

            // Initializes new ground tiles.
            GroundTile groundTile = new GroundTile(0, GroundTilePosY, _groundSprites[0]);
            _groundTiles.Add(groundTile);
            _entityManager.AddEntity(groundTile);
            _newestGroundTile = groundTile;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }

        public void Update(GameTime gameTime)
        {

            List<GroundTile> removedTiles = new List<GroundTile>();

            foreach (GroundTile gt in _groundTiles)
            {
                gt.PositionX -= _trex.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (gt.PositionX < -SpriteWidth)
                {
                    _entityManager.RemoveEntity(gt);
                    removedTiles.Add(gt);
                }

            }
            
            if (_newestGroundTile.PositionX <= 0)
            {
                var newGroundTile = new GroundTile(
                    _newestGroundTile.PositionX + SpriteWidth,
                    GroundTilePosY,
                    _groundSprites[_random.Next(0, 2)]);
                _groundTiles.Add(newGroundTile);
                _entityManager.AddEntity(newGroundTile);
                _newestGroundTile = newGroundTile;
            }

            foreach (GroundTile gt in removedTiles) _groundTiles.Remove(gt);
        }
    }
}
