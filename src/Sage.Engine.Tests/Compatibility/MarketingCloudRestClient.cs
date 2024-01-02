// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using Polly;

namespace Sage.Engine.Tests.Compatibility
{
    /// <summary>
    /// A very simple REST client for testing compatibility of content
    /// </summary>
    internal class MarketingCloudRestClient : IMarketingCloudRestClient
    {
        private readonly string _clientId;
        readonly string _clientSecret;
        readonly int _mid;
        readonly Uri _restUri;
        readonly Uri _authUri;
        private readonly HttpClient _authHttpClient;
        private readonly HttpClient _restHttpClient;

        DateTime _expiration = DateTime.MinValue;

        public MarketingCloudRestClient()
        {
            this._clientId = Environment.GetEnvironmentVariable("MC_CLIENT_ID") ?? throw new InvalidDataException("MC_CLIENT_ID not set in the environment");
            this._clientSecret = Environment.GetEnvironmentVariable("MC_CLIENT_SECRET") ?? throw new InvalidDataException("MC_CLIENT_SECRET not set in the environment");
            this._mid = int.Parse(Environment.GetEnvironmentVariable("MC_CLIENT_MID") ?? throw new InvalidDataException("MC_CLIENT_MID not set in the environment"));
            string baseUri = Environment.GetEnvironmentVariable("MC_CLIENT_BASE_URI") ?? throw new InvalidDataException("MC_CLIENT_BASE_URI not set in the environment");

            this._authUri = new Uri($"https://{baseUri}.auth.marketingcloudapis.com");
            this._restUri = new Uri($"https://{baseUri}.rest.marketingcloudapis.com");
            _authHttpClient = new HttpClient()
            {
                BaseAddress = _authUri,
                Timeout = TimeSpan.FromMinutes(5)
            };
            _authHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _restHttpClient = new HttpClient()
            {
                BaseAddress = _restUri,
                Timeout = TimeSpan.FromMinutes(5)
            };
            _restHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task RefreshToken()
        {
            var content = new JsonObject
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = this._clientId,
                ["client_secret"] = this._clientSecret,
                ["account_id"] = _mid
            };

            var exponentialBackoff = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetry(5, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );

            HttpResponseMessage response = exponentialBackoff.Execute(() => GetAuthResponse(this._authHttpClient, new JsonContent(content)).Result);
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync() ??
                                    throw new InvalidDataException("Response successful but no payload");

            JsonNode responseBody = JsonNode.Parse(responseString) ??
                                    throw new InvalidDataException("Response successful but no payload");

            string? accessToken = responseBody["access_token"]?.ToString();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new InvalidDataException("No access token in payload");
            }

            _restHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            if (!int.TryParse(responseBody["expires_in"]?.ToString(), out int expirationSeconds))
            {
                throw new InvalidDataException("No expiration in payload");
            }

            _expiration = DateTime.Now.AddSeconds(expirationSeconds);
        }

        protected virtual async Task<HttpResponseMessage> GetAuthResponse(HttpClient client, StringContent content)
        {
            return await client.PostAsync("/v2/token", content);
        }

        public HttpResponseMessage SendWithRetryBackoff(HttpRequestMessage message)
        {
            var exponentialBackoff = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetry(5, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );

            return exponentialBackoff.Execute(() =>
            {
                return GetRestResponse(this._restHttpClient, message).Result;
            });
        }

        protected virtual async Task<HttpResponseMessage> GetRestResponse(HttpClient client, HttpRequestMessage request)
        {
            await RefreshTokenIfNecessary();

            return await client.SendAsync(request);
        }

        protected virtual async Task RefreshTokenIfNecessary()
        {
            if (DateTime.Now > _expiration)
            {
                await RefreshToken();
            }
        }
    }
}
