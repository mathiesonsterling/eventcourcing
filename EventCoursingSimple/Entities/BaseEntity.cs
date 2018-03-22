using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventCoursing.Entities;
using EventCoursing.Services;

namespace EventCoursingSimple.Entities
{
    /// <summary>
    /// Base entity that we can use to easily construct our Domain specific things
    /// </summary>
    public abstract class BaseEntity : IEntity<Guid>
    {
        public Guid Id { get; protected set; }

        private IDictionary<string, Func<IEntityEvent<Guid>, Task<EntityEventResult>>> _mappings;
        protected IEventReceiver<Guid> EventPipeline { get; private set; }

        /// <summary>
        /// We're doing method injection here - don't like it, but with new() generics have no choice
        /// </summary>
        /// <param name="pipeline"></param>
        internal void SetPipeline(IEventReceiver<Guid> pipeline)
        {
            EventPipeline = pipeline;
            _mappings = SetupHandlers();
        }

        internal void SetId(Guid id)
        {
            Id = id;
        }
        
        /// <summary>
        /// The implementing class needs to provide a mapping of event names, and the functions to handle those events
        /// </summary>
        protected abstract IDictionary<string, Func<IEntityEvent<Guid>, Task<EntityEventResult>>> SetupHandlers();

        public Task<EntityEventResult> ApplyEvent(IEntityEvent<Guid> ev)
        {
            if (_mappings == null)
            {
                throw new InvalidOperationException("Entity cannot be used until SetPipeline has been called, and entity is unpopulated."+
                                                    "  Please use the Factory to GetEntity instead");
            }
            if (!_mappings.ContainsKey(ev.Name))
            {
                return Task.FromResult(EntityEventResult.Ignored);
            }

            var func = _mappings[ev.Name];
            return func(ev);
        }
    }
}