using System.Threading.Tasks;

namespace EventCoursing.Entities
{
    /// <summary>
    /// Basic entity in a DDD sense.  Can specify which type of field is the key, and what class is set to be the data holder
    /// </summary>
    /// <typeparam name="TIdentifierType"></typeparam>
    /// <typeparam name="TDataType"></typeparam>
    public interface IEntity<TIdentifierType>
    {
        TIdentifierType Id { get; }
        
        /// <summary>
        /// Apply an event
        /// </summary>
        /// <param name="ev"></param>
        /// <returns></returns>
        Task<EntityEventResult> ApplyEvent(IEntityEvent<TIdentifierType> ev);
    }
}