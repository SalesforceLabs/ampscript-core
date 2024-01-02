using System.Text.Json;
using Sage.Engine.Tests.Compatibility;

namespace MarketingCloudIntegration.Render
{
    internal class RenderService : IRenderService
    {
        private readonly IMarketingCloudRestClient _restClient;

        public RenderService(IMarketingCloudRestClient restClient)
        {
            _restClient = restClient;
        }

        public async Task<RenderResponse> Render(string channel, RenderRequest request)
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, $"/messaging/v1/{channel}/render");

            message.Content = new JsonContent(JsonSerializer.Serialize(request));

            var response = _restClient.SendWithRetryBackoff(message);
            string responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new BadScriptException(request.content ?? string.Empty, responseString);
                }
            }

            response.EnsureSuccessStatusCode();

            return JsonSerializer.Deserialize<RenderResponse>(responseString) ?? throw new InvalidDataException("Invalid response from API");
        }
    }
}
