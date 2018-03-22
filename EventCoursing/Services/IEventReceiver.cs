using System.Threading.Tasks;
using EventCoursing.Entities;

namespace EventCoursing.Services
{
    /// <summary>
    /// Represents any item which can retrieve and then process events to keep the system in a consistent state
    /// </summary>
    public interface IEventReceiver<TIdentifierType>
    {
        /// <summary>
        /// Store an event in the store
        /// </summary>
        /// <param name="ev"></param>
        /// <returns></returns>
        Task<EntityEventResult> AddEvent<TEntityType>(IEntityEvent<TIdentifierType> ev)
            where TEntityType : class, IEntity<TIdentifierType>, new();
    }
}