using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventCoursing.Entities;

namespace EventCoursing.Services
{
    public interface IEventRetriever<TIdentifierType>
    {
        /// <summary>
        /// Get all events for an entity
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="startId">Optional.  If set, only get events after this, noninclusive</param>
        /// <returns></returns>
        Task<IEnumerable<IEntityEvent<TIdentifierType>>> GetStreamForEntity(TIdentifierType entityId, long? startId = null);
        
        /// <summary>
        /// Get all events based on time, inclusive
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<IEnumerable<IEntityEvent<TIdentifierType>>> GetEvents(DateTime start, DateTime end);
    }
}