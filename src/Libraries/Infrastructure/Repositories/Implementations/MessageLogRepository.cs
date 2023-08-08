using Domain.Entities;
using Infrastructure.Data.DbContext;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementations
{
    public class MessageLogRepository : Repository<MessageLog>, IMessageLogRepository
    {
        public MessageLogRepository(AppDbContext context) : base(context)
        {
                
        }

        public IQueryable<MessageLog> CreateMessageLogQuerable(Guid waId, string search = null)
        {
            var messageLogQuery = base.Query(x => x.WhatsappUserId == waId);

            search = !string.IsNullOrEmpty(search) ? search.Trim() : string.Empty;

            messageLogQuery = messageLogQuery.Where(x =>
            (string.IsNullOrEmpty(search) ||
            (x.From.Contains(search)
            || x.To.Contains(search)
            || x.Business.Name.Contains(search)
            || x.WhatsappUser.Name.Contains(search))));

            return messageLogQuery;
        }

        public IQueryable<MessageLog> CreateMessageLogQuerable(string waId, string search = null)
        {
            var messageLogQuery = base.Query(x => x.WhatsappUser.WaId == waId);

            search = !string.IsNullOrEmpty(search) ? search.Trim() : string.Empty;

            messageLogQuery = messageLogQuery.Where(x =>
            (string.IsNullOrEmpty(search) ||
            (x.From.Contains(search)
            || x.To.Contains(search)
            || x.Business.Name.Contains(search)
            || x.WhatsappUser.Name.Contains(search))));

            return messageLogQuery;
        }

        public IQueryable<MessageLog> GetMessageLogQuery(string search = null)
        {
            var query = _context.MessageLogs.IgnoreQueryFilters() as IQueryable<MessageLog>;

            if (!string.IsNullOrEmpty(search))
            {
                var searchQuery = search.Trim();
                query = query.Where(x =>
                x.To.Contains(searchQuery) ||
                x.From.Contains(searchQuery));
            }

            return query;
        }

        public async Task<int> MessageLogCountAsync()
        {
            return await _context.MessageLogs.CountAsync();
        }
    }
}
