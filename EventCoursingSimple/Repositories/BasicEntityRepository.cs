using System;
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
        
        public BasicEntityRepository(IEventStore<Guid> eventStore, IEventRetriever<Guid> eventRetriever)
        {
            _eventStore = eventStore;
            _eventRetriever = eventRetriever;
        }

        public async Task<TEntityType> GetEntity<TEntityType>(Guid entityId, bool allowSnapshots = true) 
            where TEntityType : class, IEntity<Guid>, new()
        {
            //get all events for our entity
            var events = await _eventRetriever.GetStreamForEntity(entityId);

            events = events.OrderBy(e => e.Timestamp);

            var entity = new TEntityType() as BaseEntity;

            if (entity == null)
            {
                throw new InvalidOperationException("Entities must derive from BaseEntity to use this factory");
            }
            
            //set the entity to send new events back to us
            entity.SetPipeline(this);
            foreach (var ev in events)
            {
                await entity.ApplyEvent(ev);
            }

            return entity as TEntityType;
        }

        public async Task<EntityEventResult> AddEvent<TEntityType>(IEntityEvent<Guid> ev) where TEntityType : class, IEntity<Guid>, new()
        {
            var entity = await GetEntity<TEntityType>(ev.EntityId);
            
            var retrieve = entity.ApplyEvent(ev);

            await _eventStore.StoreEvent(ev);

            return await retrieve;
        }
    }
}