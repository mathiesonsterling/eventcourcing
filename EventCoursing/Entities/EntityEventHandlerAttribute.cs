using System;

namespace EventCoursing.Entities
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EntityEventHandlerAttribute : System.Attribute
    {
        public EntityEventHandlerAttribute(Type eventType)
        {
            EventType = eventType;
        }
        public Type EventType { get; }
    }
}