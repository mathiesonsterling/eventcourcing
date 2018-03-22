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
        
        public BasicEntityRepository(IEventStore<Guid> eventStore, IEventRetriever<Guid> eventRetriever)
        {
            _eventStore = eventStore;
            _eventRetriever = eventRetriever;
        }

        public Task<TEntityType> GetEntity<TEntityType>(Guid entityId, bool allowSnapshots = true) 
            where TEntityType : class, IEntity<Guid>, new()
        {

            var entity = new TEntityType();

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

            var realEnt = entity as BaseEntity;
            if (realEnt == null)
            {
                throw new InvalidOperationException("Entities must derive from BaseEntity to use this factory");
            }
            
            //set the entity to send new events back to us
            realEnt.SetPipeline(this);
            if (events != null)
            {
                foreach (var ev in events)
                {
                    await entity.ApplyEvent(ev);
                }
            }

            return entity;
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