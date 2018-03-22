namespace EventCoursing.Entities
{
    /// <summary>
    /// When an entity is given an event, what it did with it
    /// </summary>
    public enum EntityEventResult
    {
        /// <summary>
        /// The entity isn't in a state to take this event
        /// </summary>
        Ignored,
        /// <summary>
        /// The event was submitted and validated, but further processing is needed by the entity
        /// </summary>
        Applying,
        /// <summary>
        /// The event was successfully applied and now makes up part of the entity
        /// </summary>
        Applied,
        /// <summary>
        /// The event is the correct type, but data fields in the event are contradictory
        /// </summary>
        FailedVerification,
        /// <summary>
        /// This event type is not applicable to the entity
        /// </summary>
        InvalidEventType
    }
}