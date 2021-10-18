using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace TrexGame.Entities
{
    public class ObstacleManager : IGameEntity
    {
        private static readonly int[] FlyingDinoYPositions = new int[] { 90, 62, 24 };

        private const float MinSpawnDistance = 20;

        private const int MinObstacleDistance = 10;
        private const int MaxObstacleDistance = 50;

        private const int ObstacleDistanceSpeedTolerance = 5;
        private const int LargeCactusPosY = 80;
        private const int SmallCactusPosY = 94;

        private const int ObstacleDrawOrder = 12;
        private const int ObstacleDespawnPosition = -200;

        private const int FlyingDinoSpawnScoreMin = 150;

        private double _lastSpawnScore = -1;
        private double _currentTargetDistance;

        private readonly EntityManager _entityManager;
        private readonly Trex _trex;
        private readonly ScoreBoard _scoreBoard;

        private readonly Random _random;

        private Texture2D _spriteSheet;

        public bool IsEnabled = false;

        public bool CanSpawnObstacles => IsEnabled && _scoreBoard.Score >= MinSpawnDistance;

        public int DrawOrder => 0;

        public ObstacleManager(EntityManager entityManager, Trex trex, ScoreBoard scoreBoard, Texture2D spriteSheet)
        {
            _entityManager = entityManager;
            _trex = trex;
            _scoreBoard = scoreBoard;
            _random = new Random();
            _spriteSheet = spriteSheet;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }

        public void Update(GameTime gameTime)
        {
            if (!IsEnabled) return;

            if (CanSpawnObstacles && (_lastSpawnScore <= 0 || (_scoreBoard.Score - _lastSpawnScore >= _currentTargetDistance)))
            {
                _currentTargetDistance = _random.NextDouble() * (MaxObstacleDistance - MinObstacleDistance) + MinObstacleDistance;
                _currentTargetDistance += (_trex.Speed - Trex.StartSpeed) / (Trex.MaxSpeed - Trex.StartSpeed) * ObstacleDistanceSpeedTolerance;
                _lastSpawnScore = _scoreBoard.Score;

                // Spawn random obstacle.
                float cactusSpawnProbability = _scoreBoard.Score <= FlyingDinoSpawnScoreMin ? 1f : 0.8f;
                Debug.WriteLine(_scoreBoard.Score);

                Obstacle obstacle = null;
                
                if (_random.NextDouble() <= cactusSpawnProbability)
                {
                    CactusGroup.GroupSize randomGroupSize = (CactusGroup.GroupSize)_random.Next((int)CactusGroup.GroupSize.Small, (int)CactusGroup.GroupSize.Large + 1);

                    bool isLarge = _random.NextDouble() > 0.5;
                    float posY = isLarge ? LargeCactusPosY : SmallCactusPosY;

                    obstacle = new CactusGroup(_spriteSheet, isLarge, randomGroupSize, _trex, new Vector2(TrexRunnerGame.WindowWidth, posY));
                }
                else
                {
                    float posY = FlyingDinoYPositions[_random.Next(0, FlyingDinoYPositions.Length)];
                    obstacle = new FlyingDino(_trex, new Vector2(TrexRunnerGame.WindowWidth, posY), _spriteSheet);
                }

                obstacle.DrawOrder = ObstacleDrawOrder;
                _entityManager.AddEntity(obstacle);
            }

            foreach (Obstacle obstacle in _entityManager.GetEntitiesOfType<Obstacle>())
            {
                if (obstacle.Position.X < ObstacleDespawnPosition) _entityManager.RemoveEntity(obstacle);
            }
        }

        public void Reset()
        {
            foreach (Obstacle obstacle in _entityManager.GetEntitiesOfType<Obstacle>())
            {
                _entityManager.RemoveEntity(obstacle);
            }

            _currentTargetDistance = 0;
            _lastSpawnScore = -1;
        }
    }
}
