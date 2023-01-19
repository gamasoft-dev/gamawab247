using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class MessageSessionService : IMessageSessionService
    {
        private readonly IRepository<OutboundMessage> _messageSession;
        private readonly IMapper _mapper;

        public MessageSessionService(IRepository<OutboundMessage> messageSession, IMapper mapper)
        {
            _messageSession = messageSession;
            _mapper = mapper;
        }

        public async Task<OutboundMessage> CreateMessageSession(CreateMessageSessionDto messageSession)
        {
            if (messageSession is null)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, "Validation for message session");

            var session = _mapper.Map<OutboundMessage>(messageSession);

            //save to mesageSession
            await _messageSession.AddAsync(session);
            await _messageSession.SaveChangesAsync();

            return session;
        }

        public async Task<OutboundMessage> GetMessageSession(Guid messageSessionId)
        {
            if (messageSessionId == Guid.Empty)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, "Validation for message session");

            var get = await _messageSession
                .FirstOrDefault(x => x.Id == messageSessionId && (x.CreatedAt >= DateTime.Now.AddHours(24)
                || x.CreatedAt <= DateTime.Now.AddHours(24)));

            if (get is null)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, "Validation for message session");

            return get;
        }
    }
}
