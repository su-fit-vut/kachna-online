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
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task PerformStartAction(int stateId);

        /// <summary>
        /// Perform this handler's action for the end of the specified state. 
        /// </summary>
        /// <param name="stateId">The ID of the state that has ended.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task PerformEndAction(int stateId);
    }
}
