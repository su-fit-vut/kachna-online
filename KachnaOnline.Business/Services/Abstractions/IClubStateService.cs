// IClubStateService.cs
// Author: Ondřej Ondryáš

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
        /// <returns></returns>
        Task<State> GetCurrentState();

        /// <summary>
        /// Returns the state with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<State> GetState(int id);

        /// <summary>
        /// Returns the next (closest to now) planned state of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<State> GetNextPlannedState(StateType type);

        /// <summary>
        /// Returns a collection of all state records in the specified time range.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        Task<ICollection<State>> GetStates(DateTime from, DateTime to);

        /// <summary>
        /// Returns a collection of all repeating states, optionally bounded by their effective dates.
        /// </summary>
        /// <param name="effectiveFrom">If not null, only returns repeating states with their effective from date
        /// being after or equal to the specified value.</param>
        /// <param name="effectiveTo">If not null, only returns repeating states with their effective to date
        /// being before or equal to the specified value.</param>
        /// <returns></returns>
        Task<ICollection<RepeatingState>> GetRepeatingStates(DateTime? effectiveFrom = null,
            DateTime? effectiveTo = null);

        /// <summary>
        /// Returns a collection of all repeating states that are effective at the specified date.
        /// </summary>
        /// <param name="effectiveAt"></param>
        /// <returns></returns>
        Task<ICollection<RepeatingState>> GetRepeatingStates(DateTime effectiveAt);

        /// <summary>
        /// Returns a collection of all states that were planned based on the specified repeating state.
        /// </summary>
        /// <param name="repeatingStateId"></param>
        /// <param name="futureOnly"></param>
        /// <returns></returns>
        Task<ICollection<State>> GetStatesForRepeatingState(int repeatingStateId, bool futureOnly = true);

        /// <summary>
        /// Creates a new repeating state record and plans states accordingly.
        /// </summary>
        /// <returns></returns>
        Task<RepeatingStatePlanningResult> MakeRepeatingState(RepeatingState newRepeatingState);

        /// <summary>
        /// Removes a repeating state record if its effective span hasn't started yet.
        /// If it has, deletes all its planned states that haven't started yet and changes its
        /// <see cref="RepeatingState.EffectiveTo"/> to today, making it ended and immutable.
        /// </summary>
        /// <param name="repeatingStateId">The ID of the repeating state to remove or end.</param>
        /// <param name="removedById">The ID of the user performing the operation.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
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
        /// <exception cref="InvalidOperationException">Thrown when one of the restrictions is violated.</exception>
        /// <returns></returns>
        Task<RepeatingStatePlanningResult> ModifyRepeatingState(RepeatingStateModification modification,
            int changeMadeByUserId);

        /// <summary>
        /// Creates a new planned state record.
        /// </summary>
        /// <remarks>
        /// A state cannot be planned if it begins earlier than an already planned state that it would overlap.
        /// However, it can be planned so that in begins in the middle of an already planned state. In that case,
        /// the <see cref="State.PlannedEnd"/> of the previously planned state is set to the newly planned state's
        /// beginning and its next state reference is set to the newly planned state.<br/>
        /// </remarks>
        /// <returns></returns>
        Task<StatePlanningResult> PlanState(NewState newState);

        /// <summary>
        /// Removes the specified planned state record.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified state has already started or ended.</exception>
        /// <exception cref="StateNotFoundException">Thrown when no state with the specified <paramref name="id"/>
        /// exists.</exception>
        /// <returns></returns>
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
        /// <exception cref="InvalidOperationException">Thrown when one of the restrictions is violated.</exception>
        /// <exception cref="StateNotFoundException">Thrown when no state with the specified
        /// <see cref="StateModification.StateId"/> exists.</exception>
        /// <returns></returns>
        Task<StatePlanningResult> ModifyState(StateModification stateModification, int changeMadeByUserId);

        /// <summary>
        /// Sets the current state as closed.
        /// </summary>
        /// <remarks>
        /// This sets the state's <see cref="State.Ended"/> to the current datetime.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the current state is <see cref="StateType.Closed"/> or when the specified user
        /// is not a state manager.</exception>
        /// <returns></returns>
        Task CloseNow(int closedByUserId);
    }
}
