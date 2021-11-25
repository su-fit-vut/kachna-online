using System.Net.Http;
using System.Threading.Tasks;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Services.Discord;
using KachnaOnline.Business.Services.StatePlanning.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KachnaOnline.Business.Services.StatePlanning.TransitionHandlers
{
    public class FitwideDiscordTransitionHandler : DiscordWebhookClient, IStateTransitionHandler
    {
        private readonly ILogger<FitwideDiscordTransitionHandler> _logger;

        public FitwideDiscordTransitionHandler(IOptionsMonitor<ClubStateOptions> options,
            IHttpClientFactory httpClientFactory,
            ILogger<FitwideDiscordTransitionHandler> logger)
            : base(options.CurrentValue.FitwideDiscordWebhookUrl, httpClientFactory, logger)
        {
            _logger = logger;
        }

        public Task PerformStartAction(int stateId, int? previousStateId)
        {
            throw new System.NotImplementedException();
        }

        public Task PerformEndAction(int stateId, int? nextStateId)
        {
            throw new System.NotImplementedException();
        }
    }
}
