using System;

namespace EventCoursing.Entities
{
    /// <summary>
    /// Represents an event, which occurred relative to an entity.  Entities are the sum of events
    /// </summary>
    public interface IEntityEvent<out TIdentifierType>
    {
         TIdentifierType EntityId { get; }
         DateTime Timestamp { get; }
        
        /// <summary>
        /// Name of the event.  This MUST be unique
        /// </summary>
        string Name { get; }
    }
}