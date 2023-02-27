using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.BusinessDtos;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Application.Services.Implementations
{
    public class BusinessSettingsService : IBusinessSettingsService
    {
        private readonly IRepository<BusinessMessageSettings> _businessMessageSettingsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BusinessSettingsService> _logger;
        private readonly ISystemSettingsService _systemSettingService;
        private readonly IBusinessService _businessService;
        private readonly IWebHookRegistrationHelper _webHookRegistrationHelper;
        private SystemSettingsConfig _configuration;

        public BusinessSettingsService(IRepository<BusinessMessageSettings> businessMessageSettingsRepository,
            ISystemSettingsService systemSettingService,
            IOptions<SystemSettingsConfig> options,
            IMapper mapper, ILogger<BusinessSettingsService> logger,
            IBusinessService businessService,
            IWebHookRegistrationHelper webHookRegistrationHelper)
        {
            _businessMessageSettingsRepository = businessMessageSettingsRepository;
            _systemSettingService = systemSettingService;
            _mapper = mapper;
            _logger = logger;
            _configuration = options.Value;
            _businessService = businessService;
            _webHookRegistrationHelper = webHookRegistrationHelper;
        }

        public async Task<SuccessResponse<BusinessMessageSettings>> ProcessCreateBusinessSetting(Guid id, CreateBusinessSetupDto dto)
        {
            if (dto == null || id == Guid.Empty) throw new RestException(HttpStatusCode.BadRequest, "one/more request parameter were not met");

            var business = await _businessService.GetBusinessByBusinessId(id);

            if(business is null) throw new RestException(HttpStatusCode.BadRequest, "No such business exist");

            var botName =
               await _businessMessageSettingsRepository
               .FirstOrDefault(x => x.BotName.Trim().ToLower() == dto.BotName);

            if (botName != null)
                throw new RestException(HttpStatusCode.BadRequest, "Sorry, Business botname already exist for another business.");

            var businessMessageSetting = 
                await _businessMessageSettingsRepository
                .FirstOrDefault(x=>x.BusinessId == id || x.BotName.Trim().ToLower() == dto.BotName);

            if(businessMessageSetting !=null) 
                throw new RestException(HttpStatusCode.BadRequest, "Business set-up already exist for this business.");


            var model = _mapper.Map<BusinessMessageSettings>(dto);
            model.BusinessId = id;

            // generate business webhook after business addition
            model.WebhookUrl = await GenerateBusinessWebHook(id);

            await _businessMessageSettingsRepository.AddAsync(model);
            await _businessMessageSettingsRepository.SaveChangesAsync();

            var businessSetting = await GetById(model.Id);

            return new SuccessResponse<BusinessMessageSettings>
            {
                Data = businessSetting.Data,
				Message = ResponseMessages.CreationSuccessResponse
            };
        }

        public async Task<SuccessResponse<BusinessMessageSettings>> GetByBusinessId(Guid businessGuid)
        {
            if (businessGuid == Guid.Empty) throw new RestException(HttpStatusCode.BadRequest, "No business identifier was provided");

            var businessMessageSetting = await _businessMessageSettingsRepository.FirstOrDefault(x => x.BusinessId == businessGuid);
            if (businessMessageSetting == null)
                return new SuccessResponse<BusinessMessageSettings>();

            return new SuccessResponse<BusinessMessageSettings>
            {
                Data = businessMessageSetting,
                Message = ResponseMessages.RetrievalSuccessResponse
            };
        }

        public async Task<SuccessResponse<BusinessMessageSettings>> GetById(Guid id)
        {
            if (id == Guid.Empty) throw new RestException(HttpStatusCode.BadRequest, "No business identifier was provided");

            var businessSettings = await _businessMessageSettingsRepository.FirstOrDefault(x => x.Id == id);

            if (businessSettings == null)
                throw new RestException(HttpStatusCode.NotFound, "Business message settings not found for this is");

            return new SuccessResponse<BusinessMessageSettings>
            {
                Data = businessSettings,
                Message = ResponseMessages.RetrievalSuccessResponse
            };
        }

        //admin usage...
        public async Task<PagedList<BusinessMessageSettings>> GetAllBusinessSetups(string search, int? skip = 0,
            int? take = int.MaxValue)
        {
            var getAll = await _businessMessageSettingsRepository.FindAsync(x=>
                (string.IsNullOrEmpty(search) || x.ApiKey.Trim().ToLower().Contains(search.ToLower()) 
                                              || x.WebhookUrl.Trim().ToLower().Contains(search.ToLower()) 
                                              || x.BusinessId.ToString().Trim().ToLower().Contains(search.ToLower())));


            return new PagedList<BusinessMessageSettings>(getAll.ToList(), 0, 0, 0);//will come back to insh this up
        }

        public async Task DeleteBusinessById(Guid id)
        {
            if (id != Guid.Empty)
            {
                var getBusinessId = await GetById(id);
                if (getBusinessId != null)
                {
                    _businessMessageSettingsRepository.Remove(getBusinessId.Data);
                }
            }
        }

        public async Task<SuccessResponse<bool>> Update(BusinessMessageSettings model)
        {
            if (model == null || model.Id == Guid.Empty) throw new RestException(HttpStatusCode.BadRequest, "An error occurred. Bad Request");

            var businessMesaageSetting = await _businessMessageSettingsRepository.FirstOrDefault(x=>x.Id == model.Id);

            if (businessMesaageSetting == null)
                throw new RestException(HttpStatusCode.NotFound, "No business settings found");

            businessMesaageSetting.ApiKey = !string.IsNullOrEmpty(model.ApiKey)? model.ApiKey:businessMesaageSetting.ApiKey;
            businessMesaageSetting.WebhookUrl = !string.IsNullOrEmpty(model.WebhookUrl) ? model.WebhookUrl: businessMesaageSetting.WebhookUrl;

            await _businessMessageSettingsRepository.SaveChangesAsync();

            return new SuccessResponse<bool>
            {
                Data = true,
                Message = ResponseMessages.Successful
            };
        }

        public async Task<bool> ProcessTestCounter(Guid id)
        {
            if (id == Guid.Empty) throw new RestException(HttpStatusCode.BadRequest, "An error occurred.");

            var businessMessageSetting = await _businessMessageSettingsRepository.FirstOrDefault(x=>x.Id == id);

            if (businessMessageSetting == null)
                throw new RestException(HttpStatusCode.NotFound, "No business settings found");

            businessMessageSetting.TestCounter += 1;

            await _businessMessageSettingsRepository.SaveChangesAsync();
            return true;
        }

       // Generate webhook for businesses.
       // TODO: Use app settings to retrieve base url.
       public async Task<string> GenerateBusinessWebHook(Guid businessId)
       {
           var baseWebhook = await _systemSettingService.GetSystemSettings();

           if (businessId == Guid.Empty)
               throw new RestException(HttpStatusCode.Conflict, "The business Id cannot be empty");

           if (baseWebhook is null)
               throw new RestException(HttpStatusCode.PreconditionFailed, "Base webhook system settings not configured");

           var businessWebHook = $"{baseWebhook.Data.BaseWebhook}/{businessId}/message";
           return businessWebHook;
       }
    }
}
