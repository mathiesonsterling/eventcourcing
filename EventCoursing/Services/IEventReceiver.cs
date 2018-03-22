using System.Threading.Tasks;
using EventCoursing.Entities;

namespace EventCoursing.Services
{
    /// <summary>
    /// Pipeline to recieve events then send them to the correct entity.  Can be a repository, or a message bus
    /// </summary>
    public interface IEventReceiver<TIdentifierType>
    {
        /// <summary>
        /// Store an event in the store
        /// </summary>
        /// <param name="ev"></param>
        /// <returns></returns>
        Task<EntityEventResult> AddEvent<TEntityType>(IEntityEvent<TIdentifierType> ev)
            where TEntityType : IEntity<TIdentifierType>;
    }
}