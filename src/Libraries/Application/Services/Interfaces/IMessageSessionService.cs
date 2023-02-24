using Application.AutofacDI;
using Application.DTOs;
using Application.Helpers;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IMessageSessionService : IAutoDependencyService
    {
        Task<OutboundMessage> CreateMessageSession(CreateMessageSessionDto messageSession);
        Task<OutboundMessage> GetMessageSession(Guid messageSessionId);
    }
}