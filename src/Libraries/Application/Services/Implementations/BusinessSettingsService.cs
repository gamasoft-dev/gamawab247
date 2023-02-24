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
        private readonly Dialog360Settings _configuration;

        public BusinessSettingsService(IRepository<BusinessMessageSettings> businessMessageSettingsRepository,
            ISystemSettingsService systemSettingService,
            IOptions<Dialog360Settings> options,
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

            var getBusinessById = await _businessService.GetBusinessByBusinessId(id);

            if(getBusinessById is null) throw new RestException(HttpStatusCode.BadRequest, "No such business exist");

            var getExistingBotnameOnBizSetup =
               await _businessMessageSettingsRepository
               .FirstOrDefault(x => x.BotName.Trim().ToLower() == dto.BotName);

            if (getExistingBotnameOnBizSetup != null)
                throw new RestException(HttpStatusCode.BadRequest, "Sorry, Business botname already exist for another business.");

            var getExistingBusinessOnBizSetup = 
                await _businessMessageSettingsRepository
                .FirstOrDefault(x=>x.BusinessId == id || x.BotName.Trim().ToLower() == dto.BotName);

            if(getExistingBusinessOnBizSetup !=null) 
                throw new RestException(HttpStatusCode.BadRequest, "Business set-up already exist for this business.");


            var model = _mapper.Map<BusinessMessageSettings>(dto);
            model.BusinessId = id;

            // generate business webhook after business addition
            model.WebhookUrl = _configuration.BaseUrl;
            model.ApiKey = _configuration.AuthorizationName;

            await _businessMessageSettingsRepository.AddAsync(model);
            await _businessMessageSettingsRepository.SaveChangesAsync();

            await _webHookRegistrationHelper.RegisterBusinessWebHookUrl(model.WebhookUrl,model.ApiKey);

            var businessSetting = await GetById(model.Id);
            var response = _mapper.Map<BusinessMessageSettings>(businessSetting);

            return new SuccessResponse<BusinessMessageSettings>
            {
                Data = response,
                Message = ResponseMessages.CreationSuccessResponse
            };
        }

        public async Task<SuccessResponse<BusinessMessageSettings>> GetByBusinessId(Guid businessGuid)
        {
            if (businessGuid == Guid.Empty) throw new RestException(HttpStatusCode.BadRequest, "No business identifier was provided");

            var get = await _businessMessageSettingsRepository.FirstOrDefault(x => x.BusinessId == businessGuid);
            if (get == null)
                return new SuccessResponse<BusinessMessageSettings>();

            return new SuccessResponse<BusinessMessageSettings>
            {
                Data = get,
                Message = ResponseMessages.RetrievalSuccessResponse
            };
        }

        public async Task<SuccessResponse<BusinessMessageSettings>> GetById(Guid id)
        {
            if (id == Guid.Empty) throw new RestException(HttpStatusCode.BadRequest, "No business identifier was provided");

            var get = await _businessMessageSettingsRepository.FirstOrDefault(x => x.Id == id);
            if (get == null)
                return new SuccessResponse<BusinessMessageSettings>();
            return new SuccessResponse<BusinessMessageSettings>
            {
                Data = get,
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

        public async Task<SuccessResponse<bool>> ProcessUpdate(BusinessMessageSettings model)
        {
            if (model == null || model.Id == Guid.Empty) throw new RestException(HttpStatusCode.BadRequest, "An error occurred. Bad Request");

            var getBusinessId = await _businessMessageSettingsRepository.FirstOrDefault(x=>x.Id == model.Id);
            if (getBusinessId == null)
                throw new RestException(HttpStatusCode.NotFound, "No business settings found");

            getBusinessId.ApiKey = !string.IsNullOrEmpty(model.ApiKey)? model.ApiKey:getBusinessId.ApiKey;
            getBusinessId.WebhookUrl = !string.IsNullOrEmpty(model.WebhookUrl) ? model.WebhookUrl: getBusinessId.WebhookUrl;

            _businessMessageSettingsRepository.Update(getBusinessId);
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

            var getBusinessId = await GetById(id);
            if (getBusinessId == null) return false;

            getBusinessId.Data.TestCounter += 1;

            _businessMessageSettingsRepository.Update(getBusinessId.Data);
            await _businessMessageSettingsRepository.SaveChangesAsync();
            return true;
        }

       // Generate webhook for businesses.
       // TODO: Use app settings to retrieve base url.
  //     public async Task<string> GetBusinessWebHook(Guid businessId)
  //     {
  //          //var baseWebhook = await _systemSettingService.GetSystemSettings();

  //          //if (businessId == Guid.Empty)
  //          //    throw new RestException(HttpStatusCode.Conflict, "The business Id cannot be empty");

  //          //if (baseWebhook is null)
  //          //    throw new RestException(HttpStatusCode.PreconditionFailed, "Base webhook system settings not configured");

  //          //var businessWebHook = $"{baseWebhook.Data.BaseWebhook}/{businessId}/message";
  //          //return businessWebHook;
  //          var baseWebhook = _configuration.BaseUrl;
		//	if (businessId == Guid.Empty)
		//	    throw new RestException(HttpStatusCode.Conflict, "The business Id cannot be empty");

		//	if (baseWebhook is null)
		//	    throw new RestException(HttpStatusCode.PreconditionFailed, "Base webhook system settings not configured");

		//	var businessWebHook = $"{baseWebhook}/{businessId}/message";
		//	    return businessWebHook;
		//}

		//public Task<string> GetWebhookApiKey()
		//{
		//	var baseWebhookApiKey = _configuration.AuthorizationName;
	
		//	if (baseWebhookApiKey is null)
		//		throw new RestException(HttpStatusCode.PreconditionFailed, "Base webhook system settings not configured");

			
		//	return Task.FromResult(baseWebhookApiKey);
		//}
	}
}
