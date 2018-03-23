using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventCoursing.Entities;
using EventCoursing.Repositories;
using EventCoursing.Services;
using EventCoursingSimple.Entities;

namespace EventCoursingSimple.Repositories
{
    /// <summary>
    /// A basic entity repository that uses Guids for an ID.  Also allows us to submit events
    /// (for single system programs this is fine, for distributed systems we'll need a separate message bus, etc)
    /// TODO enable real caching, snapshots
    /// </summary>
    public class BasicEntityRepository : IEntityRepository<Guid>, IEventReceiver<Guid>
    {
        private readonly IEventStore<Guid> _eventStore;
        private readonly IEventRetriever<Guid> _eventRetriever;

        private readonly Dictionary<Guid, IEntity<Guid>> _entityCache;
        public BasicEntityRepository(IEventStore<Guid> eventStore, IEventRetriever<Guid> eventRetriever)
        {
            _eventStore = eventStore;
            _eventRetriever = eventRetriever;
            
            _entityCache = new Dictionary<Guid, IEntity<Guid>>();
        }

        public Task<TEntityType> CreateEntity<TEntityType>(bool allowSnapshots = true)
            where TEntityType : class, IEntity<Guid>, new()
        {
            return GetEntity<TEntityType>(Guid.NewGuid(), allowSnapshots);
        }
        
        public Task<TEntityType> GetEntity<TEntityType>(Guid entityId, bool allowSnapshots = true) 
            where TEntityType : class, IEntity<Guid>, new()
        {

            var entity = new TEntityType();
            var realEnt = DowncastEnity(entity);
            realEnt.SetId(entityId);

            return RegisterEntity(entity, allowSnapshots);
        }

        public async Task<TEntityType> RegisterEntity<TEntityType>(TEntityType entity, bool allowSnapshots = true) where TEntityType : IEntity<Guid>
        {
            //get all events for our entity
            IEnumerable<IEntityEvent<Guid>> events = null;
            if (entity.Id != Guid.Empty)
            {
                events = await _eventRetriever.GetStreamForEntity(entity.Id);

                events = events.OrderBy(e => e.Timestamp);
            }

            var realEnt = DowncastEnity(entity);
            //set the entity to send new events back to us
            realEnt.SetReceiver(this);
            
            //load the events
            if (events != null)
            {
                foreach (var ev in events)
                {
                    await entity.ApplyEvent(ev);
                }
            }

            //save the entity
            CacheEntity(entity);
            
            return entity;
        }

        private static IRepositoryManagedEntity<Guid> DowncastEnity<TEntityType>(TEntityType entity) where TEntityType : IEntity<Guid>
        {
            if (!(entity is IRepositoryManagedEntity<Guid> realEnt))
            {
                throw new InvalidOperationException("Entities must derive from BaseEntity to use this factory");
            }

            return realEnt;
        }

        private void CacheEntity(IEntity<Guid> entity)
        {
            _entityCache[entity.Id] = entity;
        }

        private IEntity<Guid> GetEntityFromCache(Guid id)
        {
            return _entityCache.ContainsKey(id) ? _entityCache[id] : null;
        }
        
        public async Task<EntityEventResult> AddEvent<TEntityType>(IEntityEvent<Guid> ev) where TEntityType : IEntity<Guid>
        {
            var entity = GetEntityFromCache(ev.EntityId);
            
            var retrieve = entity.ApplyEvent(ev);

            await _eventStore.StoreEvent(ev);

            return await retrieve;
        }
    }
}