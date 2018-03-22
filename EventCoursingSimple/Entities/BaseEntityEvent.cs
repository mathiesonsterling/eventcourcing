using System;
using EventCoursing.Entities;
using EventCoursing.Services;

namespace EventCoursingSimple.Entities
{
    public abstract class BaseEntityEvent : IEntityEvent<Guid>
    {
        protected BaseEntityEvent(Guid entityId)
        {
            EntityId = entityId;
            Timestamp = DateTime.UtcNow;
        }
        public Guid EntityId { get; protected set; }
        public DateTime Timestamp { get; }
        public abstract string Name { get; }
    }
}