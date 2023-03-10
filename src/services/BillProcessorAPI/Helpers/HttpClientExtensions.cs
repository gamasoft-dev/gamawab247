using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Helpers.Revpay;
using Microsoft.Extensions.Options;

namespace BillProcessorAPI.Helpers
{
    public static class HttpClientExtensions
    {
        private const string Authorization = "Authorization";
       
        public static async Task<T> ReadContentAs<T>(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");

            var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<T>(dataAsString);
        }

        public static async Task<HttpResponseMessage> PostAsJson(
            this HttpClient httpClient, 
            string Baseurl, string relativeUrl,
            AbcRequestPayload data)
        {
   
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.BaseAddress = new Uri(Baseurl);
            var dataAsString = JsonSerializer.Serialize(data);
            var content = new StringContent(dataAsString, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, relativeUrl);
            request.Content = content;
            
            return await httpClient.SendAsync(request);
        }

        public static Task<HttpResponseMessage> GetAsJson(this HttpClient httpClient, string url, IConfiguration config)
        {
            httpClient.DefaultRequestHeaders.Add(Authorization, $"Bearer {config["SecretKey"]}");

            return httpClient.GetAsync(url);
        }
    }
}
