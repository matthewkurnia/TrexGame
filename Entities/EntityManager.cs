using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace TrexGame.Entities
{
    public class EntityManager
    {
        private readonly List<IGameEntity> _entities = new List<IGameEntity>();

        private readonly List<IGameEntity> _cachedEntities = new List<IGameEntity>();
        private readonly List<IGameEntity> _removedEntities = new List<IGameEntity>();

        public IEnumerable<IGameEntity> Entities => new ReadOnlyCollection<IGameEntity>(_entities);

        public void Update(GameTime gameTime)
        {
            foreach (IGameEntity entity in _entities)
            {
                if (_removedEntities.Contains(entity)) continue;

                entity.Update(gameTime);
            }
            foreach (IGameEntity entity in _cachedEntities)
            {
                _entities.Add(entity);
            }
            foreach (IGameEntity entity in _removedEntities)
            {
                _entities.Remove(entity);
            }

            _cachedEntities.Clear();
            _removedEntities.Clear();
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (IGameEntity entity in _entities.OrderBy(e => e.DrawOrder))
            {
                entity.Draw(spriteBatch, gameTime);
            }
        }

        public void AddEntity(IGameEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity), "Null cannot be added as entity");

            _cachedEntities.Add(entity);
        }

        public void RemoveEntity(IGameEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity), "Null cannot be removed");

            _removedEntities.Add(entity);
        }

        public void Clear()
        {
            _removedEntities.AddRange(_entities);
        }

        public IEnumerable<T> GetEntitiesOfType<T>() where T : IGameEntity
        {
            return _entities.OfType<T>();
        }
    }
}
