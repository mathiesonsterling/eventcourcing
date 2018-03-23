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
    public abstract class BaseEntity : IRepositoryManagedEntity<Guid>
    {
        public Guid Id { get; private set; }

        private IEventReceiver<Guid> _eventReceiver;

        public void SetId(Guid id)
        {
            Id = id;
        }

        void IRepositoryManagedEntity<Guid>.SetReceiver(IEventReceiver<Guid> receiver)
        {
            _eventReceiver = receiver;
        }

        
        /// <summary>
        /// Allows entities to send events to other entities internally
        /// </summary>
        /// <param name="ev"></param>
        /// <typeparam name="TDestinationEntity"></typeparam>
        /// <returns></returns>
        protected Task<EntityEventResult> SendEvent<TDestinationEntity>(IEntityEvent<Guid> ev) where TDestinationEntity: IEntity<Guid>
        {
            return _eventReceiver.AddEvent<TDestinationEntity>(ev);
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