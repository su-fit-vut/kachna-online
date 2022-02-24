using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.ClubStates;
using KachnaOnline.Business.Models.ClubStates;

namespace KachnaOnline.Business.Services.Abstractions
{
    public interface IClubStateService
    {
        /// <summary>
        /// Returns the current state.
        /// </summary>
        /// <returns>The current state. Its <see cref="State.FollowingState"/> is populated if the state has
        /// a following state.</returns>
        Task<State> GetCurrentState();

        /// <summary>
        /// Returns the state with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The ID of the state.</param>
        /// <returns>The state. Its <see cref="State.FollowingState"/> is populated if the state has
        /// a following state.</returns>
        Task<State> GetState(int id);

        /// <summary>
        /// Returns the next (closest to now) planned state of the given <paramref name="type"/>, if specified.
        /// </summary>
        /// <param name="type">The state type. If null, no restrictions are placed on the returned state type.</param>
        /// <param name="enablePrivate">If false, private states will never be returned.</param>
        /// <returns>The next planned state.
        /// Its <see cref="State.FollowingState"/> is populated if the state has a following state.</returns>
        Task<State> GetNextPlannedState(StateType? type, bool enablePrivate = true);

        /// <summary>
        /// Returns a collection of all state records in the specified time range.
        /// </summary>
        /// <param name="from">Only returns states starting after this datetime.</param>
        /// <param name="to">Only returns states starting before this datetime.</param>
        /// <exception cref="ArgumentException">The <paramref name="to"/> datetime is before <paramref name="from"/>.
        /// </exception>
        /// <exception cref="ArgumentException">The time range specified by <paramref name="from"/> and
        /// <paramref name="to"/> is longer than 60 days.</exception>
        /// <returns>A collection of <see cref="State"/> objects. Their <see cref="State.FollowingState"/> field
        /// is not populated.</returns>
        Task<ICollection<State>> GetStates(DateTime from, DateTime to);

        /// <summary>
        /// Returns a collection of all repeating states, optionally bounded by their effective dates.
        /// </summary>
        /// <param name="effectiveFrom">If not null, only returns repeating states with their effective from date
        /// being after or equal to the specified value.</param>
        /// <param name="effectiveTo">If not null, only returns repeating states with their effective to date
        /// being before or equal to the specified value.</param>
        /// <returns>A collection of <see cref="RepeatingState"/> objects.</returns>
        Task<ICollection<RepeatingState>> GetRepeatingStates(DateTime? effectiveFrom = null,
            DateTime? effectiveTo = null);

        /// <summary>
        /// Returns a collection of all repeating states that are effective at the specified date.
        /// </summary>
        /// <param name="effectiveAt">The date to return repeating states for.</param>
        /// <returns>A collection of <see cref="RepeatingState"/> objects.</returns>
        Task<ICollection<RepeatingState>> GetRepeatingStates(DateTime effectiveAt);

        /// <summary>
        /// Returns a repeating state with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The ID of the repeating state.</param>
        /// <returns>A <see cref="RepeatingState"/> object.</returns>
        Task<RepeatingState> GetRepeatingState(int id);

        /// <summary>
        /// Returns a collection of all states that were planned based on the specified repeating state.
        /// </summary>
        /// <param name="repeatingStateId">The ID of the repeating state.</param>
        /// <param name="futureOnly">If true, only the planned states that haven't started yet will be returned.</param>
        /// <returns>A collection of <see cref="State"/> objects describing the states planned for the repeating state.
        /// Their <see cref="State.FollowingState"/> field is not populated.</returns>
        Task<ICollection<State>> GetStatesForRepeatingState(int repeatingStateId, bool futureOnly = true);

        /// <summary>
        /// Creates a new repeating state record and plans states accordingly.
        /// </summary>
        /// <exception cref="ArgumentException"><see cref="RepeatingState.EffectiveTo"/> is before
        /// <see cref="RepeatingState.EffectiveFrom"/>.</exception>
        /// <exception cref="ArgumentException"><see cref="RepeatingState.TimeTo"/> represents a time earlier
        /// than <see cref="RepeatingState.TimeFrom"/>.</exception>
        /// <exception cref="ArgumentException"><see cref="RepeatingStateModification.State"/> is
        /// set to <see cref="StateType.Closed"/>.</exception>
        /// <exception cref="StatePlanningException">A locking or database error occurs.</exception>
        /// <returns>A <see cref="RepeatingStatePlanningResult"/> with information about the ID of the newly
        /// created repeating state and about existing overlapping planned states.</returns>
        Task<RepeatingStatePlanningResult> MakeRepeatingState(RepeatingState newRepeatingState);

        /// <summary>
        /// Removes a repeating state record if its effective span hasn't started yet.
        /// If it has, deletes all its planned states that haven't started yet and changes its
        /// <see cref="RepeatingState.EffectiveTo"/> to today, making it ended and immutable.
        /// </summary>
        /// <param name="repeatingStateId">The ID of the repeating state to remove or end.</param>
        /// <param name="removedById">The ID of the user performing the operation.</param>
        /// <remarks>See <see cref="ModifyRepeatingState"/> for information about exceptions.</remarks>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <seealso cref="ModifyRepeatingState"/>
        Task RemoveRepeatingState(int repeatingStateId, int removedById);

        /// <summary>
        /// Changes details of the specified repeating state. Projects these changes into all
        /// its planned states. Passed states are not affected.
        /// </summary>
        /// <remarks>
        /// When <see cref="RepeatingState.EffectiveTo"/> is changed, new states may be planned or existing
        /// planned states may be removed.<br/>
        /// <see cref="RepeatingState.EffectiveTo"/> cannot be changed to a date in the past.<br/>
        /// Repeating states of which the <see cref="RepeatingState.EffectiveTo"/>
        /// has already passed cannot be modified at all.
        /// </remarks>
        /// <exception cref="RepeatingStateNotFoundException">No repeating state with the ID specified
        /// in <paramref name="modification"/> exists.</exception>
        /// <exception cref="RepeatingStateReadOnlyException">The target repeating state's
        /// <see cref="RepeatingState.EffectiveTo"/> is today or in the past.</exception>
        /// <exception cref="ArgumentException"><see cref="RepeatingStateModification.State"/> is
        /// set to <see cref="StateType.Closed"/>.</exception>
        /// <exception cref="ArgumentException"><see cref="RepeatingStateModification.EffectiveTo"/> is
        /// set to a past date.</exception>
        /// <exception cref="UserNotFoundException">The user doesn't exist.</exception>
        /// <exception cref="UserUnprivilegedException"><see cref="RepeatingState.MadeById"/>
        /// modification is requested by a non-administrator user.</exception>
        /// <exception cref="StatePlanningException">A locking or database error occurs.</exception>
        /// <returns>A <see cref="RepeatingStatePlanningResult"/> with information about existing overlapping
        /// planned states. (Relevant iff the repeating state is prolonged.)</returns>
        Task<RepeatingStatePlanningResult> ModifyRepeatingState(RepeatingStateModification modification,
            int changeMadeByUserId);

        /// <summary>
        /// Creates a new planned state record.
        /// </summary>
        /// <remarks>
        /// A state cannot be planned if it begins earlier than an already planned state that it would overlap with.
        /// However, it can be planned so that it begins in the middle of an already planned state. In that case,
        /// the <see cref="State.PlannedEnd"/> of the previously planned state is set to the newly planned state's
        /// beginning and its next state reference is set to the newly planned state.
        /// </remarks>
        /// <returns>A <see cref="StatePlanningResult"/> with information about the newly planned state, a state that
        /// was modified as a result of this new state or about states that it would overlap with.</returns>
        Task<StatePlanningResult> PlanState(NewState newState);

        /// <summary>
        /// Removes the specified planned state record.
        /// </summary>
        /// <param name="id">The ID of the planned state to remove.</param>
        /// <exception cref="StateReadOnlyException">
        /// The specified state has already started or ended.</exception>
        /// <exception cref="StateNotFoundException">No state with the specified <paramref name="id"/>
        /// exists.</exception>
        /// <exception cref="StatePlanningException">A locking or database error occurs.</exception>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task RemovePlannedState(int id);

        /// <summary>
        /// Changes details of the specified state (current or planned).
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        ///     <item><description>
        ///     For the current state and planned states, the planned end and both notes can be changed.
        ///     </description></item>
        ///     <item><description>
        ///     Planned end must be set to a date after the state's start.
        ///     </description></item>
        ///     <item><description>
        ///     For already planned states in the future, the planned start can also be changed.
        ///     Planned start must be set to a date after now and before the state's planned end.
        ///     </description></item>
        ///     <item><description>
        ///     For states that already ended, only the internal note can be changed.
        ///     </description></item>
        ///     <item><description>
        ///     Administrators (only) can also change the MadeById.
        ///     </description></item>
        ///     <item><description>
        ///     A general rule is that a state cannot be (re-)planned in a way that would cause another state's start
        ///     to be changed.
        ///     </description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="InvalidOperationException">One of the restrictions is violated.</exception>
        /// <exception cref="StateNotFoundException">No state with the specified
        /// <see cref="StateModification.StateId"/> exists or, if it's not specified, when no state is currently active.
        /// </exception>
        /// <exception cref="UserNotFoundException">The user doesn't exist.</exception>
        /// <exception cref="StatePlanningException">A locking or database error occurs.</exception>
        /// <returns></returns>
        Task<StatePlanningResult> ModifyState(StateModification stateModification, int changeMadeByUserId);

        /// <summary>
        /// Sets the current state as closed.
        /// </summary>
        /// <remarks>
        /// This sets the state's <see cref="State.Ended"/> to the current datetime.
        /// </remarks>
        /// <exception cref="UserNotFoundException">The user doesn't exist.</exception>
        /// <exception cref="StatePlanningException">A locking or database error occurs.</exception>
        /// <exception cref="StateNotFoundException">No state is currently active.</exception>
        /// <returns></returns>
        Task CloseNow(int closedByUserId);

        /// <summary>
        /// Sets state <see cref="State.EventId"/> to null (unlinks that state from any event) to state specified by <paramref name="stateId"/>.
        /// </summary>
        /// <param name="stateId">State ID to set its <see cref="State.EventId"/> to null.</param>
        /// <exception cref="StateReadOnlyException">Thrown when planned state to be unlinked from the event has already started or ended.</exception>
        /// <exception cref="StateNotFoundException">Thrown when planned state to be unlinked from the event has not been found.</exception>
        /// <exception cref="StateNotAssociatedToEventException">Thrown when planned states is not associated to any event.</exception>
        Task UnlinkStateFromEvent(int stateId);

    }
}
