// IStateTransitionHandler.cs
// Author: Ondřej Ondryáš

using System.Threading.Tasks;

namespace KachnaOnline.Business.Services.StatePlanning.Abstractions
{
    /// <summary>
    /// Represents a single type of action that happens when a state starts or ends.
    /// </summary>
    public interface IStateTransitionHandler
    {
        /// <summary>
        /// Perform this handler's action for the start of the specified state.
        /// </summary>
        /// <param name="stateId">The ID of the state that has started.</param>
        /// <param name="previousStateId">The ID of the previous state. Null if no state directly precedes this one.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task PerformStartAction(int stateId, int? previousStateId);

        /// <summary>
        /// Perform this handler's action for the end of the specified state.
        /// </summary>
        /// <param name="stateId">The ID of the state that has ended.</param>
        /// <param name="nextStateId">The ID of the following state. Null if no state comes right after this one.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task PerformEndAction(int stateId, int? nextStateId);
    }
}
