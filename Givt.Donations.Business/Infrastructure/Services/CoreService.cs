using Givt.Common.Models;
using Givt.Donations.Business.Infrastructure.Options;
using Serilog.Sinks.Http.Logger;
using System.Net.Http.Headers;

namespace Givt.Donations.Business.Infrastructure.Services
{
    public class CoreService
    {
        private readonly ILog _log;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly InfrastructureOptions _infrastructureOptions;

        public CoreService(
            ILog Log,
            IHttpClientFactory HttpClientFactory,
            InfrastructureOptions InfrastructureOptions)
        {
            _log = Log;
            _httpClientFactory = HttpClientFactory;
            _infrastructureOptions = InfrastructureOptions;
        }

        public async Task<CampaignModel> GetCampaignAsync(string mediumId, IList<string> languages, CancellationToken cancellationToken)
        {
            using var client = _httpClientFactory.CreateClient(nameof(CoreService));
            client.BaseAddress = new Uri(_infrastructureOptions.CoreServiceUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var requestPath = "/v2/campaigns/" + mediumId;
            AddAcceptLanguages(languages, client);
            var response = await client.GetAsync(requestPath, cancellationToken);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<CampaignModel>();

            _log.Error("Error returned from CoreService: {0} ({1})", new object[] { response.StatusCode, response.ReasonPhrase });
            return null;
        }

        private static void AddAcceptLanguages(IList<string> languages, HttpClient client)
        {
            if (languages?.Count > 0)
            {
                var quality = 1.0f;
                foreach (var language in languages)
                {
                    client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(language, quality));
                    quality -= 0.1f;
                }
            }
        }
    }
}
