using Application.AutofacDI;
using Infrastructure.Sessions;
using Application.Cron.ResponseProcessing;
using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.Implementations.UserSession
{
    public interface IUserSessionService : IAutoDependencyService
    {
        Task<DialogSession> CheckOrMakeSession(string waId, string name, Guid businessId, _360MessageDto dto);
    }

    public class UserSessionService : IUserSessionService
    {
        private readonly ISessionManagement _session;
      

        private string KeyPrefix = "UserSession_";
        private string BusinessFormSessionIdPrefix = "userBusinessForm_";

        public UserSessionService(ISessionManagement session)
        {
            _session = session;
        }

        public async Task<DialogSession> CheckOrMakeSession(string waId, string name, Guid businessId,
            _360MessageDto dto)
        {    
            var session = await _session.GetByWaId(waId);

            if(session is null)
            {
                var business = new Business() { Id = businessId};
                session = await _session.CreateNewSession(waId,
                    dto.messages.FirstOrDefault().id, name, business,
                    DateTimeHelper.GetDateTimeFromTimeStamp(dto.messages.FirstOrDefault().timestamp),
                    null, null, null);
            }

            //if (session == null)
            //{

            //    UserSessionDto sessionDto = new UserSessionDto
            //    {
            //        StartDateTime = DateTime.UtcNow,
            //        ExpectedEndDateTime = DateTime.UtcNow.AddMinutes(10),
            //        WaId = waId,
            //        SessionId = sessionId,
            //        SessionFlag = SessionFlag.Initiated
            //    };

            //    await _session.AddUserSession(sessionId, sessionDto);

            //    return sessionDto;
            //}
            //else if (session != null)
            //{
            //    session.SessionFlag = SessionFlag.Modified;
            //    session.ExpectedEndDateTime = DateTime.UtcNow.AddMinutes(10);
            //    session.StartDateTime = DateTime.UtcNow;

            //    //Update cache here to extend cache lifespan ..
            //    await _session.UpdateUserSession(sessionId, session);

            //    //check if formprocessing request is present for the user..
            //    var userBusinessFromSession = await _session.GetByWaId(waId);
            //    if(userBusinessFromSession.SessionFormDetails != null 
            //        && dto.messages.FirstOrDefault().text !=null)
            //    {
            //        userBusinessFromSession.SessionFormDetails.ValidationProcessorKey = 
            //            dto.messages.FirstOrDefault().text.body;

            //        userBusinessFromSession.SessionFormDetails.NextFormElement = 
            //            userBusinessFromSession.SessionFormDetails.NextFormElement;

            //        await _session.Update(waId, userBusinessFromSession);
            //    }
            //    else if(dto.messages.FirstOrDefault().interactive == null)
            //    {
            //        //send invalid command to Text messageType func to relay to user ..
            //        var message = "You entered an invalid command. Restart session";
            //        var inboundMessage = new InboundMessage
            //        {
            //            ErrorMessage = "User entered an invalid command, and we couldn't process it",
            //            Body = message,
            //            BusinessId = businessId,
            //            CanUseNLPMapping = false,
            //            Wa_Id = waId,
            //            Name = name,
            //            From = waId,
            //            WhatsUserName = name
            //        };
            //        await _session.RemoveUserSession(sessionId);
            //        await _responsePreProcessingCron.SendUnResolvedMessage(inboundMessage, message);
            //        return null;
            //    }

            //    return session;
            //}
            //else
            //{
            //    //send invalid command to Text messageType func to relay to user ..
            //    var message = "You just entered an invalid command";
            //    var inboundMessage = new InboundMessage
            //    {
            //        ErrorMessage = "User entered an invalid command, and we couldn't process it",
            //        Body = message,
            //        BusinessId = businessId,
            //        CanUseNLPMapping = false,
            //        Wa_Id = waId,
            //        Name = name,
            //        From = waId,
            //        WhatsUserName = name,
            //    };

            //    await _responsePreProcessingCron.SendUnResolvedMessage(inboundMessage, message);
                return session;
            //}
        }
    }
}
