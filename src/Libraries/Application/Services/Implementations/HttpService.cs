using Application.DTOs;
using Application.Services.Interfaces;
using Domain.Common;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
	public class HttpService: IHttpService
    {
        private readonly HttpClient _restClient;
        private readonly Dialog360Settings _appSettings;
        
        public HttpService(IOptions<Dialog360Settings> appSettings)
        {
            _restClient = new HttpClient();
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Generic post request using restsharp
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="url"></param>
        /// <param name="header"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<HttpMessageResponse<TResponse>> Post<TResponse, TRequest>(string url, RequestHeader header, TRequest request)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            AddHeaderParams(client, header);

            string jsonObject = JsonConvert.SerializeObject(request);
            var ct = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, ct);

            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<TResponse>(responseString);
                return new HttpMessageResponse<TResponse>
                {
                    Data = result,
                    Message = ResponseMessages.Successful,
                    Status = (int)response.StatusCode
                };
            }
            return new HttpMessageResponse<TResponse>
            {
                Data = default,
                Message = $"{ResponseMessages.Failed} : {responseString}",
                Status = (int)response.StatusCode
            };
        }

        /// <summary>
        /// Get request implementation using restsharp
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="url"></param>
        /// <param name="header"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<HttpMessageResponse<TResponse>> Get<TResponse>(string url, RequestHeader  header, IDictionary<string, object> parameters = null)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //RestRequest restRequest = new RestRequest(url, Method.Get);

            // add header paramters
            AddHeaderParams(client, header);

            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TResponse>(responseString);

            if (response.IsSuccessStatusCode)
            {
                return new HttpMessageResponse<TResponse>
                {
                    Data = result,
                    Message = ResponseMessages.Successful,
                    Status = (int)response.StatusCode
                };
            }
            return new HttpMessageResponse<TResponse>
            {
                Data = default,
                Message = $"{ResponseMessages.Failed} : {responseString}",
                Status = (int)response.StatusCode
            };
        }

        /// <summary>
        /// Add the request Headers
        /// </summary>
        /// <param name="request"></param>
        /// <param name="requestHeader"></param>
        private void AddHeaderParams(HttpClient request, RequestHeader requestHeader)
        {
            if (requestHeader is not null)
            {
                requestHeader.Headers.Distinct();
                foreach (var c in requestHeader.Headers)
                {
                    request.DefaultRequestHeaders.TryAddWithoutValidation(c.Key, c.Value);
                };
            }
        }

        /// <summary>
        /// Add request parameters
        /// </summary>
        /// <param name="request"></param>
        /// <param name="parameters"></param>
        private void AddRequestParameter(RestRequest request, Dictionary<string, object> parameters)
        {
            foreach (var kvp in parameters)
            {
                var parameter = new Param(kvp.Key, kvp.Value, ParameterType.QueryString, true);
                request.AddParameter(parameter);
            }   
        }

        private string SerialJsonToString<T>(T objectBody) where T : class
        {
            return JsonConvert.SerializeObject(objectBody);
        }
    }

    public record Param : Parameter
    {
        public Param(string name, object value, ParameterType type, bool encode)
            : base(name, value, type, encode)
        {
            
        }
    }
}