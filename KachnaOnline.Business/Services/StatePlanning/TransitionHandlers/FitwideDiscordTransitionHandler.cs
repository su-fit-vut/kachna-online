// FitwideDiscordTransitionHandler.cs
// Author: Ondřej Ondryáš

using System;
using System.Net.Http;
using System.Threading.Tasks;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Business.Extensions;
using KachnaOnline.Business.Models.ClubStates;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Business.Services.Discord;
using KachnaOnline.Business.Services.StatePlanning.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KachnaOnline.Business.Services.StatePlanning.TransitionHandlers
{
    public class FitwideDiscordTransitionHandler : DiscordWebhookClient, IStateTransitionHandler
    {
        private const string PeepoLove = "<:peepolove:827833315666558996>";
        private const string PeepoSushiRoll = "<:peeposushiroll:827833316290854932>";
        private const string Hypers = "<:HYPERS:493154327318233088>";

        private readonly ILogger<FitwideDiscordTransitionHandler> _logger;
        private readonly IClubStateService _stateService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public FitwideDiscordTransitionHandler(IOptionsMonitor<ClubStateOptions> options,
            IHttpClientFactory httpClientFactory, ILogger<FitwideDiscordTransitionHandler> logger,
            IClubStateService stateService, IUserService userService, IUnitOfWork unitOfWork)
            : base(options.CurrentValue.FitwideDiscordWebhookUrl, httpClientFactory, logger)
        {
            _logger = logger;
            _stateService = stateService;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task PerformStartAction(int stateId, int? previousStateId)
        {
            var state = await _stateService.GetState(stateId);
            var previousState = previousStateId.HasValue ? await _stateService.GetState(previousStateId.Value) : null;

            var madeBy = state.MadeById.HasValue ? await _userService.GetUser(state.MadeById.Value) : null;
            var madeByName = madeBy.GetDiscordMention(false);

            string msg = null;

            if (state.Type == StateType.OpenChillzone)
            {
                if (!state.PlannedEnd.HasValue)
                {
                    _logger.LogCritical("Data inconsistency: chillzone with no planned end (ID {Id}).", state.Id);
                    return;
                }

                if (previousState?.Type == StateType.OpenChillzone
                    && previousState.PlannedEnd.HasValue)
                {
                    var timeDelta = state.PlannedEnd.Value - previousState.PlannedEnd.Value;
                    if (timeDelta < TimeSpan.Zero)
                    {
                        msg = $"Chillzóna je zkrácena do **{state.PlannedEnd.Value:HH:mm}** {PeepoSushiRoll}";
                        if (state.NotePublic != null)
                        {
                            msg += " " + state.NotePublic;
                        }
                    }
                    else if (timeDelta > TimeSpan.FromMinutes(15))
                    {
                        msg = $"Chillzóna je prodloužena do **{state.PlannedEnd.Value:HH:mm}** {Hypers}";
                        if (state.NotePublic != null)
                        {
                            msg += " " + state.NotePublic;
                        }
                    }

                    if (previousState.MadeById != state.MadeById
                        && madeByName != null)
                        msg += $" Nyní pro vás otevírá {madeByName} {PeepoLove}";
                }
                else
                {
                    var openTillString = $"**do {state.PlannedEnd.Value:HH:mm}**";
                    var nextBarOpening = await _stateService.GetNextPlannedState(StateType.OpenBar);
                    if (nextBarOpening != null &&
                        nextBarOpening.Start - state.PlannedEnd.Value < TimeSpan.FromMinutes(15))
                    {
                        openTillString = $"až do otvíračky (v {nextBarOpening.Start:HH:mm})";
                    }

                    msg = $"Kachna je otevřena v režimu chillzóna {openTillString}! {Hypers}";
                    if (madeByName != null)
                    {
                        msg += $" Otevírá pro vás {madeByName} {PeepoLove}";
                    }

                    if (state.NotePublic != null)
                    {
                        msg += " " + state.NotePublic;
                    }
                }
            }
            else if (state.Type == StateType.OpenBar &&
                     (previousState is null || previousState.Type != StateType.OpenBar))
            {
                msg = "Máme tady otvíračku!";

                if (state.PlannedEnd.HasValue)
                {
                    msg += $" Končíme ve {state.PlannedEnd.Value:HH:mm} {Hypers} {state.NotePublic}";
                }
            }

            if (msg != null)
            {
                var result = await this.SendWebhookMessage(msg, true);
                if (result != null)
                {
                    await this.SaveMessageId(stateId, result.Id);
                }
            }
        }

        private async Task SaveMessageId(int stateId, ulong messageId)
        {
            var stateEntity = await _unitOfWork.PlannedStates.Get(stateId);
            stateEntity.DiscordNotificationId = messageId;

            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot save Discord message ID.");
            }
        }

        private async Task<ulong?> GetMessageId(int stateId)
        {
            var stateEntity = await _unitOfWork.PlannedStates.Get(stateId);
            return stateEntity?.DiscordNotificationId;
        }

        public async Task PerformEndAction(int stateId, int? nextStateId)
        {
            var messageId = await this.GetMessageId(stateId);
            if (messageId.HasValue)
            {
                await this.DeleteWebhookMessage(messageId.Value);
            }
        }
    }
}
