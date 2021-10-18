using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TrexGame.Entities
{
    public class SkyManager : IGameEntity, IDayNightCycle
    {
        private const int CloudDrawOrder = -1;
        private const int MoonDrawOrder = -2;
        private const int StarDrawOrder = -3;

        private const int CloudMinPosY = 20;
        private const int CloudMaxPosY = 70;
        private const int CloudMinDistance = 120;
        private const int CloudMaxDistance = 400;

        private const int StarMinPosY = 10;
        private const int StarMaxPosY = 60;
        private const int StarMinDistance = 120;
        private const int StarMaxDistance = 380;

        private const int MoonPosY = 16;

        private const int NightTimeScore = 500;
        private const int NightTimeDurationScore = 250;
        private const float TransitionDuration = 1f;
        private float _normalizedScreenColor = 1f;
        private int _previousScore;
        private int _nightTimeStartScore;
        private bool _isTransitioningToNight = false;
        private bool _isTransitioningToDay = false;

        private readonly EntityManager _entityManager;
        private readonly ScoreBoard _scoreBoard;
        private readonly Trex _trex;
        private readonly Texture2D _spriteSheet;
        private Moon _moon;

        private int _targetCloudDistance;
        private int _targetStarDistance;

        private Random _random;

        private Color[] _textureData;

        public int DrawOrder => 0;

        public int NightCount { get; set; }

        public bool IsNight => _normalizedScreenColor < 0.5f;

        public Color ClearColor => new Color(_normalizedScreenColor, _normalizedScreenColor, _normalizedScreenColor);

        public SkyManager(Trex trex, Texture2D spriteSheet, Texture2D invertedSpriteSheet, EntityManager entityManager, ScoreBoard scoreBoard)
        {
            _entityManager = entityManager;
            _scoreBoard = scoreBoard;
            _random = new Random();
            _spriteSheet = spriteSheet;
            _textureData = new Color[_spriteSheet.Width * _spriteSheet.Height];
            _spriteSheet.GetData(_textureData);
            //Debug.WriteLine(_textureData[100].R);
            //_invertedSpriteSheet = invertedSpriteSheet;
            //_invertedSpriteSheet.GetData(_invertedTextureData);
            _trex = trex;
            _moon = new Moon(this, spriteSheet, trex, new Vector2(TrexRunnerGame.WindowWidth, MoonPosY));
            _moon.DrawOrder = MoonDrawOrder;
            entityManager.AddEntity(_moon);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            
        }

        public void Update(GameTime gameTime)
        {
            // Spawn clouds.
            IEnumerable<Cloud> clouds = _entityManager.GetEntitiesOfType<Cloud>();

            if (clouds.Count() <= 0 || TrexRunnerGame.WindowWidth - clouds.Max(c => c.Position.X) >= _targetCloudDistance)
            {
                _targetCloudDistance = _random.Next(CloudMinDistance, CloudMaxDistance + 1);
                int posY = _random.Next(CloudMinPosY, CloudMaxPosY + 1);
                Cloud cloud = new Cloud(_spriteSheet, _trex, new Vector2(TrexRunnerGame.WindowWidth, posY));
                cloud.DrawOrder = CloudDrawOrder;
                _entityManager.AddEntity(cloud);
            }


            // Spawn stars.
            IEnumerable<Star> stars = _entityManager.GetEntitiesOfType<Star>();

            if (stars.Count() <= 0 || TrexRunnerGame.WindowWidth - stars.Max(s => s.Position.X) >= _targetStarDistance)
            {
                _targetStarDistance = _random.Next(StarMinDistance, StarMaxDistance + 1);
                int posY = _random.Next(StarMinPosY, StarMaxPosY + 1);
                Star star = new Star(this, _spriteSheet, _trex, new Vector2(TrexRunnerGame.WindowWidth, posY));
                star.DrawOrder = StarDrawOrder;
                _entityManager.AddEntity(star);
            }


            foreach (SkyObject skyObject in _entityManager.GetEntitiesOfType<SkyObject>().Where(s => s.Position.X < -200))
            {
                if ((skyObject is Moon))
                {
                    _moon.Position = new Vector2(TrexRunnerGame.WindowWidth, MoonPosY);
                }
                else
                    _entityManager.RemoveEntity(skyObject);
            }

            if (_previousScore != 0 && _previousScore < _scoreBoard.DisplayScore && _previousScore / NightTimeScore != _scoreBoard.DisplayScore / NightTimeScore && (!IsNight && !_isTransitioningToNight))
            {
                // Transition to night time.
                _nightTimeStartScore = _scoreBoard.DisplayScore;
                _isTransitioningToDay = false;
                _isTransitioningToNight = true;
                _normalizedScreenColor = 1f;
                NightCount++;
            }
            if ((_scoreBoard.DisplayScore - _nightTimeStartScore >= NightTimeDurationScore && (IsNight && !_isTransitioningToDay)))
            {
                // Transition back to day time.
                _isTransitioningToNight = false;
                _isTransitioningToDay = true;
                _normalizedScreenColor = 0f;
            }
            if (_scoreBoard.DisplayScore < NightTimeScore && (IsNight || _isTransitioningToNight))
            {
                _normalizedScreenColor = 1f;
            }

            // Updates transition.
            if (_isTransitioningToNight)
            {
                // Updates transitions to night.
                _normalizedScreenColor -= (float)gameTime.ElapsedGameTime.TotalSeconds / TransitionDuration;
                UpdateTextureData();
            }
            if (_isTransitioningToDay)
            {
                // Updates transitions to day.
                _normalizedScreenColor += (float)gameTime.ElapsedGameTime.TotalSeconds / TransitionDuration;
                UpdateTextureData();
            }
            _normalizedScreenColor = Math.Clamp(_normalizedScreenColor, 0f, 1f);

            _previousScore = _scoreBoard.DisplayScore;
        }

        private void UpdateTextureData()
        {
            Color[] textureData = _textureData.Select(
                c => c == Color.Transparent ? c : new Color(
                    c.G / 255f * _normalizedScreenColor + (1f - c.G / 255f) * (1 - _normalizedScreenColor),
                    c.G / 255f * _normalizedScreenColor + (1f - c.G / 255f) * (1 - _normalizedScreenColor),
                    c.B / 255f * _normalizedScreenColor + (1f - c.B / 255f) * (1 - _normalizedScreenColor),
                    c.A / 255f
                )
            ).ToArray();
            _spriteSheet.SetData(textureData);
        }
    }
}
