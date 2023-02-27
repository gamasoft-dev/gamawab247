using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Http;
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
            return await _businessSetup.Query(x => x.IsWebhookConfigured == false).ToListAsync();    
        }

        public async Task ProcessGamaSoftBusinessConfigurationTo360()
        {
            var businessMessageSettings = await GetAllUnConfiguredBusinesses();
            foreach (var businessMessageSetting in businessMessageSettings)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(businessMessageSetting.ApiKey))
                    {
                        var response = await HttpProcessTo360Dialog(businessMessageSetting);
                        if (response.Data is not null)
                        {
                            //update the config to true.
                            UpdateBusinessSetupConfigStatus(businessMessageSetting);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    continue;
                }
            }
        }

        async Task<HttpMessageResponse<DialogWebhookConfigDto>> HttpProcessTo360Dialog(BusinessMessageSettings settings)
        {
            if (_dialog360Settings is null || string.IsNullOrEmpty(_dialog360Settings.AuthorizationName) ||
                string.IsNullOrEmpty(_dialog360Settings.BaseUrl))
                throw new BgBusinessConfigException("The enviroment variable for 360Dialog could not be retrieved or incorrectly configured." +
                    " Make sure it has baseUrl, AuthorizationName keys and values");

            const string endpoint = "v1/configs/webhook";
            var url = $"{_dialog360Settings.BaseUrl}/{endpoint}";

            IDictionary<string, string> dictNew = new Dictionary<string, string>();
            dictNew.Add(_dialog360Settings.AuthorizationName, settings.ApiKey);

            var header = new RequestHeader(dictNew);

            var configDto = new DialogWebhookConfigDto
            {
                Url = settings.WebhookUrl
            };
            HttpMessageResponse<DialogWebhookConfigDto> httpResult = null;
            try
            {
               httpResult  = await _httpService.Post<DialogWebhookConfigDto, DialogWebhookConfigDto>
                    (fullUrl: url, header: header, request: configDto);
            }
            catch (Exception ex)
            {
                throw new BgBusinessConfigException("An error occurred whilst making post call to 360Dialog to register webhook", ex);
            }

            return httpResult;
        }
    }
}
