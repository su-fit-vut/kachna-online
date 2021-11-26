// IEventsService.cs
// Author: David Chocholatý

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.ClubStates;
using KachnaOnline.Business.Exceptions.Events;
using KachnaOnline.Business.Models.ClubStates;
using KachnaOnline.Business.Models.Events;
using KachnaOnline.Data.Entities.ClubStates;

namespace KachnaOnline.Business.Services.Abstractions
{
    public interface IEventsService
    {
        /// <summary>
        /// Returns a collection of current events.
        /// </summary>
        /// <returns>A Collection of <see cref="Event"/> of current events or an empty collection if no there is no event
        /// held currently. The collection usually contains maximally one event, more in case multiple next events start
        /// at the same time.</returns>
        Task<ICollection<Event>> GetCurrentEvents();

        /// <summary>
        /// Returns the event corresponding to the specified <paramref name="eventId"/>.
        /// </summary>
        /// <param name="eventId">The event ID to search for.</param>
        /// <returns>An <see cref="Event"/> object containing the event matching the specified <paramref name="eventId"/>
        /// </returns>
        /// <exception cref="EventNotFoundException"> thrown when an event with the given <paramref name="eventId"/>
        /// does not exist.</exception>
        Task<Event> GetEvent(int eventId);

        /// <summary>
        /// Returns a list of next (closest to now) planned events.
        /// </summary>
        /// <remarks>Usually returns only one event, only if two or more next events start at the same time,
        /// list contains more than one event.</remarks>
        /// <returns>An <see cref="Event"/> object containing the next event if such event exists,
        /// or null if it doesn't.</returns>
        Task<ICollection<Event>> GetNextPlannedEvents();

        /// <summary>
        /// Returns a list of events starting in the future, optionally only the ones starting in the timespan specified by <paramref name="from"/> and <paramref name="to"/>.
        /// </summary>
        /// <param name="from">If not null, only returns events with their from date being after or equal to the specified value.</param>
        /// <param name="to">If not null, only returns events with their to date being before or equal to the specified value.</param>
        /// <returns>A Collection of <see cref="Event"/> or an empty list if no there is no event planned.</returns>
        Task<ICollection<Event>> GetEvents(DateTime? from = null, DateTime? to = null);

        /// <summary>
        /// Returns a collection of all events that are held at the date and time specified by <paramref name="at"/>.
        /// </summary>
        /// <param name="at">Date and time to look for held events.</param>
        /// <returns>A Collection of <see cref="Event"/> or an empty list if there is no event at the specified date and time.</returns>
        Task<ICollection<Event>> GetEvents(DateTime at);

        /// <summary>
        /// Plans a new event.
        /// </summary>
        /// <remarks>
        /// An event can be planned even if it begins earlier than or in the middle of an another already planned event that it would overlap.
        /// </remarks>
        /// <param name="newEvent"><see cref="NewEvent"/> to plan.</param>
        /// <returns> The planned <see cref="Event"/> with a filled ID.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="newEvent"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="newEvent"/> has invalid attributes.</exception>
        /// <exception cref="EventManipulationFailedException">Thrown when the board game cannot be created.
        /// This can be caused by a database error.</exception>
        /// <exception cref="UserNotFoundException">Thrown when a user with the ID assigned to the event does
        /// not exist.</exception>
        Task<Event> PlanEvent(NewEvent newEvent);

        /// <summary>
        /// Get conflicting planned states for event specified by <param name="eventId"></param>.
        /// </summary>
        /// <param name="eventId">The event to get the conflicting planned states for.</param>
        /// <returns>A collection of conflicting states for the event.</returns>
        /// <exception cref="EventNotFoundException">When the event with the given <paramref name="eventId"/> does not
        /// exist.</exception>
        Task<ICollection<State>> GetConflictingStatesForEvent(int eventId);

        /// <summary>
        /// Removes an event record with the ID <paramref name="eventId"/>.
        /// </summary>
        /// <param name="eventId">ID of the event to remove.</param>
        /// <returns>A list of <see cref="State"/> linked to this event at the time of deletion.</returns>
        /// <remarks>
        /// All planned states that were linked to the event have their reference to the event set to null.
        /// </remarks>>
        /// <exception cref="EventNotFoundException">When the event with the given <paramref name="eventId"/> does not
        /// exist.</exception>
        /// <exception cref="EventManipulationFailedException">thrown when the event cannot be deleted.</exception>
        Task<ICollection<State>> RemoveEvent(int eventId);

        /// <summary>
        /// Sets state <see cref="State.EventId"/> to null to states specified by <paramref name="stateId"/>;
        /// </summary>
        /// <param name="stateId">State ID to set its <see cref="State.EventId"/> to null.</param>
        /// <exception cref="EventManipulationFailedException">Thrown when linked planned states cannot be unlinked from the event.</exception>
        /// <exception cref="StateReadOnlyException">Thrown when planned state to be unlinked from the event has already started or ended.</exception>
        /// <exception cref="StateNotFoundException">Thrown when planned state to be unlinked from the event has not been found.</exception>
        /// <exception cref="StateNotAssociatedToEventException">Thrown when planned states is not associated to any event.</exception>
        Task UnlinkStateFromEvent(int stateId);

        /// <summary>
        /// Changes details of an event specified by <paramref name="eventId"/>. Projects these changes into all planned states linked to this event. State records from the past are not changed.
        /// </summary>
        /// <remarks>
        /// When one of <see cref="Event.From"/> or <see cref="Event.To"/>
        /// is changed, new states may be planned or existing planned states may be unlinked and/or removed. These properties
        /// cannot be changed if the dates have already passed.<br/>
        /// <see cref="Event.To"/> cannot be changed to a date in the past.<br/>
        /// Events of which the <see cref="Event.To"/>
        /// has already passed cannot be modified at all.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when the passed <paramref name="modifiedEvent"/> model is null.</exception>
        /// <exception cref="EventNotFoundException">Thrown when an event with the given <paramref name="eventId"/>
        /// does not exist.</exception>
        /// <exception cref="EventReadOnlyException">Thrown when event to be modified has already ended.</exception>
        /// <exception cref="EventManipulationFailedException">Thrown when event modification has failed.</exception>
        Task ModifyEvent(int eventId, ModifiedEvent modifiedEvent);

        /// <summary>
        /// Link planned states to event specified by <paramref name="eventId"/>
        /// </summary>
        /// <param name="eventId">ID of the event to link planned states to.</param>
        /// <param name="plannedStatesToLinkIds">List of planned state IDs to link to the event specified by <paramref name="eventId"/>.</param>
        /// <exception cref="EventNotFoundException">Thrown when an event with the given <paramref name="eventId"/>
        /// does not exist.</exception>
        /// <exception cref="EventManipulationFailedException">Thrown when planned states cannot be linked to the event.</exception>
        /// <exception cref="LinkingStateToEventException">Thrown when linking planned states to the event is not possible.</exception>
        /// <exception cref="StateReadOnlyException">Thrown when planned state to be linked to the event has already started or ended.</exception>
        /// <exception cref="StateNotFoundException">Thrown when planned state to be linked to the event has not been found.</exception>
        /// <exception cref="EventReadOnlyException">Thrown when event to be linked to has already ended.</exception>
        Task LinkPlannedStatesToEvent(int eventId, IEnumerable<int> plannedStatesToLinkIds);


        /// <summary>
        /// Get states linked to an event specified by <param name="eventId"></param>.
        /// </summary>
        /// <param name="eventId">The event to get the linked states for.</param>
        /// <returns>A collection of states linked to the event.</returns>
        /// <exception cref="EventNotFoundException">Thrown when the event with the given <paramref name="eventId"/> does not
        /// exist.</exception>
        Task<ICollection<State>> GetLinkedStates(int eventId);

        /// <summary>
        /// Returns the event corresponding to the specified <paramref name="eventId"/> with linked states.
        /// </summary>
        /// <param name="eventId">The event ID to search for.</param>
        /// <returns>An <see cref="Event"/> object containing the event matching the specified <paramref name="eventId"/>
        /// </returns>
        /// <exception cref="EventNotFoundException">Thrown when an event with the given <paramref name="eventId"/>
        /// does not exist.</exception>
        Task<EventWithLinkedStates> GetEventWithLinkedStates(int eventId);

        /// <summary>
        /// Sets (overrides) the current linked states to the event specified by <paramref name="eventId"/> with <paramref name="plannedStatesToLinkIds"/>.
        /// </summary>
        /// <param name="eventId">Event to set linked planned states for.</param>
        /// <param name="plannedStatesToLinkIds">IDs of states to link to the event.</param>
        Task SetLinkedPlannedStatesForEvent(int eventId, IEnumerable<int> plannedStatesToLinkIds);

        /// <summary>
        /// Clears (unlinks) the current linked states from the event specified by <paramref name="eventId"/>.
        /// </summary>
        /// <param name="eventId">Event to unlink linked planned states from.</param>
        /// <exception cref="EventNotFoundException">Thrown when an event with the given <paramref name="eventId"/>
        /// does not exist.</exception>
        /// <exception cref="EventManipulationFailedException">Thrown when linked planned states cannot be unlinked from the event.</exception>
        /// <exception cref="StateReadOnlyException">Thrown when planned state to be unlinked from the event has already started or ended.</exception>
        /// <exception cref="EventReadOnlyException">Thrown when event to unlinked linked states from has already ended.</exception>
        Task ClearLinkedPlannedStatesForEvent(int eventId);
    }
}
