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
    private readonly AlphaBetaConfig alphaBetaConfig;

    public AplhaBetaBillHolderRetrievalService(IHttpService httpService,
        IPartnerService partnerService,
        IRepository<PartnerIntegrationDetails> partnerIntegrationRepo,
        IApiCustomizationUtil customizationUtil, IOptions<AlphaBetaConfig> options)
    {
        this.httpService = httpService;
        this.partnerService = partnerService;
        this.partnerIntegrationRepo = partnerIntegrationRepo;
        this.customizationUtil = customizationUtil;
        this.alphaBetaConfig = options.Value;
    }

    public async Task<string> RetrieveContent<TRequest>(string waId, TRequest request)
    {
        var billHolerSummaryInfo = string.Empty;

        var partnerConfigDetail = await partnerIntegrationRepo
            .FirstOrDefault(x => x.PartnerContentProcessorKey.ToLower() == PartnerContentProcessorKey.ToLower());

        if (partnerConfigDetail is null)
            throw new BackgroundException($"No partner integration detail could be found base on this processor key {PartnerContentProcessorKey}");

        // make call to api cald holder infor


        return billHolerSummaryInfo;
    }
}

