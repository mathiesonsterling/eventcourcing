using System;
using System.Threading.Tasks;
using EventCoursing.Entities;

namespace EventCoursing.Services
{
    /// <summary>
    /// Write only stream to push events into permanent storage
    /// </summary>
    /// <typeparam name="TIdentifierType"></typeparam>
    public interface IEventStore<in TIdentifierType>
    {
        /// <summary>
        /// Take an event and put it into the store
        /// </summary>
        /// <param name="ev"></param>
        /// <returns></returns>
        Task<bool> StoreEvent(IEntityEvent<TIdentifierType> ev);

        DateTime GetLastEventTime();
    }
}