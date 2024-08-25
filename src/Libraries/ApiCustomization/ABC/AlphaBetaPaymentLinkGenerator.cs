using System;
using ApiCustomization.Common;
using Domain.Common.ShortLink;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.ShortLink;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApiCustomization.ABC
{
	public class AlphaBetaPaymentLinkGenerator: IApiContentRetrievalService
    {
        private readonly IApiCustomizationUtil customizationUtil;
        private readonly IRepository<PartnerIntegrationDetails> partnerIntegrationRepo;
        private readonly AlphaBetaConfig alphaBetaConfig;
        private readonly ICutlyService _cutlyService;
        private readonly ILogger<AlphaBetaPaymentLinkGenerator> _logger;

        public AlphaBetaPaymentLinkGenerator(IApiCustomizationUtil customizationUtil,
            IRepository<PartnerIntegrationDetails> partnerIntegrationRepo,
            IOptions<AlphaBetaConfig> options,
            ICutlyService cutlyService,
            ILogger<AlphaBetaPaymentLinkGenerator> logger)
        {
            this.customizationUtil = customizationUtil;
            this.partnerIntegrationRepo = partnerIntegrationRepo;
            alphaBetaConfig = options.Value;
            _cutlyService = cutlyService;
            _logger = logger;
        }

        public string PartnerContentProcessorKey => EExternalPartnerContentProcessorKey
            .ALPHA_BETA_PAYMENT_LINK_GENERATOR.ToString();

        /// <summary>
        /// In this implementation the TRequest is string being the billcode of the user
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="waId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<RetrieveContentResponse> RetrieveContent<TRequest>(Guid businessId, string waId, TRequest request)
        {
            var partnerConfigDetail = await partnerIntegrationRepo
                .FirstOrDefault(x => x.PartnerContentProcessorKey.ToLower() == PartnerContentProcessorKey.ToLower());

            if (partnerConfigDetail is null)
                throw new BackgroundException($"No partner integration detail could be found base on this processor key {PartnerContentProcessorKey}");

            var argumentKvpObj = partnerConfigDetail?.Parameters?.FirstOrDefault(x => x.Key?.ToLower() == alphaBetaConfig.LinkGeneratorUserParamKey?.ToLower());
            if (argumentKvpObj is null)
                throw new BackgroundException($"No value found in the Paramters property for this PaartnerContentDeatil for key {PartnerContentProcessorKey}");

            var billCode = await customizationUtil.GetArgumentValueFromSession(argumentKvpObj, waId, PartnerContentProcessorKey);

            if (string.IsNullOrEmpty(billCode))
                throw new BackgroundException($"User {waId} bill code not found");

            var endPointParams = $"?billCode={billCode}&phoneNumber={waId}";
            var paymentLink = $"{alphaBetaConfig.BillCodePaymentPageLink}{endPointParams}";
            var shortLink = await _cutlyService.ShortLink(paymentLink);
            _logger.LogInformation($"system generated payment link: {paymentLink}");


            return new RetrieveContentResponse
            {
                IsSuccessful = true,
                Response = shortLink,
            };
        }
    }
}

