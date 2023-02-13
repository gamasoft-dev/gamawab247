using BillProcessorAPI.Dtos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RestSharp;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace BillProcessorAPI.Helpers.Revpay
{
    public class ApiClient
    {
        private const int MaxRetries = 3;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IOptions<RevpayOptions> _options;
        private readonly AsyncRetryPolicy _retryPolicy;
        public ApiClient(IOptions<RevpayOptions> options, IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _options = options;
            _retryPolicy = Policy.Handle<HttpRequestException>().RetryAsync(MaxRetries);
           
        }

        public async Task<HttpResponseMessage> PostAsync(string baseUrl, string relativeUrl, object values)
        {
			//create Client
			var client = _clientFactory.CreateClient(baseUrl);

			//Get appsettings values
			var state = _options.Value.State;
			var key = _options.Value.Key;
			var clientId = _options.Value.ClientId;
			var webGuid = _options.Value.ClientId;
			
            return await _retryPolicy.ExecuteAsync(async () =>
            {
				string jsonSerialization = JsonConvert.SerializeObject(values);
				StringContent content = new(jsonSerialization, Encoding.UTF8, "application/json");
				var request = new HttpRequestMessage(HttpMethod.Post, relativeUrl);
				request.Content = content;

				request.Headers.Add("webguid", webGuid);
				request.Headers.Add("state", state);
				request.Headers.Add("hash", RevpayConfig.GenerateSHA512Hash(key, webGuid, state));
				request.Headers.Add("clientId", clientId);

				var response = await client.SendAsync(request);
				return response;
			});

        }
       
    }
}
