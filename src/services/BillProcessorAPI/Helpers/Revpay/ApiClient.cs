using BillProcessorAPI.Dtos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace BillProcessorAPI.Helpers.Revpay
{
    public class ApiClient
    {
        private readonly HttpClient _client;
        private readonly RestClient _apiClient;
        private readonly IOptions<RevpayOptions> _options;
        public ApiClient(HttpClient client, IOptions<RevpayOptions> options)
        {
            _client = client;
            _options = options;
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _apiClient = new RestClient(_options.Value.BaseUrl);
        }

        public async Task<HttpResponseMessage> PostAsync(string baseUrl, string relativeUrl, object values)
        {
            _client.BaseAddress = new Uri(baseUrl);

            //Get appsettings values
            var state = _options.Value.State;
            var key = _options.Value.Key;
            var clientId = _options.Value.ClientId;
            var webGuid = _options.Value.ClientId;

            var jsonSerialization = JsonConvert.SerializeObject(values);
            var content = new StringContent(jsonSerialization, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, relativeUrl);
            request.Content = content;

            request.Headers.Add("webguid", webGuid);
            request.Headers.Add("state", state);
            request.Headers.Add("hash", RevpayConfig.GenerateSHA512Hash(key, webGuid, state));
            request.Headers.Add("clientId", clientId);


            var response = await _client.SendAsync(request);
            return response;
        }


        //Post Implementation with restsharp
        public async Task<RestResponse> Post(string relativeURl, object model)
        {
            //Get appsettings value
            var state = _options.Value.State;
            var key = _options.Value.Key;
            var clientId = _options.Value.ClientId;
            var webGuid = _options.Value.ClientId;

            var request = new RestRequest(relativeURl, Method.Post);
            request.AddHeader("webguid", webGuid);
            request.AddHeader("state", state);
            request.AddHeader("clientId", clientId);
            request.AddHeader("hash", RevpayConfig.GenerateSHA512Hash(key, webGuid, state));
            request.AddJsonBody(model);
            request.Method = Method.Post;

            RestResponse<BillPayerInfoDto> response = await _apiClient.ExecuteAsync<BillPayerInfoDto>(request);
            return response;
        }
    }
}
