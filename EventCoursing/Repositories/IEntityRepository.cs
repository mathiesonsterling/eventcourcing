using System;
using System.Runtime.InteropServices;
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
        /// Create a new instance of an entity type that can recieve events
        /// </summary>
        /// <param name="allowSnapshots"></param>
        /// <typeparam name="TEntityType"></typeparam>
        /// <returns></returns>
        Task<TEntityType> CreateEntity<TEntityType>(bool allowSnapshots = true)
            where TEntityType : class, IEntity<TIdentifierType>, new();
        
        /// <summary>
        /// Get an existing entity by its ID and return it with its data
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="allowSnapshots"></param>
        /// <typeparam name="TEntityType"></typeparam>
        /// <returns></returns>
        Task<TEntityType> GetEntity<TEntityType>(TIdentifierType entityId, bool allowSnapshots = true)
            where TEntityType : class, IEntity<TIdentifierType>, new();

        /// <summary>
        /// Take an instance of an entity, register it and hydrate it
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="allowSnapshots"></param>
        /// <typeparam name="TEntityType">The type of IEntity that we're working with</typeparam>
        /// <returns></returns>
        Task<TEntityType> RegisterEntity<TEntityType>(TEntityType entity, bool allowSnapshots = true)
            where TEntityType : IEntity<TIdentifierType>;
    }
}