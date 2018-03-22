using System;
using System.Threading.Tasks;
using EventCoursing.Entities;
using EventCoursing.Services;
using System.Linq;
using EventCoursing.Factories;

namespace EventCoursing.Repositories.Implementation
{
    /// <summary>
    /// A basic entity repository that uses Guids for an ID
    /// TODO enable real caching, snapshots
    /// </summary>
    public class BasicEntityRepository : IEntityRepository<Guid>
    {
        private readonly IEventStore<Guid> _eventStore;
        private readonly IEventRetriever<Guid> _eventRetriever;
        private readonly IEntityFactory<Guid> _entityFactory;
        
        public BasicEntityRepository(IEventStore<Guid> eventStore, IEventRetriever<Guid> eventRetriever, IEntityFactory<Guid> entityFactory)
        {
            _eventStore = eventStore;
            _eventRetriever = eventRetriever;
            _entityFactory = entityFactory;
        }

        public Task AddEvent(IEntityEvent<Guid> ev)
        {
            return _eventStore.StoreEvent(ev);
        }

        public async Task<TEntityType> GetEntity<TEntityType>(Guid entityId, bool allowSnapshots = true) where TEntityType : IEntity<Guid>
        {
            //get all events for our entity
            var events = await _eventRetriever.GetStreamForEntity(entityId);

            events = events.OrderBy(e => e.EventOrderingId).ThenBy(e => e.Timestamp);

            var baseEntity = await _entityFactory.CreateEntity<TEntityType>(entityId);
            foreach (var ev in events)
            {
                await baseEntity.ApplyEvent(ev);
            }

            return baseEntity;
        }

        public Task<long> GetNextEventId()
        {
            return _eventStore.GetNextEventId();
        }
    }
}