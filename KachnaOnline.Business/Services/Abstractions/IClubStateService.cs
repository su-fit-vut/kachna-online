// IClubStateService.cs
// Author: Ondřej Ondryáš

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        Task MakeRepeatingState(RepeatingState newRepeatingState);

        /// <summary>
        /// Removes a repeating state record and all planned states that were based on it.
        /// State records from the past are not removed.
        /// </summary>
        /// <param name="repeatingStateId"></param> 
        /// <returns></returns>
        Task RemoveRepeatingState(int repeatingStateId);

        /// <summary>
        /// Changes details of the specified repeating state. Projects these changes into all
        /// planned states that were based on it. State records from the past are not changed.
        /// </summary>
        /// <remarks>
        /// When one of <see cref="RepeatingState.EffectiveFrom"/> or <see cref="RepeatingState.EffectiveTo"/>
        /// is changed, new states may be planned or existing planned states may be removed. These properties
        /// cannot be changed if the dates have already passed.<br/>
        /// <see cref="RepeatingState.EffectiveTo"/> cannot be changed to a date in the past.<br/>
        /// Repeating states of which the <see cref="RepeatingState.EffectiveTo"/>
        /// has already passed cannot be modified at all.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown when one of the restrictions is violated.</exception>
        /// <returns></returns>
        Task ModifyRepeatingState(RepeatingState repeatingState, int changeMadeByUserId);

        /// <summary>
        /// Creates a new planned state record.
        /// </summary>
        /// <remarks>
        /// A state cannot be planned if it begins earlier than an already planned state that it would overlap.
        /// However, it can be planned so that in begins in the middle of an already planned state. In that case,
        /// the <see cref="State.PlannedEnd"/> of the previously planned state set to the newly planned state
        /// beginning and its next state reference is set to the newly planned state.<br/>
        /// </remarks>
        /// <returns></returns>
        Task PlanState(State newState);

        /// <summary>
        /// Removes the specified planned state record.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified state has already started or ended.</exception>
        /// <exception cref="ArgumentException">Thrown when no state with the specified <paramref name="id"/>
        /// exists.</exception>
        /// <returns></returns>
        Task RemovePlannedState(int id);

        /// <summary>
        /// Changes details of the specified state (current or planned).
        /// </summary>
        /// <remarks>
        /// For the current state and planned states, the planned end and both notes can be changed.
        /// Planned end must be set to a date after the state's start.
        /// For planned states, the planned start can also be changed.
        /// Planned start must be set to a date after now and before the state's planned end.
        /// For states that already ended, only the internal note can be changed.
        /// Administrators (only) can also change the MadeById.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown when one of the restrictions is violated.</exception>
        /// <exception cref="ArgumentException">Thrown when no state with the specified
        /// <see cref="StateModification.StateId"/> exists.</exception>
        /// <returns></returns>
        Task ModifyState(StateModification stateModification, int changeMadeByUserId);

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
