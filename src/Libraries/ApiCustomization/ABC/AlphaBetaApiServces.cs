using ApiCustomization.ABC;
using ApiCustomization.Common;
using Microsoft.Extensions.Options;

namespace ApiCustomization;

public class AlphaBetaApiServces : IApiContentRetrievalService
{
    public string ApiProcessIndentifier => throw new NotImplementedException();

    private readonly IHttpClientFactory httpClientFactory;
    private readonly AlphaBetaConfig alphaBetaConfig;

    public AlphaBetaApiServces(IHttpClientFactory httpClientFactory,
        IOptions<AlphaBetaConfig> alphaBetaConfig)
    {
        this.httpClientFactory = httpClientFactory;
        this.alphaBetaConfig = alphaBetaConfig.Value;
    }

    public Task<string> RetrieveContent<TRequest>(TRequest request)
    {
        throw new NotImplementedException();
    }
}

