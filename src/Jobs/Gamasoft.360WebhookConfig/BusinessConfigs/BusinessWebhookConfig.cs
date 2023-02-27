using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Http;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;

namespace Gamasoft.Worker.Cron.BusinessConfigs
{
    public class BusinessWebhookConfig : IBusinessWebhookConfig
    {
        private readonly IRepository<Business> _businessRepo;
        private readonly IRepository<BusinessMessageSettings> _businessSetup;
        private readonly SystemSettingsConfig _systemSettings;
        private readonly IHttpService _httpService;
        private readonly Dialog360Settings _dialog360Settings;

        public BusinessWebhookConfig(IRepository<Business> businessRepo,
        IOptions<SystemSettingsConfig> systemSettings, IRepository<BusinessMessageSettings> businessConfigRepository,
        IHttpService httpService, IOptions<Dialog360Settings> options)
        {
            _businessRepo = businessRepo;
            _businessSetup = businessConfigRepository;
            _systemSettings = systemSettings.Value;
            _httpService = httpService;
            _dialog360Settings = options.Value;
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
                    var response = await HttpProcessTo360Dialog(item);
                    if (response.Data is not null)
                    {
                        //update the config to true.
                        UpdateBusinessSetupConfigStatus(item);
                    }
                }
            }
        }

        async Task<HttpMessageResponse<DialogWebhookConfigDto>> HttpProcessTo360Dialog(BusinessMessageSettings settings)
        {
            if (_dialog360Settings is null)
                throw new RestException(System.Net.HttpStatusCode.InternalServerError,
                    "The enviroment variable for 360Dialog could not be retrieved from settings");

            const string endpoint = "v1/configs/webhook";
            var url = $"{_dialog360Settings.BaseUrl}/{endpoint}";

            IDictionary<string, string> dictNew = new Dictionary<string, string>();

            if (_dialog360Settings is null || string.IsNullOrEmpty(_dialog360Settings?.AuthorizationName))
                throw new RestException(HttpStatusCode.InternalServerError, "Dialog Settings config not set");

            dictNew.Add(key: _dialog360Settings.AuthorizationName, settings.ApiKey);

            var header = new RequestHeader(dictNew);

            var configDto = new DialogWebhookConfigDto
            {
                Url = settings?.WebhookUrl
            };

            var httpResult = await _httpService.Post<DialogWebhookConfigDto, DialogWebhookConfigDto>
                (url, header: header, request: configDto);

            return httpResult;
        }

        async void UpdateBusinessSetupConfigStatus(BusinessMessageSettings businessMessageSettings)
        {
            if (businessMessageSettings is null)
                throw new RestException(HttpStatusCode.BadGateway, ResponseMessages.PayLoadCannotBeNull);

            businessMessageSettings.IsWebhookConfigured = true;
            businessMessageSettings.UpdatedAt = DateTime.UtcNow;
            await _businessSetup.SaveChangesAsync();
        }
    }
}
