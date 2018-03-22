using System;
using System.Threading.Tasks;
using EventCoursing.Entities;

namespace EventCoursingSimple.Entities
{
    /// <summary>
    /// Base entity that we can use to easily construct our Domain specific things
    /// </summary>
    public abstract class BaseEntity : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public abstract Task<EntityEventResult> ApplyEvent(IEntityEvent<Guid> ev);
    }
}