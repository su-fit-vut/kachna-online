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

        private string MakeMessage(State state, State previousState)
        {
            string msg = null;

            if (state.Type == StateType.OpenEvent)
            {
                msg = $"V Kachně začala akce: {state.NotePublic} {Hypers}";
            }
            else if (state.Type == StateType.OpenBar &&
                     (previousState is null || previousState.Type != StateType.OpenBar))
            {
                msg = $"Máme tady otvíračku! {Hypers} Aktuální nabídku najdete na https://su.fit.vut.cz/kachna/.";
            }
            else if (state.Type == StateType.OpenTearoom &&
                     (previousState is null || previousState.Type != StateType.OpenTearoom))
            {
                msg =
                    $"Máme tady čajovnu – klidnou otvíračku bez alkoholu. {Hypers} Nabídku čajů i dalšího občerstvení najdete na https://su.fit.vut.cz/kachna/.";
            }
            else
            {
                return null;
            }

            if (state.PlannedEnd.HasValue)
            {
                msg += $" Končíme ve {state.PlannedEnd.Value:HH:mm}.";
            }

            if (state.Type != StateType.OpenEvent)
            {
                if (!string.IsNullOrEmpty(state.NotePublic))
                {
                    msg += $"\\n{state.NotePublic}";
                }
            }

            return msg;
        }

        public async Task PerformStartAction(int stateId, int? previousStateId)
        {
            var state = await _stateService.GetState(stateId);
            if (state.Type is StateType.Private or StateType.Closed)
                return;

            var previousState = previousStateId.HasValue ? await _stateService.GetState(previousStateId.Value) : null;

            var msg = this.MakeMessage(state, previousState);
            if (msg != null)
            {
                var result = await this.SendWebhookMessage(msg, true);
                if (result != null)
                {
                    await this.SaveMessageId(stateId, result.Id);
                }
            }
        }

        public async Task PerformModifyAction(State previousState)
        {
            var state = await _stateService.GetState(previousState.Id);
            if (state.Type is StateType.Private or StateType.Closed)
                return;

            if (state.Start > DateTime.Now || state.Ended.HasValue)
                return;

            var discordMessageId = await this.GetMessageId(state.Id);
            if (!discordMessageId.HasValue)
                return;

            var msg = this.MakeMessage(state, null);
            if (msg != null)
            {
                await this.ModifyWebhookMessage(discordMessageId.Value, msg);
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
