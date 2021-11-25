// StateTransitionService.cs
// Author: Ondřej Ondryáš

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KachnaOnline.Business.Services.StatePlanning.Abstractions;
using Microsoft.Extensions.Logging;

namespace KachnaOnline.Business.Services.StatePlanning
{
    public class StateTransitionService : IStateTransitionService
    {
        private readonly IStateTransitionHandler[] _transitionHandlers;
        private readonly ILogger<StateTransitionService> _logger;

        public StateTransitionService(IEnumerable<IStateTransitionHandler> transitionHandlers,
            ILogger<StateTransitionService> logger)
        {
            _transitionHandlers = transitionHandlers.ToArray();
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task TriggerStateStart(int stateId, int? previousStateId)
        {
            _logger.LogDebug("Processing trigger actions for the start of state {StateId}.", stateId);

            foreach (var transitionHandler in _transitionHandlers)
            {
                try
                {
                    await transitionHandler.PerformStartAction(stateId, previousStateId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occurred when performing start action {ActionName}.",
                        transitionHandler.GetType().Name);
                }
            }
        }

        /// <inheritdoc />
        public async Task TriggerStateEnd(int stateId, int? nextStateId)
        {
            _logger.LogDebug("Processing trigger actions for the end of state {StateId}.", stateId);

            foreach (var transitionHandler in _transitionHandlers)
            {
                try
                {
                    await transitionHandler.PerformEndAction(stateId, nextStateId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occurred when performing end action {ActionName}.",
                        transitionHandler.GetType().Name);
                }
            }
        }
    }
}
