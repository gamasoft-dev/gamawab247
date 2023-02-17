﻿using Application.Services.Interfaces;
using Domain.Common;
using Domain.Exceptions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        private readonly IHttpClientFactory httpClientFactory;

        public HttpService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Generic post request using restsharp
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="fullUrl"></param>
        /// <param name="header"></param>
        /// <param name="request"></param>
        /// <exception cref="HttpException">Throws all deriavations of http exceptions</exception>
        /// <returns></returns>
        public async Task<HttpMessageResponse<TResponse>> Post<TResponse, TRequest>(string fullUrl, RequestHeader header, TRequest request)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                AddHeaderParams(client, header);

                string jsonObject = JsonConvert.SerializeObject(request);
                var ct = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(fullUrl, ct);

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
                else
                {
                    if ((int)response.StatusCode >= 401 && (int)response.StatusCode < 404)
                        throw new AuthenticationException("Api Authentication error", (int)response.StatusCode);

                    else if ((int)response.StatusCode >= 500)
                        throw new InternalServerException("An error occurred from the api", (int)response.StatusCode);

                    else
                        throw new BadRequestException(await response.Content.ReadAsStringAsync());
                }

            }
            catch (System.Exception ex)
            {
                throw new HttpException(ex.Message, ex);
            }
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
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // add header paramters
            AddHeaderParams(client, header);

            var response = await client.GetAsync(url);
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
            else
            {
                if ((int)response.StatusCode >= 401 && (int)response.StatusCode < 404)
                    throw new AuthenticationException("Api Authentication error", (int)response.StatusCode);

                else if ((int)response.StatusCode >= 500)
                    throw new InternalServerException("An error occurred from the api", (int)response.StatusCode);

                else
                    throw new BadRequestException(await response.Content.ReadAsStringAsync());
            }
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

        
        private string SerialJsonToString<T>(T objectBody) where T : class
        {
            return JsonConvert.SerializeObject(objectBody);
        }
    }
}