﻿using System.Net;
using ApiCustomization.ABC;
using ApiCustomization.ABC.Dtos;
using ApiCustomization.Common;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Infrastructure.Http;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using static System.Net.WebRequestMethods;

namespace ApiCustomization;

public class AplhaBetaBillHolderRetrievalService : IApiContentRetrievalService
{
    public string PartnerContentProcessorKey => EExternalPartnerContentProcessorKey
        .ABC_CARD_HOLDER_API_INFO.ToString();

    private readonly IHttpService httpService;
    private readonly IRepository<Partner> partnerrRepository;
    private readonly IRepository<PartnerIntegrationDetails> partnerIntegrationRepo;
    private readonly IApiCustomizationUtil customizationUtil;
    private readonly AlphaBetaConfig alphaBetaConfig;

    public AplhaBetaBillHolderRetrievalService(IHttpService httpService,
        IRepository<Partner> partnerrRepository,
        IRepository<PartnerIntegrationDetails> partnerIntegrationRepo,
        IApiCustomizationUtil customizationUtil,
        IOptions<AlphaBetaConfig> options)
    {
        this.httpService = httpService;
        this.partnerrRepository = partnerrRepository;
        this.partnerIntegrationRepo = partnerIntegrationRepo;
        this.customizationUtil = customizationUtil;
        this.alphaBetaConfig = options.Value;
    }

    public async Task<RetrieveContentResponse> RetrieveContent<TRequest>(Guid businessId, string waId, TRequest request)
    {
        string billPaymentCode = string.Empty;
        Exception exception = null;
        try {
            var billHolerSummaryInfo = string.Empty;
            var partnerConfigDetail = await partnerIntegrationRepo
                .FirstOrDefault(x => x.PartnerContentProcessorKey.ToLower() == PartnerContentProcessorKey.ToLower());

            var argumentKvpObj = partnerConfigDetail?.Parameters?.FirstOrDefault(x => x.Key?.ToLower() == alphaBetaConfig.LinkGeneratorUserParamKey?.ToLower());
            if (argumentKvpObj is null)
                throw new BackgroundException($"No value found in the Paramters property for this PartnerContentDeatil for key {PartnerContentProcessorKey}");

            billPaymentCode = await customizationUtil.GetArgumentValueFromSession(argumentKvpObj, waId, PartnerContentProcessorKey);

            if (partnerConfigDetail is null)
                throw new BackgroundException($"No partner integration detail could be found base on this processor key {PartnerContentProcessorKey}");
           
        }
        catch(Exception e) {
            exception = e;
        }

        return alphaBetaConfig.IsMockRequest ? DemoBillPaymentUserInfo() :
               await MakeApiCallToAbc(billPaymentCode: billPaymentCode, phone: waId, exception);
    }

    private RetrieveContentResponse DemoBillPaymentUserInfo()
    {
        return new RetrieveContentResponse
        {
            IsSuccessful = false,
            Response = "Record found!\n Here is the Transaction Summary\n  Tax Payer: Chris Yakubu\n Property PIN: N-1234587\n Bill No.: 1234567890\n Amount: NGN24,250.00"
        };

    }

    private async Task<RetrieveContentResponse> MakeApiCallToAbc(string billPaymentCode, string phone, Exception exception = null) {

        var url = $"{alphaBetaConfig.BaseUrl}/{alphaBetaConfig.HolderVerificationEndpoint}/{billPaymentCode}";

        HttpMessageResponse<CustomizationSuccessResponse<BillReferenceResponse>> httpResult = null;
        var message = "";
        var success = true; 
        ESessionState? updatedSession = null;
        IDictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("phone", phone);

        try
        {
            if (exception is not null)
                throw exception;

            if (billPaymentCode.Length != 10)
                throw new BadRequestException("Invalid bill-code, please provide a 10 digit bill-code", (int)HttpStatusCode.Forbidden);

            httpResult = await httpService.Get<CustomizationSuccessResponse<BillReferenceResponse>>
                (url: url, parameters: parameters, header: null);

            if (httpResult is null || httpResult.Data is null)
                throw new BadRequestException("Response is empty, record not found", (int)HttpStatusCode.BadRequest);

            message = ProcessResponse(httpResult.Data);
        }
        catch (BadRequestException ex)
        {
            if (ex.StatusCode == 400 || ex.StatusCode == 404) {
                 message = $"User info not found, kindly verify your bank payment code and try again." +
                    $"{Environment.NewLine}{Environment.NewLine}" +
                    " You can type 'end' to restart the session";
            }
            if (ex.StatusCode == 403)
            {
                message = $"Unrecognized payment code, kindly ensure that your bank payment code is *10 digits* and try again." +
                   $"{Environment.NewLine}{Environment.NewLine}" +
                   " You can type 'end' to restart the session";
            }

            success = false;
        }
        catch (InternalServerException e)
        {
            message = $"User info retreival issue! {Environment.NewLine}" +
                $"User info could not be verified." +
                $"{Environment.NewLine}Contact admin for assistance.";

            success = false;
            updatedSession = ESessionState.PLAINCONVERSATION;

            exception = e;
        }

        return new RetrieveContentResponse
        {
            IsSuccessful = success,
            Response = message,
            UpdatedSessionState = updatedSession,
            DisContinueProcess = !success
        };
    }

    private string ProcessResponse(CustomizationSuccessResponse<BillReferenceResponse> response) {

        if(response is null || response?.Data is null)
            throw new BadRequestException("Empty result, Bill Payer info not found", 400);

        var model = response.Data;

        if (model is null || string.IsNullOrEmpty(model.payerName) || string.IsNullOrEmpty(model.pid))
            throw new BadRequestException("Empty result, Bill Payer info not found", 400);

        return
            $"*Record found!* \n \n" +
            $"Here is the transaction summary \n" +
            $"Tax Payer: {model.payerName} \n" +
            $"Property PIN: {model.pid} \n" +
            $"Bill No: {model.oraAgencyRev} \n" +
            $"Amount: {model.amountDue} \n";
    }
}

