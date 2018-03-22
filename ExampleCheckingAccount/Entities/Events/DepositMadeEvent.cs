using System;
using EventCoursingSimple.Entities;

namespace ExampleCheckingAccount.Entities.Events
{
    public class DepositMadeEvent : BaseEntityEvent
    {
        public decimal Amount { get; set; }

        public DepositMadeEvent(Guid entityId) : base(entityId)
        {
        }
    }
}