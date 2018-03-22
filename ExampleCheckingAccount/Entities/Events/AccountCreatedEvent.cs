using System;
using EventCoursingSimple.Entities;

namespace ExampleCheckingAccount.Entities.Events
{
    public class AccountCreatedEvent : BaseEntityEvent
    {
        public AccountCreatedEvent(Guid entityId) : base(entityId)
        {
        }
        public string AccountHolderName { get; set; }
        public decimal OpeningBalance { get; set; }
        public string EmployeeOpening { get; set; }
    }
}