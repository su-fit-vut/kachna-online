using System.Net.Http;
using System.Threading.Tasks;
using KachnaOnline.Business.Services.StatePlanning.Abstractions;
using Microsoft.Extensions.Logging;

namespace KachnaOnline.Business.Services.StatePlanning.TransitionHandlers
{
    public class DiscordTransitionHandler : IStateTransitionHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DiscordTransitionHandler> _logger;

        public DiscordTransitionHandler(IHttpClientFactory httpClientFactory,
            ILogger<DiscordTransitionHandler> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public Task PerformStartAction(int stateId)
        {
            _logger.LogInformation("Discord TH start action for {Id}", stateId);
            return Task.CompletedTask;
        }

        public Task PerformEndAction(int stateId)
        {
            _logger.LogInformation("Discord TH end action for {Id}", stateId);
            return Task.CompletedTask;
        }
    }
}
