﻿using System.Net.Http;
using System.Threading.Tasks;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Business.Services.Discord;
using KachnaOnline.Business.Services.StatePlanning.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KachnaOnline.Business.Services.StatePlanning.TransitionHandlers
{
    public class SuDiscordTransitionHandler : DiscordWebhookClient, IStateTransitionHandler
    {
        private readonly ILogger<SuDiscordTransitionHandler> _logger;
        private readonly IClubStateService _stateService;

        public SuDiscordTransitionHandler(IOptionsMonitor<ClubStateOptions> options,
            IHttpClientFactory httpClientFactory,
            ILogger<SuDiscordTransitionHandler> logger, IClubStateService stateService)
            : base(options.CurrentValue.SuDiscordWebhookUrl, httpClientFactory, logger)
        {
            _logger = logger;
            _stateService = stateService;
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
