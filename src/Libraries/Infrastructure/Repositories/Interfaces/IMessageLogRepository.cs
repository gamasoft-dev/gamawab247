using Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IMessageLogRepository : IRepository<MessageLog>
    {
        IQueryable<MessageLog> GetMessageLogQuery(string search = null);
        IQueryable<MessageLog> CreateMessageLogQuerable(Guid waId, string search = null);
        IQueryable<MessageLog> CreateMessageLogQuerable(string waId, string search = null);
        Task<int> MessageLogCountAsync();
    }
}
