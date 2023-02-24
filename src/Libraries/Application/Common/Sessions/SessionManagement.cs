using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.AutofacDI;
using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Entities.FormProcessing;
using Domain.Entities.FormProcessing.ValueObjects;
using Domain.Enums;

namespace Application.Common.Sessions
{
    public class SessionManagement: ISessionManagement
    {
        private readonly ICacheService _cacheService;
        public SessionManagement(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }
        /// <summary>
        /// KVP of whatsapp user Id and the DialogSession object
        /// </summary>

        public async Task<DialogSession> CreateNewSession(string waId,
            string messageId, string userName, Business business,
            DateTime? messageTime = null,
            ESessionState? eSessionState = null,
            SessionFormDetail sessionFormDetail = null,
            Guid? businessFormId = null)
        {
            var session = await GetByWaId(waId);
            if (session is not null)
            {
                session.SessionState = eSessionState == null ? session.SessionState : eSessionState.Value ;
                session.UpdatedAt = DateTime.Now;
                session.BusinessId = business.Id;
                session.CreatedAt = DateTime.Now;
                session.UserName = userName ?? waId;
                session.WaId = waId;
                session.LastMessageId = messageId;

                if(messageTime is not null)
                    session.LastInboundMessageTime = messageTime;

                await Update(waId, session);
            }
            else
            {
                session = new DialogSession();
                session.SessionState = eSessionState == null ? ESessionState.PLAINCONVERSATION : eSessionState.Value;
                session.UpdatedAt = DateTime.Now;
                session.BusinessId = business.Id;
                session.CreatedAt = DateTime.Now;
                session.UserName = userName ?? waId;
                session.Id = Guid.NewGuid();
                session.SessionFormDetails = sessionFormDetail;
                session.WaId = waId;
                session.LastInboundMessageTime = messageTime;
                session.LastMessageId = messageId;

                await  Create(waId, session);
            }
            return session;
        }

        public async Task<DialogSession> GetByWaId(string waId)
        {
            return await _cacheService.ReadFromCacheAsync<DialogSession>(waId);
        }

        public async Task Update(string waId, DialogSession model)
        {
            await _cacheService.UpdateCache(waId, model);
        }

      
        public async Task<BusinessForm> RetrieveFormForUser(string key)
        {
            var form = await _cacheService.ReadFromCacheAsync<BusinessForm>(key);
            return form;
        }

        public async Task RemoveUserSession(string key)
        {
           await _cacheService.RemoveFromCache(key);
        }

        private async Task Create(string waId, DialogSession model)
        {
            await _cacheService.AddToCacheAsync(waId, model);
        }
    }

    public interface ISessionManagement: IAutoDependencyService
    {
        Task Update(string  waId, DialogSession dialogSession);
        Task<DialogSession> GetByWaId(string waId);
        Task<DialogSession> CreateNewSession(string waId, string messageId, string userName, Business business,
            DateTime? messageTime = null,
            ESessionState? eSessionState = null, SessionFormDetail sessionFormDetail = null, Guid? businessFormId = null);

        Task RemoveUserSession(string key);
    }
}