using System;
using System.Threading;
using System.Threading.Tasks;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Business.Services.StatePlanning.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace KachnaOnline.Business.Services.StatePlanning
{
    public class StatePlannerService : IStatePlannerService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly SemaphoreSlim _semaphore;
        private CancellationTokenSource _cts;

        public StatePlannerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _cts = new CancellationTokenSource();
            _semaphore = new SemaphoreSlim(1);
        }

        /// <inheritdoc />
        public async Task NotifyPlanChanged(CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);

            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();

            _semaphore.Release();
        }

        /// <inheritdoc />
        public async Task<IStatePlannerService.StatePlannerResult> GetNextPlannedTransition(
            CancellationToken cancellationToken)
        {
            try
            {
                await _semaphore.WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return new IStatePlannerService.StatePlannerResult() { PlanExists = false };
            }

            using var scope = _serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var uow = services.GetRequiredService<IUnitOfWork>();
            var states = uow.PlannedStates;
            IStatePlannerService.StatePlannerResult result;

            // Check if a state is currently in progress
            var currentState = await states.GetCurrent();
            if (currentState != null)
            {
                // If so, plan its end triggers
                result = new IStatePlannerService.StatePlannerResult()
                {
                    StateId = currentState.Id,
                    NextStateId = currentState.NextPlannedStateId,
                    CancellationToken = _cts.Token,
                    TransitionDate = currentState.PlannedEnd,
                    IsStateEnd = true,
                    PlanExists = true
                };
            }
            else
            {
                // If no state is currently in progress, check for the next planned one
                var nextState = await states.GetNearest();

                result = nextState == null
                    ? new IStatePlannerService.StatePlannerResult()
                    {
                        PlanExists = false,
                        CancellationToken = _cts.Token
                    }
                    : new IStatePlannerService.StatePlannerResult()
                    {
                        StateId = nextState.Id,
                        CancellationToken = _cts.Token,
                        TransitionDate = nextState.Start,
                        IsStateEnd = false,
                        PlanExists = true
                    };
            }

            _semaphore.Release();
            return result;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _semaphore?.Dispose();
            _cts?.Dispose();
        }
    }
}
