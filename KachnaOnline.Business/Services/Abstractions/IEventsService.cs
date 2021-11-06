// IEventsService.cs
// Author: David Chocholatý

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.Events;
using KachnaOnline.Business.Models.Events;

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
        /// <exception cref="UserNotFoundException">When a user with the ID assigned to the event does
        /// not exist.</exception>
        Task<Event> PlanEvent(NewEvent newEvent);
        // TODO Add service to link states to events.

        /// <summary>
        /// Removes an event record and all planned states that were linked to the event.
        /// State records from the past are not removed.
        /// </summary>
        /// <param name="eventId">ID of the event to remove.</param>
        /// <summary>
        /// Remove an event with the given ID.
        /// </summary>
        /// <exception cref="EventNotFoundException">When the event with the given <paramref name="eventId"/> does not
        /// exist.</exception>
        /// <exception cref="EventManipulationFailedException">When the event cannot be deleted.</exception>
        Task RemoveEvent(int eventId);

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
        Task ModifyEvent(int eventId, ModifiedEvent modifiedEvent);
    }
}
