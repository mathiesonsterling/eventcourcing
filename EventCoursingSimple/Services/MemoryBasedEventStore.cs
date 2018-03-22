using System;
using System.Collections;
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
        private DateTime? _lastEventTime;

        public MemoryBasedEventStore()
        {
            _events = new Dictionary<Guid, IList<IEntityEvent<Guid>>>();
        }

        public Task<IEnumerable<IEntityEvent<Guid>>> GetStreamForEntity(Guid entityId, long? startId = null)
        {
            if (_events.ContainsKey(entityId))
            {
                return Task.FromResult(_events[entityId].AsEnumerable());
            }

            return Task.FromResult(new List<IEntityEvent<Guid>>().AsEnumerable());
        }

        public Task<IEnumerable<IEntityEvent<Guid>>> GetEvents(DateTime start, DateTime end)
        {
            var events = AllEvents.Where(e => e.Timestamp >= start && e.Timestamp <= end);
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

            _lastEventTime = ev.Timestamp;
            return Task.FromResult(true);
        }

        public DateTime GetLastEventTime()
        {
            return _lastEventTime??DateTime.MinValue;
        }

        private IEnumerable<IEntityEvent<Guid>> AllEvents
        {
            get
            {
                foreach (var list in _events.Values)
                {
                    foreach (var ev in list)
                    {
                        yield return ev;
                    }
                }
            }
        }
    }
}