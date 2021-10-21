// IStatePlanner.cs
// Author: Ondřej Ondryáš

using System;
using System.Threading;
using System.Threading.Tasks;
using KachnaOnline.Business.Services.Abstractions;

namespace KachnaOnline.Business.Services.StatePlanning.Abstractions
{
    /// <summary>
    /// Represents a singleton service that is used to control the <see cref="StatePlannerBackgroundService"/>.
    /// A <see cref="IClubStateService"/> uses the <see cref="NotifyPlanChanged"/> method to notify the background
    /// service of a plan change. The background service then uses <see cref="GetNextPlannedTransition"/> to poll the
    /// upcoming state change (start or end) and reschedule its waiting for this event.
    /// </summary>
    public interface IStatePlannerService
    {
        /// <summary>
        /// Carries information about the closest upcoming state transition and a <see cref="CancellationToken"/> that
        /// can be observed to get notified about invalidation of the plan.
        /// Created by <see cref="IStatePlannerService.GetNextPlannedTransition"/>.
        /// </summary>
        public readonly struct StatePlannerResult
        {
            /// <summary>
            /// A <see cref="CancellationToken"/> the cancellation of which signalizes that a plan change has occurred
            /// and this transition may no longer be the closest next one.
            /// </summary>
            public CancellationToken CancellationToken { get; init; }

            /// <summary>
            /// True if there's a specific transition to wait for; false otherwise.
            /// </summary>
            public bool PlanExists { get; init; }

            /// <summary>
            /// The ID of the state that triggers should be run for.
            /// </summary>
            /// <remarks>
            /// If <see cref="PlanExists"/> is false, the value of this property has no meaning.
            /// </remarks>
            public int StateId { get; init; }

            /// <summary>
            /// The ID of the immediately following state that start triggers should be run for.
            /// </summary>
            public int? NextStateId { get; init; }

            /// <summary>
            /// The datetime of the closest planned transition.
            /// </summary>
            /// <remarks>
            /// If <see cref="PlanExists"/> is false, the value of this property has no meaning.
            /// </remarks>
            public DateTime TransitionDate { get; init; }

            /// <summary>
            /// True if the next event to wait for is the end of the current state; false if the event is the start
            /// of a planned state. 
            /// </summary>
            public bool IsStateEnd { get; init; }
        }

        /// <summary>
        /// Returns information about the next planned transition (current state end or the closest planned state start). 
        /// A <see cref="CancellationToken"/> is included in the returned value that signalizes the plan service that
        /// the returned transition may no longer be the closest next one.
        /// </summary>
        /// <remarks>
        /// The method may asynchronously wait if <see cref="NotifyPlanChanged"/> or <see cref="GetNextPlannedTransition"/>
        /// is currently in process.
        /// </remarks>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe when waiting.</param>
        /// <returns>A <see cref="StatePlannerResult"/> with details about the next planned transition and with
        /// a <see cref="CancellationToken"/> that can be observed to get notified about future plan changes.</returns>
        Task<StatePlannerResult> GetNextPlannedTransition(CancellationToken cancellationToken = default);

        /// <summary>
        /// Notifies the plan service that the plan has been changed by triggering cancellation of the
        /// <see cref="CancellationToken"/> that was returned when <see cref="GetNextPlannedTransition"/> had
        /// last been called.
        /// </summary>
        /// <remarks>
        /// The method may asynchronously wait if <see cref="NotifyPlanChanged"/> or <see cref="GetNextPlannedTransition"/>
        /// is currently in process.
        /// </remarks>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe when waiting.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task NotifyPlanChanged(CancellationToken cancellationToken = default);
    }
}
