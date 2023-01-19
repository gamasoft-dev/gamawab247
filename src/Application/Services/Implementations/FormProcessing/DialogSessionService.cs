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

namespace Application.Services.Implementations.FormProcessing
{
    public class DialogSessionService : IDialogSessionService
    {
        private readonly IRepository<DialogSession> _dialogSessionRepository;
        private readonly IMapper _mapper;

        public DialogSessionService(IRepository<DialogSession> dialogSessionRepository, IMapper mapper)
        {
            _dialogSessionRepository = dialogSessionRepository;
            _mapper = mapper;
        }

        public async Task<SuccessResponse<DialogSessionDto>> CreateUserFormData(CreateDialogSessionDto model)
        {
            var dialogSession = _mapper.Map<DialogSession>(model);

            await _dialogSessionRepository.AddAsync(dialogSession);
            await _dialogSessionRepository.SaveChangesAsync();

            var response = _mapper.Map<DialogSessionDto>(dialogSession);

            return new SuccessResponse<DialogSessionDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = response
            };
        }


        public async Task<SuccessResponse<DialogSessionDto>> GetUserFormDataById(Guid id)
        {
            var dialogSession = await _dialogSessionRepository.FirstOrDefault(x => x.Id == id);
            if (dialogSession == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.DialogSessionNotFound);

            var response = _mapper.Map<DialogSessionDto>(dialogSession);
            return new SuccessResponse<DialogSessionDto>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = response
            };
        }

        public async Task<SuccessResponse<DialogSessionDto>> UpdateUserFormData(UpdateDialogSessionDto model)
        {
            var newDialogSession = _mapper.Map<DialogSession>(model);
            _dialogSessionRepository.Update(newDialogSession);
            await _dialogSessionRepository.SaveChangesAsync();

            var response = _mapper.Map<DialogSessionDto>(newDialogSession);

            return new SuccessResponse<DialogSessionDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = response
            };
        }
    }
}
