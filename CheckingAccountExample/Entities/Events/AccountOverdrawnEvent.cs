using System;
using EventCoursingSimple.Entities;

namespace CheckingAccountExample.Entities.Events
{
    /// <summary>
    /// Signals that an account was overdrawn
    /// </summary>
    public class AccountOverdrawnEvent : BaseEntityEvent
    {
        public const string EVENT_NAME = "AccountOverdrawn";
        public override string Name => EVENT_NAME;

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