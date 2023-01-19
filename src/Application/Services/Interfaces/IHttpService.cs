using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Helpers;

namespace Application.Services.Interfaces
{
    public interface IHttpService : IAutoDependencyService
    {
        Task<HttpMessageResponse<TResponse>> Post<TResponse, TRequest>(string url, RequestHeader header,
            TRequest request);
        Task<HttpMessageResponse<TResponse>> Get<TResponse>(string url, RequestHeader  header, IDictionary<string, object> parameters = null);
    }
}