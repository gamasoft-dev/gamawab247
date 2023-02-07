using API.Extensions;
using Application.DTOs;
using Application.Exceptions;
using Application.Services.Implementations.UserSession;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Middlewares
{
    public class SessionCheckerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUserSessionService _userSessionService;
        private readonly IMessageProcessor _messageProcessor;

        public SessionCheckerMiddleware(RequestDelegate next, IUserSessionService userSessionService,
            IMessageProcessor messageProcessor)
        {
            _next = next;
            _userSessionService = userSessionService;
            _messageProcessor = messageProcessor;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var rawRequest = await httpContext.Request.GetRawBodyStringAsync();
            var businessId = FilterBusinessId(httpContext.Request.Path);

            // validate request is valid inbound message
            try
            {
                await _messageProcessor.ValidateInboundMessage(businessId, rawRequest);
            }
            catch (ProcessCancellationException)
            {
                return;
            }
            

            var requestPayload = GetDialo360Message(rawRequest);


            await _userSessionService.CheckOrMakeSession(
                requestPayload.contacts.FirstOrDefault().wa_id,
                requestPayload.contacts.FirstOrDefault().profile.name,
                businessId, requestPayload);

            await _next(httpContext);
        }

        static _360MessageDto GetDialo360Message(string request)
        {
            if (string.IsNullOrEmpty(request))
                throw new ProcessCancellationException("Empty validate request");

            return JsonConvert.DeserializeObject<_360MessageDto>(request);
        }

        static Guid FilterBusinessId(string path)
        {
            var convertToString = path.Split("/");
            foreach (var item in convertToString)
            {
                if(Guid.TryParse(item, out Guid outPuttedGuid))
                {
                    return outPuttedGuid;
                }
            }
            return Guid.Empty;
        }
    }
}