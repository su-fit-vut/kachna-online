// StatePlannerBackgroundService.cs
// Author: Ondřej Ondryáš

using System;
using System.Threading;
using System.Threading.Tasks;
using KachnaOnline.Business.Services.StatePlanning.Abstractions;
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
                }

                _logger.LogDebug(
                    "Invoking transition triggers for state {NextStateId} (end transition: {IsEndTransition}).",
                    nextTransition.StateId, nextTransition.IsStateEnd);

                // Process the triggers in a 'fire-and-forget' fashion – we don't really have to wait for
                // the HTTP requests to finish...
                _ = Task.Run(async () =>
                {
                    try
                    {
                        // Create a scope for the scoped transition service
                        using var scope = _serviceProvider.CreateScope();
                        var transitionService = scope.ServiceProvider.GetRequiredService<IStateTransitionService>();

                        // Process the triggers 
                        if (nextTransition.IsStateEnd)
                        {
                            await transitionService.TriggerStateEnd(nextTransition.StateId);
                            
                            if (nextTransition.NextStateId.HasValue)
                            {
                                await transitionService.TriggerStateStart(nextTransition.NextStateId.Value);
                            }
                        }
                        else
                        {
                            await transitionService.TriggerStateStart(nextTransition.StateId);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogCritical(e,
                            "An exception occurred when processing the state transition triggers for state {StateId}.",
                            nextTransition.StateId);
                    }
                }).ConfigureAwait(false);
            }

            _logger.LogInformation("The state planning service has ended.");
        }
    }
}
