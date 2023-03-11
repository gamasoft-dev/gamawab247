using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.FormProcessing.ValueObjects;
using Domain.Enums;

namespace Infrastructure.Sessions
{
    public interface ISessionManagement
    {
        Task Update(string  waId, DialogSession dialogSession);
        Task<DialogSession> GetByWaId(string waId);
        Task<DialogSession> CreateNewSession(string waId, string messageId, string userName, Business business,
            DateTime? messageTime = null,
            ESessionState? eSessionState = null, SessionFormDetail sessionFormDetail = null, Guid? businessFormId = null);

        Task RemoveUserSession(string key);
    }
}