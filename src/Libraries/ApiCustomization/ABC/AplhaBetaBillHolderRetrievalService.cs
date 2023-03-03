using ApiCustomization.ABC;
using ApiCustomization.Common;
using Application.Common.Sessions;
using Application.DTOs.PartnerContentDtos;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Entities.FormProcessing.ValueObjects;
using Domain.Exceptions;
using Infrastructure.Http;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace ApiCustomization;

public class AplhaBetaBillHolderRetrievalService : IApiContentRetrievalService
{
    public string PartnerContentProcessorKey => "ABC_CARD_HOLDER_API_INFO_PROCESSOR";

    private readonly IHttpService httpService;
    private readonly IPartnerService partnerService;
    private readonly IRepository<PartnerIntegrationDetails> partnerIntegrationRepo;
    private readonly IApiCustomizationUtil customizationUtil;

    public AplhaBetaBillHolderRetrievalService(IHttpService httpService,
        IPartnerService partnerService,
        IRepository<PartnerIntegrationDetails> partnerIntegrationRepo,
        IApiCustomizationUtil customizationUtil)
    {
        this.httpService = httpService;
        this.partnerService = partnerService;
        this.partnerIntegrationRepo = partnerIntegrationRepo;
        this.customizationUtil = customizationUtil;
    }

    public async Task<string> RetrieveContent<TRequest>(string waId, TRequest request)
    {
        var billHolerSummaryInfo = string.Empty;

        var partnerConfigDetail = await partnerIntegrationRepo
            .FirstOrDefault(x => x.PartnerContentProcessorKey.ToLower() == PartnerContentProcessorKey.ToLower());

        if (partnerConfigDetail is null)
            throw new BackgroundException($"No partner integration detail could be found base on this processor key {PartnerContentProcessorKey}");

        var config = GetBetaConfig(partnerConfigDetail);
        // make call to api cald holder infor


        return billHolerSummaryInfo;
    }




    // TODO: Create and Retrieve from a custom configSettings.json
    private AlphaBetaConfig GetBetaConfig(PartnerIntegrationDetails partnerConfig)
    {
        if (partnerConfig is null)
            throw new BackgroundException($"Partner details could not be found on the {PartnerContentProcessorKey} API-Partner-processor");

        AlphaBetaConfig alphaBetaConfig = new AlphaBetaConfig();

        alphaBetaConfig.BaseUrl = partnerConfig?.Configs?.FirstOrDefault(x => x.Key?.ToLower() == nameof(AlphaBetaConfig.BaseUrl).ToLower())?.Key;
        if(alphaBetaConfig.BaseUrl is null)
            throw new BackgroundException($"Partner details Base Url is empty on the {PartnerContentProcessorKey} API-Partner-processor");


        alphaBetaConfig.ClientKey = partnerConfig?.Configs?.FirstOrDefault(x => x.Key?.ToLower() == nameof(AlphaBetaConfig.ClientKey).ToLower())?.Key;
        if (alphaBetaConfig.ClientKey is null)
            throw new BackgroundException($"Partner details Base Url is empty on the {PartnerContentProcessorKey} API-Partner-processor");


        alphaBetaConfig.HolderVerificationEndpoint = partnerConfig?.Configs?.FirstOrDefault(x => x.Key?.ToLower() == nameof(AlphaBetaConfig.HolderVerificationEndpoint).ToLower())?.Key;
        if (alphaBetaConfig.HolderVerificationEndpoint is null)
            throw new BackgroundException($"Partner details Base Url is empty on the {PartnerContentProcessorKey} API-Partner-processor");

        return alphaBetaConfig;

    }

}

