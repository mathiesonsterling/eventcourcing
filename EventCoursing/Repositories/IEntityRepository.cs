using System;
using System.Threading.Tasks;
using EventCoursing.Entities;

namespace EventCoursing.Repositories
{
    /// <summary>
    /// Retrieves events with their data as the representation of their stream
    /// </summary>
    /// <typeparam name="TIdentifierType"></typeparam>
    public interface IEntityRepository<TIdentifierType>
    {
        /// <summary>
        /// Get all events for a stream, apply them to 
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="allowSnapshots"></param>
        /// <typeparam name="TEntityType"></typeparam>
        /// <returns></returns>
        Task<TEntityType> GetEntity<TEntityType>(TIdentifierType entityId, bool allowSnapshots = true)
            where TEntityType : class, IEntity<TIdentifierType>, new();
    }
}