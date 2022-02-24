using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KachnaOnline.Business.Models.Discord;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace KachnaOnline.Business.Services.Discord
{
    public abstract class DiscordWebhookClient
    {
        private readonly string _webhookUrl;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DiscordWebhookClient> _logger;

        internal DiscordWebhookClient(IHttpClientFactory httpClientFactory, ILogger<DiscordWebhookClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        internal DiscordWebhookClient(string webhookUrl, IHttpClientFactory httpClientFactory,
            ILogger<DiscordWebhookClient> logger) : this(httpClientFactory, logger)
        {
            _webhookUrl = webhookUrl;
        }

        /// <summary>
        /// Sends a Discord webhook message.
        /// </summary>
        /// <param name="message">Message to send.</param>
        /// <param name="wait">Whether to wait for server message confirmation. False by default.</param>
        /// <param name="webhookUrl">URL of the target Discord webhook.</param>
        /// <returns>Message model if <paramref name="wait"/> was true and the request succeeded, null otherwise.</returns>
        protected async Task<DiscordMessage> SendWebhookMessage(string message, bool wait = false,
            string webhookUrl = null)
        {
            webhookUrl ??= _webhookUrl;

            if (string.IsNullOrEmpty(webhookUrl))
            {
                _logger.LogError("Webhook URL not available");
                return null;
            }

            if (string.IsNullOrEmpty(message))
            {
                _logger.LogError("Empty message.");
                return null;
            }

            using var client = _httpClientFactory.CreateClient();
            var content = $"{{ \"content\": \"{message}\" }}";
            var waitString = wait.ToString().ToLower();

            try
            {
                _logger.LogDebug("Sending a Discord webhook message.");
                var response = await client.PostAsync($"{webhookUrl}?wait={waitString}",
                    new StringContent(content, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Webhook response payload: {data}", responseData);
                return JsonConvert.DeserializeObject<DiscordMessage>(responseData);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot send Discord webhook message");
                return null;
            }
        }

        protected async Task<DiscordMessage> ModifyWebhookMessage(ulong messageId, string message,
            string webhookUrl = null)
        {
            webhookUrl ??= _webhookUrl;

            if (string.IsNullOrEmpty(webhookUrl))
            {
                _logger.LogError("Webhook URL not available");
                return null;
            }

            if (string.IsNullOrEmpty(message))
            {
                _logger.LogError("Empty message.");
                return null;
            }

            using var client = _httpClientFactory.CreateClient();
            var content = $"{{ \"content\": \"{message}\" }}";

            try
            {
                _logger.LogDebug("Updating a Discord webhook message.");
                var response = await client.PatchAsync($"{webhookUrl}/messages/{messageId}",
                    new StringContent(content, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Webhook response payload: {data}", responseData);
                return JsonConvert.DeserializeObject<DiscordMessage>(responseData);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot update Discord webhook message");
                return null;
            }
        }

        /// <summary>
        /// Deletes a Discord webhook message.
        /// </summary>
        /// <param name="messageId">ID of the message to delete.</param>
        /// <param name="webhookUrl">URL of the target Discord webhook.</param>
        protected async Task DeleteWebhookMessage(ulong messageId, string webhookUrl = null)
        {
            webhookUrl ??= _webhookUrl;

            if (string.IsNullOrEmpty(webhookUrl))
            {
                _logger.LogError("Webhook URL not available");
                return;
            }

            _logger.LogDebug("Requesting removal of webhook message {Id}.", messageId);
            using var client = _httpClientFactory.CreateClient();
            await client.DeleteAsync($"{webhookUrl}/messages/{messageId}");
        }
    }
}
