// KisService.cs
// Author: Ondřej Ondryáš

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Models.Kis;
using KachnaOnline.Business.Services.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KachnaOnline.Business.Services
{
    public class KisService : IKisService
    {
        /// <summary>
        /// A model for JSONs returned by the KIS login endpoint and refresh token endpoint.
        /// </summary>
        private class TokenResponseModel
        {
            [JsonPropertyName("auth_token")] public string AuthToken { get; set; }
            [JsonPropertyName("expires_at")] public string ExpiresAt { get; set; }
            [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; }
        }

        private const string LoginEndpoint = "auth/eduid/login?session={0}";
        private const string RefreshTokenEndpoint = "auth/fresh_token?refresh_token={0}";
        private const string UserInfoEndpoint = "users/me";
        private const string ArticlesOfferedEndpoint = "articles/offered?stock_status=true";
        private const string TapEndpoint = "beer/taps/{0}";
        private const string LeaderboardEndpoint = "users/leaderboard?time_from={0}&time_to={1}&count={2}";

        private const string TapCacheKey = nameof(KisService) + ".Taps.";
        private const string OfferCacheKey = nameof(KisService) + ".Offer";
        private const string LeaderboardCacheKey = nameof(KisArticle) + ".Leaderboards.{0}.{1}.{2}";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptionsMonitor<KisOptions> _kisOptionsMonitor;
        private readonly IMemoryCache _cache;
        private readonly ILogger<KisService> _logger;

        public KisService(IHttpClientFactory httpClientFactory, IOptionsMonitor<KisOptions> kisOptionsMonitor,
            IMemoryCache cache, ILogger<KisService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _kisOptionsMonitor = kisOptionsMonitor;
            _cache = cache;
            _logger = logger;
        }

        /// <inheritdoc />
        /// <remarks>
        /// KIS may return HTTP 202 which means that it hasn't yet received a response from the SSO
        /// (the external eduID login provider).
        /// In that case, this method waits two seconds and then attempts to call the endpoint again.
        /// This is done maximum three times; if it fails, null is returned.
        /// </remarks>
        public async Task<KisIdentity> GetIdentityFromSession(string sessionId)
        {
            _logger.LogDebug("Creating KIS identity from session ID {SessionId}.",
                sessionId is null or { Length: <6 } ? sessionId : sessionId[..6] + "...");

            if (string.IsNullOrEmpty(sessionId))
                return null;

            return await this.GetIdentityFromSession(sessionId, 0);
        }

        /// <inheritdoc />
        public async Task<KisIdentity> GetIdentityFromRefreshToken(string refreshToken)
        {
            _logger.LogDebug("Creating KIS identity from refresh token {RefreshToken}.",
                refreshToken is null or { Length: <6 } ? refreshToken : refreshToken[..6] + "...");

            if (string.IsNullOrEmpty(refreshToken))
                return null;

            var client = _httpClientFactory.CreateClient(KisConstants.KisHttpClient);

            // Fetch new tokens
            var tokenResponse = await client.GetAsync(string.Format(RefreshTokenEndpoint, refreshToken));

            if (tokenResponse.IsSuccessStatusCode)
            {
                var responseStream = await tokenResponse.Content.ReadAsStreamAsync();
                return await this.MakeIdentityFromTokenResponse(responseStream, client);
            }
            else
            {
                _logger.LogInformation("Invalid token refresh attempt (KIS fresh_token returned code {Code}).",
                    tokenResponse.StatusCode);

                if (tokenResponse.StatusCode == HttpStatusCode.NotFound)
                    return new KisIdentity();
                else
                    return null;
            }
        }

        /// <inheritdoc />
        public async Task<ICollection<KisTapInfo>> GetTapInfo()
        {
            var tapIds = _kisOptionsMonitor.CurrentValue.TapIds;
            if (tapIds is null or { Length: 0 })
            {
                _logger.LogDebug("No tap IDs configured, returning empty tap info.");
                return new List<KisTapInfo>();
            }

            _logger.LogDebug("Fetching tap info for taps {TapIds}.", tapIds);

            var client = _httpClientFactory.CreateClient(KisConstants.KisDisplayHttpClient);
            var tapInfos = new List<KisTapInfo>();
            var atLeastOneOk = false;

            foreach (var tapId in tapIds)
            {
                if (_cache.TryGetValue(TapCacheKey + tapId, out KisTapInfo i))
                {
                    _logger.LogDebug("Using cached value for tap {TapId}.", tapId);
                    tapInfos.Add(i);
                    atLeastOneOk = true;
                    continue;
                }

                try
                {
                    _logger.LogDebug("Sending request for tap {TapId}.", tapId);
                    var response = await client.GetAsync(string.Format(TapEndpoint, tapId));
                    response.EnsureSuccessStatusCode();

                    var info = await response.Content.ReadFromJsonAsync<KisTapInfo>(MakeJsonSerializerOptions());
                    tapInfos.Add(info);

                    _cache.Set(TapCacheKey + tapId, info,
                        TimeSpan.FromSeconds(_kisOptionsMonitor.CurrentValue.CacheExpirationTimesSeconds.Taps));
                    atLeastOneOk = true;
                }
                catch (HttpRequestException e)
                {
                    if (e.StatusCode is HttpStatusCode.NotFound)
                    {
                        _logger.LogWarning("Tap ID {TapId} doesn't exist.", tapId);
                    }
                    else if (e.StatusCode is HttpStatusCode.Forbidden)
                    {
                        _logger.LogError("Cannot fetch tap info: forbidden.");
                        return null;
                    }
                    else
                    {
                        _logger.LogError(e, "Cannot fetch tap info for tap {TapId}.", tapId);
                    }
                }
                catch (JsonException e)
                {
                    _logger.LogError(e, "Cannot deserialize tap info response for tap {TapId}.", tapId);
                }
            }

            if (atLeastOneOk)
                return tapInfos;

            return null;
        }

        /// <inheritdoc />
        public async Task<ICollection<KisArticle>> GetOfferedArticles()
        {
            if (_cache.TryGetValue(OfferCacheKey, out List<KisArticle> articles))
            {
                _logger.LogDebug("Using cached value for current offer.");
                return articles;
            }

            _logger.LogDebug("Fetching offered articles.");

            var client = _httpClientFactory.CreateClient(KisConstants.KisDisplayHttpClient);

            try
            {
                var response = await client.GetAsync(ArticlesOfferedEndpoint);
                response.EnsureSuccessStatusCode();

                articles = await response.Content.ReadFromJsonAsync<List<KisArticle>>(MakeJsonSerializerOptions());
                _cache.Set(OfferCacheKey, articles,
                    TimeSpan.FromSeconds(_kisOptionsMonitor.CurrentValue.CacheExpirationTimesSeconds.Offers));
                return articles;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Cannot fetch offer.");
                return null;
            }
            catch (JsonException e)
            {
                _logger.LogError(e, "Cannot deserialize offer response.");
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<IList<KisLeaderboardItem>> GetLeaderboard(DateTime from, DateTime to, int count)
        {
            var cacheKey = string.Format(LeaderboardCacheKey, from, to, count);
            if (_cache.TryGetValue(cacheKey, out List<KisLeaderboardItem> items))
            {
                _logger.LogDebug("Using cached leaderboard items ({Count} items, from {From} to {To}).",
                    count, from, to);
                return items;
            }

            _logger.LogDebug("Fetching {Count} leaderboard items from {From} to {To}.", count, from, to);

            var client = _httpClientFactory.CreateClient(KisConstants.KisDisplayHttpClient);

            try
            {
                var response = await client.GetAsync(string.Format(LeaderboardEndpoint,
                    Uri.EscapeDataString(from.ToString("yyyy-MM-ddTHH:mm:ssK")),
                    Uri.EscapeDataString(to.ToString("yyyy-MM-ddTHH:mm:ssK")), count));
                response.EnsureSuccessStatusCode();

                items = await response.Content.ReadFromJsonAsync<List<KisLeaderboardItem>>(MakeJsonSerializerOptions());
                _cache.Set(cacheKey, items,
                    TimeSpan.FromSeconds(_kisOptionsMonitor.CurrentValue.CacheExpirationTimesSeconds.Leaderboard));
                return items;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Cannot fetch leaderboard.");
                return null;
            }
            catch (JsonException e)
            {
                _logger.LogError(e, "Cannot deserialize leaderboard response.");
                return null;
            }
        }

        /// <inheritdoc cref="GetIdentityFromSession"/>
        /// <remarks>
        /// This is the actual implementation of <see cref="GetIdentityFromSession(string)"/>. It adds a
        /// <paramref name="repeatCounter"/> to count the number of attempts and stop after three unsuccessful ones.
        /// </remarks>
        /// <param name="repeatCounter">Number of performed attempts to fetch the tokens.</param>
        // ReSharper disable once InvalidXmlDocComment
        private async Task<KisIdentity> GetIdentityFromSession(string sessionId, int repeatCounter)
        {
            if (repeatCounter == 3)
            {
                _logger.LogWarning("KIS didn't get a response from the SSO.");
                return null;
            }

            var client = _httpClientFactory.CreateClient(KisConstants.KisHttpClient);

            // Fetch tokens
            var tokenResponse = await client.GetAsync(string.Format(LoginEndpoint, sessionId));

            if (tokenResponse.IsSuccessStatusCode)
            {
                if (tokenResponse.StatusCode == HttpStatusCode.Accepted)
                {
                    // KIS may return HTTP 202 which means that it hasn't yet received a response from the SSO
                    // We'll wait for a while and then repeat the request
                    // We'll attempt this three times before giving up and returning null
                    _logger.LogInformation("KIS is still waiting for a response from the SSO, waiting and retrying.");
                    await Task.Delay(2000);
                    return await this.GetIdentityFromSession(sessionId, repeatCounter + 1);
                }
                else
                {
                    var responseStream = await tokenResponse.Content.ReadAsStreamAsync();
                    return await this.MakeIdentityFromTokenResponse(responseStream, client);
                }
            }
            else
            {
                _logger.LogInformation("Invalid login attempt (KIS login returned code {Code}).",
                    tokenResponse.StatusCode);

                if (tokenResponse.StatusCode == HttpStatusCode.NotFound)
                    return new KisIdentity();
                else
                    return null;
            }
        }

        /// <summary>
        /// Reads a stream that contains a JSON with KIS token data, deserializes it and uses the authorization token
        /// to fetch the corresponding user data using <see cref="FetchIdentity"/>. Returns a <see cref="KisIdentity"/>
        /// object with the tokens and the fetched data.
        /// </summary>
        /// <param name="responseStream">A <see cref="Stream"/> that contains a JSON with KIS token data.</param>
        /// <param name="client">An <see cref="HttpClient"/> instance to use when fetching the user data.</param>
        /// <returns>A <see cref="KisIdentity"/> object with the user's KIS tokens and data; or null if
        /// <see cref="FetchIdentity"/> returns null (an error occurs when fetching the user data).</returns>
        private async Task<KisIdentity> MakeIdentityFromTokenResponse(Stream responseStream, HttpClient client)
        {
            try
            {
                var responseJson = await JsonSerializer.DeserializeAsync<TokenResponseModel>(responseStream);
                if (responseJson is null or { AuthToken: null } or { ExpiresAt: null } or { RefreshToken: null })
                    throw new JsonException(); // Handle nulls in the JSON in the same way as invalid JSONs.

                var expiryDate = DateTime.Parse(responseJson.ExpiresAt, DateTimeFormatInfo.InvariantInfo,
                    DateTimeStyles.AssumeUniversal);

                var userData = await this.FetchIdentity(responseJson.AuthToken, client);
                if (userData is null)
                    return null;

                var identity = new KisIdentity()
                {
                    AuthToken = responseJson.AuthToken,
                    RefreshToken = responseJson.RefreshToken,
                    AuthTokenExpiry = expiryDate.ToLocalTime(),
                    UserData = userData
                };

                _logger.LogInformation("Created KIS identity for user {UserId}.", userData.Id);
                return identity;
            }
            catch (JsonException e)
            {
                _logger.LogError(e, "Cannot deserialize KIS token response.");
                return null;
            }
        }

        /// <summary>
        /// Fetches user data from KIS using the specified authorization token <paramref name="authToken"/>
        /// and an existing <see cref="HttpClient"/> instance <paramref name="client"/>.
        /// </summary>
        /// <param name="authToken">The user's KIS authorization token.</param>
        /// <param name="client">An <see cref="HttpClient"/> instance.</param>
        /// <returns>A <see cref="KisUser"/> object with the fetched user data; or null when an error occurs.</returns>
        private async Task<KisUser> FetchIdentity(string authToken, HttpClient client)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, UserInfoEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Cannot fetch KIS user data.");
                return null;
            }

            var responseStream = await response.Content.ReadAsStreamAsync();
            try
            {
                var responseJson = await JsonSerializer.DeserializeAsync<KisUser>(responseStream,
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return responseJson;
            }
            catch (JsonException e)
            {
                _logger.LogError(e, "Cannot deserialize KIS user info response.");
                return null;
            }
        }

        private static JsonSerializerOptions MakeJsonSerializerOptions() =>
            new() { PropertyNameCaseInsensitive = true };
    }
}
