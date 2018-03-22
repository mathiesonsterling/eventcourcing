using System;
using EventCoursingSimple.Entities;

namespace CheckingAccountExample.Entities.Events
{
    public class CheckCashedEvent : BaseEntityEvent
    {
        public const string EVENT_NAME = "CheckCashed";
        public override string Name => EVENT_NAME;
        
        public string Recepient { get; set; }
        public string Memo { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public CheckCashedEvent(Guid entityId) : base(entityId)
        {
        }
    }
}