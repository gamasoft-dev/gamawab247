using Domain.Entities;
using Infrastructure.Data.DbContext;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Infrastructure.Repositories.Implementations
{
    public class WhatsappUserRepository : Repository<WhatsappUser>, IWhatsappUserRepository
    {
        public WhatsappUserRepository(AppDbContext context) : base(context)
        {

        }
        public IQueryable<WhatsappUser> GetWhatsappUsersQuery(string search = null)
        {
            var query = _context.WhatsappUsers.OrderByDescending(x=>x.LastMessageTime).IgnoreQueryFilters() as IQueryable<WhatsappUser>;

            if(!string.IsNullOrEmpty(search))
            {
                var searchQuery = search.Trim();
                query = query.Where(x =>
                x.Name.Contains(searchQuery) ||
                x.PhoneNumber.Contains(searchQuery));
            }

            return query;
        }
    }
}
