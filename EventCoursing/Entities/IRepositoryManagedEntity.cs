using EventCoursing.Services;

namespace EventCoursing.Entities
{
    public interface IRepositoryManagedEntity<TIdentifierType> : IEntity<TIdentifierType>
    {
        /// <summary>
        /// Allows the factory to set IDs, needed for object registration
        /// </summary>
        /// <param name="id"></param>
        void SetId(TIdentifierType id);
        /// <summary>
        /// Allows the factory to set a receiver, which allows entities to fire events without locating the entity
        /// </summary>
        /// <param name="???"></param>
        void SetReceiver(IEventReceiver<TIdentifierType> receiver);
    }
}