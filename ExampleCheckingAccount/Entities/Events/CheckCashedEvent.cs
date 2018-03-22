using System;
using EventCoursingSimple.Entities;

namespace ExampleCheckingAccount.Entities.Events
{
    public class CheckCashedEvent : BaseEntityEvent
    {
        
        public string Recepient { get; set; }
        public string Memo { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public CheckCashedEvent(Guid entityId) : base(entityId)
        {
        }
    }
}