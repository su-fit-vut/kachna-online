using System.Threading.Tasks;
using KachnaOnline.Business.Models.ClubStates;

namespace KachnaOnline.Business.Services.StatePlanning.Abstractions
{
    /// <summary>
    /// Represents a service that performs various actions when a state begins or ends.
    /// That includes, for example, sending messages to Discord, Facebook etc.
    /// </summary>
    public interface IStateTransitionService
    {
        /// <summary>
        /// Performs the state start actions.
        /// </summary>
        /// <param name="stateId">The ID of the state that has started.</param>
        /// <param name="previousStateId">The ID of the previous state. Null if no state directly precedes this one.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task TriggerStateStart(int stateId, int? previousStateId);

        /// <summary>
        /// Performs the state modification actions.
        /// </summary>
        /// <param name="previousState"></param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task TriggerStateModification(State previousState);

        /// <summary>
        /// Performs the state end actions.
        /// </summary>
        /// <param name="stateId">The ID of the state that has ended.</param>
        /// <param name="nextStateId">The ID of the following state. Null if no state comes right after this one.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task TriggerStateEnd(int stateId, int? nextStateId);
    }
}
