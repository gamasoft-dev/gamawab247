using Domain.Entities;
using System;
using System.Linq;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IMessageLogRepository : IRepository<MessageLog>
    {
        IQueryable<MessageLog> GetMessageLogQuery(string search = null);
        IQueryable<MessageLog> CreateMessageLogQuerable(Guid waId, string search = null);
    }
}
