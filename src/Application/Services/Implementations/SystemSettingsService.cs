using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class SystemSettingsService : ISystemSettingsService
    {
        private readonly IRepository<SystemSettings> _repository;
        private readonly IMapper _mapper;
     

        public SystemSettingsService(IRepository<SystemSettings> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SuccessResponse<SystemSettingsDto>> CreateUpdateSystemSettings(CreateSystemSetttingsDto dto)
        {
            if (dto == null)
                throw new RestException(HttpStatusCode.BadRequest, message: ResponseMessages.PayLoadCannotBeNull);
                       
            var systemSettings = await _repository.FirstOrDefault(z=>z.Id != Guid.Empty);

            if (systemSettings is null)
            {             
                systemSettings = _mapper.Map<SystemSettings>(dto);
                systemSettings.CreatedAt = DateTime.Now;

                await _repository.AddAsync(systemSettings);
            }
            else
            {
                systemSettings.MaxTestCount = dto.MaxTestCount;
                systemSettings.BaseWebhook = dto.BaseWebHook;

                systemSettings.UpdatedAt = DateTime.Now;
            }

            await _repository.SaveChangesAsync();

            var result = await GetSystemSettings();
            var response = _mapper.Map<SystemSettingsDto>(result);

            return new SuccessResponse<SystemSettingsDto>
            {
                Data = response,
                Message = ResponseMessages.CreationSuccessResponse
            };
        }

        public async Task<SuccessResponse<SystemSettingsDto>> GetSystemSettings()
        {
            var systensettings = await _repository.FirstOrDefaultNoTracking(x=>x.Id != Guid.Empty);

            if (systensettings is null)
                throw new RestException(HttpStatusCode.BadRequest, "No system systems has been configures");

            var settingDto = _mapper.Map<SystemSettingsDto>(systensettings);

            return new SuccessResponse<SystemSettingsDto>
            {
                Data = settingDto,
                Message = ResponseMessages.RetrievalSuccessResponse
            };
        }
    }
}
