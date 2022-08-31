using System;
using System.Threading;
using System.Threading.Tasks;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Business.Services.StatePlanning.Abstractions;
using KachnaOnline.Business.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KachnaOnline.Business.Services.StatePlanning
{
    /// <summary>
    /// Represents a background service that waits for the upcoming planned state to start and calls
    /// <see cref="IStateTransitionService"/> to process the required tasks after it starts.
    /// </summary>
    public class StatePlannerBackgroundService : BackgroundService
    {
        private readonly ILogger<StatePlannerBackgroundService> _logger;
        private readonly IStatePlannerService _statePlannerService;
        private readonly IServiceProvider _serviceProvider;

        public StatePlannerBackgroundService(ILogger<StatePlannerBackgroundService> logger,
            IStatePlannerService statePlannerService, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _statePlannerService = statePlannerService;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting the state planning service.");

            // Do a consistency check: fix the Ended property of states in the past
            await using (var scope = _serviceProvider.CreateAsyncScope())
            {
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var badStates = uow.PlannedStates.GetPastNotEnded();
                await foreach (var state in badStates.WithCancellation(stoppingToken))
                {
                    state.Ended = state.NextPlannedStateId.HasValue ? state.NextPlannedState.Start : state.PlannedEnd;
                    _logger.LogInformation("Fixing up Ended date on state {StateId} to {Ended}.", state.Id,
                        state.Ended);
                }

                try
                {
                    await uow.SaveChanges();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Cannot save fixed states.");
                }
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                // Insert a slight delay to inhibit any timing issues
                await Task.Delay(1000, stoppingToken);

                // Get the next state transition (start or end) that we want to trigger
                _logger.LogDebug("Polling the next state transition.");
                var nextTransition = await _statePlannerService.GetNextPlannedTransition(stoppingToken);
                var linkedCts =
                    CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, nextTransition.CancellationToken);

                if (!nextTransition.PlanExists)
                {
                    // There's no planned state, wait indefinitely until a cancellation is requested
                    _logger.LogDebug("No state transition planned, waiting for a change of the state plan.");

                    try
                    {
                        await Task.Delay(-1, linkedCts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogDebug("Waiting cancelled, the state plan has been changed.");
                    }

                    continue;
                }

                var toWait = nextTransition.TransitionDate - DateTime.Now;
                if (toWait < TimeSpan.Zero)
                {
                    // This is a sanity check, this situation should never happen
                    _logger.LogError(
                        "The planner returned a state that has already started, waiting for another change of the state plan.");

                    try
                    {
                        await Task.Delay(-1, linkedCts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogDebug(
                            "Waiting from a previous erroneous state cancelled, the state plan has been changed.");
                    }

                    continue;
                }

                if (toWait > TimeSpan.Zero)
                {
                    var waitAgain = false;
                    // Task.Delay would throw, cap the waiting time
                    if (toWait.TotalMilliseconds > Int32.MaxValue)
                    {
                        toWait = TimeSpan.FromMilliseconds(Int32.MaxValue);
                        waitAgain = true;
                    }
                    _logger.LogDebug(
                        "Waiting for {WaitTimeSpan} until triggering the transition for state {NextStateId}.",
                        toWait, nextTransition.StateId);

                    try
                    {
                        await Task.Delay(toWait, linkedCts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        // Getting here means that the state plan has been modified and we have to start a new round
                        // of waiting for the newly planned state.
                        _logger.LogDebug("The last waiting was cancelled.");
                        continue;
                    }

                    if (waitAgain)
                    {
                        continue;
                    }
                }

                _logger.LogInformation(
                    "Invoking transition triggers for state {NextStateId} (end transition: {IsEndTransition}).",
                    nextTransition.StateId, nextTransition.IsStateEnd);

                // Process the triggers in a 'fire-and-forget' fashion – we don't really have to wait for
                // the HTTP requests to finish...
                TaskUtils.FireAndForget(_serviceProvider, _logger, async (services, logger) =>
                {
                    var transitionService = services.GetRequiredService<IStateTransitionService>();

                    // Process the triggers
                    if (nextTransition.IsStateEnd)
                    {
                        await transitionService.TriggerStateEnd(nextTransition.StateId, nextTransition.NextStateId);

                        if (nextTransition.NextStateId.HasValue)
                        {
                            logger.LogInformation(
                                "Invoking transition triggers for state {NextStateId} (end transition: {IsEndTransition}).",
                                nextTransition.NextStateId.Value, false);

                            await transitionService.TriggerStateStart(nextTransition.NextStateId.Value,
                                nextTransition.StateId);
                        }
                    }
                    else
                    {
                        await transitionService.TriggerStateStart(nextTransition.StateId, null);
                    }
                });
            }

            _logger.LogInformation("The state planning service has ended.");
        }
    }
}
