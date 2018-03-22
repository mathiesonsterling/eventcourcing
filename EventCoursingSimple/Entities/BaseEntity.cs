using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EventCoursing.Entities;
using EventCoursing.Services;

namespace EventCoursingSimple.Entities
{
    /// <inheritdoc />
    /// <summary>
    /// Base entity that we can use to easily construct our Domain specific things, and handle events internally
    /// </summary>
    public abstract class BaseEntity : IEntity<Guid>
    {
        public Guid Id { get; protected set; }

        protected IEventReceiver<Guid> EventPipeline { get; private set; }

        /// <summary>
        /// We're doing method injection here - don't like it, but with new() generics have no choice
        /// </summary>
        /// <param name="pipeline"></param>
        internal void SetPipeline(IEventReceiver<Guid> pipeline)
        {
            EventPipeline = pipeline;
        }

        public Task<EntityEventResult> ApplyEvent(IEntityEvent<Guid> ev)
        {
            //get the methods that have our attribute, and the same entity event as us
            //more than one is an error, and we throw

            var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

            var methodsAndAtts = methods
                .Select(m =>
                    new Tuple<MethodInfo, EntityEventHandlerAttribute>(m,
                        m.GetCustomAttribute<EntityEventHandlerAttribute>()));

            var filtered = methodsAndAtts
                .Where(t => t.Item2 != null)
                .Where(t => t.Item2.EventType == ev.GetType())
                .Select(t => t.Item1)
                .ToList();

            if (filtered.Count > 1)
            {
                var methodNames = filtered.Select(f => f.Name);
                var methodNameJoin = string.Join(", ", methodNames);
                throw new InvalidOperationException($"More than one method is attempting to handle an event.  Methods: {methodNameJoin}");
            }

            var method = filtered.FirstOrDefault();

            if (method == null)
            {
                return Task.FromResult(EntityEventResult.Ignored);
            }

            var param = new object[]{ev};
            var res = method.Invoke(this, param);

            return (Task<EntityEventResult>)res;
        }
    }
}