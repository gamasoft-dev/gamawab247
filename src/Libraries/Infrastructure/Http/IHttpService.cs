using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Common;

namespace Application.Services.Interfaces
{
    public interface IHttpService
    {
        Task<HttpMessageResponse<TResponse>> Post<TResponse, TRequest>(string fullUrl, RequestHeader header, TRequest request);
        Task<HttpMessageResponse<TResponse>> Get<TResponse>(string url, RequestHeader  header, IDictionary<string, object> parameters = null);
    }
}