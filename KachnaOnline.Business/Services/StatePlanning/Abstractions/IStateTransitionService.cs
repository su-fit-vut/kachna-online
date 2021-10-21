// IStateTransitionService.cs
// Author: Ondřej Ondryáš

using System.Threading.Tasks;

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
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task TriggerStateStart(int stateId);

        /// <summary>
        /// Performs the state end actions.
        /// </summary>
        /// <param name="stateId">The ID of the state that has ended.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task TriggerStateEnd(int stateId);
    }
}
