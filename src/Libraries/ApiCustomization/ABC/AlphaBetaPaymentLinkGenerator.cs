using System;
using ApiCustomization.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace ApiCustomization.ABC
{
	public class AlphaBetaPaymentLinkGenerator: IApiContentRetrievalService
    {
        private readonly IApiCustomizationUtil customizationUtil;
        private readonly IRepository<PartnerIntegrationDetails> partnerIntegrationRepo;
        private readonly AlphaBetaConfig alphaBetaConfig;

        public AlphaBetaPaymentLinkGenerator(IApiCustomizationUtil customizationUtil,
            IRepository<PartnerIntegrationDetails> partnerIntegrationRepo,
            IOptions<AlphaBetaConfig> options)
		{
            this.customizationUtil = customizationUtil;
            this.partnerIntegrationRepo = partnerIntegrationRepo;
            alphaBetaConfig = options.Value;
		}

        public string PartnerContentProcessorKey => EExternalPartnerContentProcessorKey
            .ALPHA_BETA_PAYMENT_LINK_GENERATOR.ToString();

        /// <summary>
        /// In this implementation the Tquest is string being the billcode of the user
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="waId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> RetrieveContent<TRequest>(string waId, TRequest request)
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

            var endPointDetails = $"?billPaymentCode={billCode}&phoneNumber={waId}";
            return $"{alphaBetaConfig.BillCodePaymentPageLink}{endPointDetails}";
        }
    }
}

