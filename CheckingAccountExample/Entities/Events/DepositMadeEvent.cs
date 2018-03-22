using System;
using EventCoursingSimple.Entities;

namespace CheckingAccountExample.Entities.Events
{
    public class DepositMadeEvent : BaseEntityEvent
    {
        public const string EVENT_NAME = "DepositMade";
        public override string Name => EVENT_NAME;
        public decimal Amount { get; set; }

        public DepositMadeEvent(Guid entityId) : base(entityId)
        {
        }
    }
}