using System;
using EventCoursing.Entities;
using EventCoursingSimple.Entities;

namespace CheckingAccountExample.Entities.Events
{
    public class AccountCreatedEvent : BaseEntityEvent
    {
        public AccountCreatedEvent() : base(Guid.NewGuid())
        {
        }

        public const string EVENT_NAME = "AccountCreated";
        public string AccountHolderName { get; set; }
        public decimal OpeningBalance { get; set; }
        public string EmployeeOpening { get; set; }
        public override string Name => EVENT_NAME;
    }
}