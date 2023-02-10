using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

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

        public static async Task<HttpResponseMessage> PostAsJson<T>(this HttpClient httpClient, string url, T? data, IConfiguration config)
        {
            httpClient.DefaultRequestHeaders.Clear();
            var dataAsString = JsonSerializer.Serialize(data);
            var content = new StringContent(dataAsString, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            httpClient.DefaultRequestHeaders.Add(Authorization, $"Bearer {config["SecretKey"]}");

            return await httpClient.PostAsync(url, content);
        }

        public static Task<HttpResponseMessage> GetAsJson(this HttpClient httpClient, string url, IConfiguration config)
        {
            httpClient.DefaultRequestHeaders.Add(Authorization, $"Bearer {config["SecretKey"]}");

            return httpClient.GetAsync(url);
        }
    }
}
