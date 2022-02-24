using System;
using System.Net.Http;
using System.Threading.Tasks;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Extensions;
using KachnaOnline.Business.Models.ClubStates;
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
        private readonly IUserService _userService;

        public SuDiscordTransitionHandler(IOptionsMonitor<ClubStateOptions> options,
            IHttpClientFactory httpClientFactory,
            ILogger<SuDiscordTransitionHandler> logger, IClubStateService stateService,
            IUserService userService)
            : base(options.CurrentValue.SuDiscordWebhookUrl, httpClientFactory, logger)
        {
            _logger = logger;
            _stateService = stateService;
            _userService = userService;
        }

        public async Task PerformStartAction(int stateId, int? previousStateId)
        {
            var state = await _stateService.GetState(stateId);
            var previousState = previousStateId.HasValue ? await _stateService.GetState(previousStateId.Value) : null;

            var madeBy = state.MadeById.HasValue ? await _userService.GetUser(state.MadeById.Value) : null;
            var madeByName = madeBy.GetDiscordMention(true);

            string msg = null;

            if (state.Type == StateType.OpenChillzone)
            {
                if (!state.PlannedEnd.HasValue)
                {
                    _logger.LogCritical("Data inconsistency: chillzone with no planned end (ID {Id}).", state.Id);
                    return;
                }

                if (previousState?.Type == StateType.OpenChillzone)
                {
                    if (previousState.MadeById != state.MadeById)
                    {
                        msg = $"Předáno {madeByName} v {state.Start:HH:mm}. ";
                    }

                    if (previousState.PlannedEnd.HasValue)
                    {
                        var timeDelta = previousState.PlannedEnd.Value - state.PlannedEnd.Value;

                        if (timeDelta > TimeSpan.Zero)
                        {
                            msg += $"Zkráceno do {state.PlannedEnd.Value:HH:mm}.";
                        }
                        else if (timeDelta < TimeSpan.Zero)
                        {
                            msg += $"Prodlouženo do {state.PlannedEnd.Value:HH:mm}.";
                        }
                    }
                }
                else
                {
                    msg = $"Otevřeno od {state.Start:HH:mm} do {state.PlannedEnd.Value:HH:mm}. Otevírá {madeByName}.";

                    if (state.NoteInternal != null)
                    {
                        msg += "\\nInterní poznámka: " + state.NoteInternal;
                    }

                    if (state.NotePublic != null)
                    {
                        msg += "\\nVeřejná poznámka: " + state.NotePublic;
                    }
                }
            }
            else if (state.Type == StateType.OpenBar && previousState?.Type == StateType.OpenChillzone)
            {
                msg = "Ukončeno otvíračkou.";
            }

            if (msg != null)
            {
                _logger.LogDebug("Sending chillzone start message to SU Discord server.");
                await this.SendWebhookMessage(msg);
            }
        }

        public async Task PerformModifyAction(State previousState)
        {
            if (previousState.Ended.HasValue || previousState.Start > DateTime.Now)
                return;

            var state = await _stateService.GetState(previousState.Id);
            var madeBy = state.MadeById.HasValue ? await _userService.GetUser(state.MadeById.Value) : null;
            var madeByName = madeBy.GetDiscordMention(true);

            await this.SendWebhookMessage(
                $"Aktuální stav pozměněn – aktuální konec: {state.PlannedEnd:HH:mm}, otevírá: {madeByName}.");
        }

        public async Task PerformEndAction(int stateId, int? nextStateId)
        {
            var state = await _stateService.GetState(stateId);
            if (state.Type == StateType.OpenChillzone && !nextStateId.HasValue)
            {
                _logger.LogDebug("Sending chillzone end message to SU Discord server.");
                await this.SendWebhookMessage($"Zavřeno v {DateTime.Now:HH:mm}.");
            }
        }
    }
}
