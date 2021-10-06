// KisService.cs
// Author: Ondřej Ondryáš

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using KachnaOnline.Business.Models.Kis;
using KachnaOnline.Business.Services.Abstractions;
using Microsoft.Extensions.Logging;

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

        private const string LoginEndpoint = "auth/eduid/login";
        private const string RefreshTokenEndpoint = "auth/fresh_token";
        private const string UserInfoEndpoint = "users/me";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<KisService> _logger;

        public KisService(IHttpClientFactory httpClientFactory, ILogger<KisService> logger)
        {
            _httpClientFactory = httpClientFactory;
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
                (sessionId is null or { Length: <6 }) ? sessionId : (sessionId[..6] + "..."));

            if (string.IsNullOrEmpty(sessionId))
            {
                return null;
            }

            return await this.GetIdentityFromSession(sessionId, 0);
        }

        /// <inheritdoc />
        public async Task<KisIdentity> GetIdentityFromRefreshToken(string refreshToken)
        {
            _logger.LogDebug("Creating KIS identity from refresh token {RefreshToken}.",
                (refreshToken is null or { Length: <6 }) ? refreshToken : (refreshToken[..6] + "..."));

            if (string.IsNullOrEmpty(refreshToken))
            {
                return null;
            }

            var client = _httpClientFactory.CreateClient("kis");

            // Fetch new tokens
            var tokenResponse = await client.GetAsync($"{RefreshTokenEndpoint}?refresh_token={refreshToken}");

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
                {
                    return new KisIdentity();
                }
                else
                {
                    return null;
                }
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

            var client = _httpClientFactory.CreateClient("kis");

            // Fetch tokens
            var tokenResponse = await client.GetAsync($"{LoginEndpoint}?session={sessionId}");

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
                {
                    return new KisIdentity();
                }
                else
                {
                    return null;
                }
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
                {
                    return null;
                }

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
    }
}
