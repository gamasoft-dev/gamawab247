using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Application.Cron
{
    public class BusinessWebhookConfigRegistrationCron : IBusinessWebhookConfigRegistrationCron
    {
        private readonly IRepository<Business> _businessRepo;
        private readonly IRepository<BusinessMessageSettings> _businessSetup;
        private readonly SystemSettingsConfig _systemSettings;
        private readonly IHttpService _httpService;
        private readonly Dialog360Settings _dialog360Settings;

        public BusinessWebhookConfigRegistrationCron(IRepository<Business> businessRepo,
        IOptions<SystemSettingsConfig> systemSettings, IRepository<BusinessMessageSettings> businessConfigRepository,
        IHttpService httpService, IOptions<Dialog360Settings> options)
        {
            _businessRepo = businessRepo;
            _businessSetup = businessConfigRepository;
            _systemSettings = systemSettings.Value;
            _httpService = httpService;
            _dialog360Settings = options.Value;
        }

        async void UpdateBusinessSetupConfigStatus(BusinessMessageSettings businessMessageSettings)
        {
            if (businessMessageSettings is null)
                throw new RestException(HttpStatusCode.BadGateway, ResponseMessages.PayLoadCannotBeNull);

            businessMessageSettings.IsWebhookConfigured = true;
            businessMessageSettings.UpdatedAt = DateTime.UtcNow;
            await _businessSetup.SaveChangesAsync();
        }

        async Task<List<BusinessMessageSettings>> GetAllUnConfiguredBusinesses()
        {
            var businessSetups = await _businessSetup.Query(x => x.IsWebhookConfigured == false).ToListAsync();
            return businessSetups;
        }

        public async Task ProcessGamaSoftBusinessConfigurationTo360()
        {
            var getAll = await GetAllUnConfiguredBusinesses();
            foreach (var item in getAll)
            {
                if (!string.IsNullOrWhiteSpace(item.ApiKey))
                {
                    var response = await HttpProcessTo360Dialog(item.BusinessId, item.ApiKey);
                    if (response.Data is not null)
                    {
                        //update the config to true.
                        item.WebhookUrl = response?.Data?.Url;
                        UpdateBusinessSetupConfigStatus(item);
                    }
                }
            }
        }

        async Task<HttpMessageResponse<DialogWebhookConfigDto>> HttpProcessTo360Dialog(Guid businessId, string apiKey)
        {
            if (_dialog360Settings is null)
                throw new RestException(System.Net.HttpStatusCode.InternalServerError,
                    "The enviroment variable for 360Dialog could not be retrieved from settings");

            const string endpoint = "v1/configs/webhook";
            var url = $"{_dialog360Settings.BaseUrl}/{endpoint}";

            IDictionary<string, string> dictNew = new Dictionary<string, string>();
            dictNew.Add(_dialog360Settings?.AuthorizationName, apiKey);

            var header = new RequestHeader(dictNew);

            var configDto = new DialogWebhookConfigDto
            {
                Url = $"{_systemSettings.AppBaseUrl}/api/v1/business/{businessId}/message"
            };

            var httpResult = await _httpService.Post<DialogWebhookConfigDto, DialogWebhookConfigDto>
                (fullUrl: url, header: header, request: configDto);

            return httpResult;
        }
    }
}
