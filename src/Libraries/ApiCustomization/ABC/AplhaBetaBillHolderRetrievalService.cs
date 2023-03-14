using System.Net;
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

    public async Task<string> RetrieveContent<TRequest>(string waId, TRequest request)
    {
        var billHolerSummaryInfo = string.Empty;
        var partnerConfigDetail = await partnerIntegrationRepo
            .FirstOrDefault(x => x.PartnerContentProcessorKey.ToLower() == PartnerContentProcessorKey.ToLower());

        var argumentKvpObj = partnerConfigDetail?.Parameters?.FirstOrDefault(x => x.Key?.ToLower() == alphaBetaConfig.BillCodePaymentPageLink?.ToLower());
        if (argumentKvpObj is null)
            throw new BackgroundException($"No value found in the Paramters property for this PaartnerContentDeatil for key {PartnerContentProcessorKey}");

        var billPaymentCode = await customizationUtil.GetArgumentValueFromSession(argumentKvpObj, waId, PartnerContentProcessorKey);

        if (partnerConfigDetail is null)
            throw new BackgroundException($"No partner integration detail could be found base on this processor key {PartnerContentProcessorKey}");
        // make call to api cald holder information

        return alphaBetaConfig.IsMockRequest ? DemoBillPaymentUserInfo() :
            await MakeApiCallToAbc(billPaymentCode: billPaymentCode);
    }

    private static string DemoBillPaymentUserInfo()
    {
        return "Record found!\n Here is the Transaction Summary\n  Tax Payer: Chris Yakubu\n Property PIN: N-1234587\n Bill No.: 1234567890\n Amount: NGN24,250.00";

    }

    private async Task<string> MakeApiCallToAbc(string billPaymentCode) {

        var url = $"{alphaBetaConfig.BaseUrl}/{alphaBetaConfig.HolderVerificationEndpoint}";

        IDictionary<string, object> parameter = new Dictionary<string, object>();
        parameter.Add("billPaymentCode", billPaymentCode);
        HttpMessageResponse<BillReferenceResponse> httpResult = null;

        try
        {
            httpResult = await httpService.Get<BillReferenceResponse>(url: url, parameters: parameter, header: null);

            if (httpResult is null || httpResult.Data is null)
                throw new BadRequestException("Response is empty, record not found", (int)HttpStatusCode.BadRequest);
        }
        catch (BadRequestException ex)
        {
            if (ex.StatusCode == 400 || ex.StatusCode == 404) {
                return $"User info not found, kindly verify your billPaymentCode and try again," +
                    $"{Environment.NewLine}" +
                    " You can type 'end' to restart a session/process";
            }
        }
        catch (InternalServerException e)
        {
            return $"User info could not be verified, kindly verify your billPaymentCode and try again," +
                $"{Environment.NewLine}" +
                " Also you can contact admin for assistance, You can type 'end' to restart a session";
            
        }

        return ProcessResponse(httpResult.Data);
    }

    private string ProcessResponse(BillReferenceResponse model) {
        return $"Tax Payer: {model.PayerName} \n" +
            $"Property PIN: {model.Pid} \n" +
            $"Bill No: {model.OraAgencyRev} \n" +
            $"Amount: {model.AmountDue} \n";
    }
}

