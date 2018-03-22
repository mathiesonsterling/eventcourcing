using System;
using EventCoursingSimple.Entities;

namespace ExampleCheckingAccount.Entities.Events
{
    /// <summary>
    /// Signals that an account was overdrawn
    /// </summary>
    public class AccountOverdrawnEvent : BaseEntityEvent
    {

        public AccountOverdrawnEvent(Guid entityId) : base(entityId)
        {
            OverdrawnEventId = Guid.NewGuid();
        }

        /// <summary>
        /// Keeps an id, so we can later deal with this event in other systems as notified
        /// </summary>
        public Guid OverdrawnEventId { get; }
    }
}