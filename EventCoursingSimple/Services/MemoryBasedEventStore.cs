using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EventCoursing.Entities;
using EventCoursing.Services;

namespace EventCoursingSimple.Services
{
    /// <summary>
    /// Simpliest possible event store - just a big pile of events in a dictionary in memory
    /// </summary>
    public class MemoryBasedEventStore : IEventRetriever<Guid>, IEventStore<Guid>
    {
        private readonly Dictionary<Guid, IList<IEntityEvent<Guid>>> _events;
        private long currentEventPos;
        
        public Task<IEnumerable<IEntityEvent<Guid>>> GetStreamForEntity(Guid entityId, long? startId = null)
        {
            return Task.FromResult(_events[entityId].AsEnumerable());
        }

        public Task<IEnumerable<IEntityEvent<Guid>>> GetEvents(DateTime start, DateTime end)
        {
            var events = AllEvents.Where(e => e.Timestamp >= start && e.Timestamp <= end);
            return Task.FromResult(events);
        }

        public Task<IEnumerable<IEntityEvent<Guid>>> GetEvents(long startPos, long endPos)
        {
            var events = AllEvents.Where(e => e.EventOrderingId >= startPos && e.EventOrderingId <= endPos);
            return Task.FromResult(events);
        }

        public Task<bool> StoreEvent(IEntityEvent<Guid> ev)
        {
            if (!_events.ContainsKey(ev.EntityId))
            {
                _events[ev.EntityId] = new List<IEntityEvent<Guid>> {ev};
            }
            else
            {
                _events[ev.EntityId].Add(ev);
            }
        }

        public Task<long> GetNextEventId()
        {
            currentEventPos = currentEventPos + 1;
            return Task.FromResult(currentEventPos);
        }

        private IEnumerable<IEntityEvent<Guid>> AllEvents
        {
            get
            {
                foreach (var key in _events)
                {
                    foreach (var ev in _events[key])
                    {
                        yield return ev;
                    }
                }
            }
        }
    }
}