using Domain.Entities.Identities;
using Infrastructure.Data.DbContext;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Infrastructure.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {

        }
        public IQueryable<User> GetUsersQuery(string search = null)
        {
            var query = _context.Users.IgnoreQueryFilters() as IQueryable<User>;

            if (!string.IsNullOrEmpty(search))
            {
                var searchQuery = search.Trim();
                query = query.Where(x =>
                x.FirstName.Contains(searchQuery) ||
                x.LastName.Contains(searchQuery));
            }

            return query;
        }

        public IQueryable<User> GetBusinessUsersQuery(Guid businessId, string search = null)
        {
            var query = _context.Users.Where(x => x.BusinessId == businessId) as IQueryable<User>;

            if (!string.IsNullOrEmpty(search))
            {
                var searchQuery = search.Trim();
                query = query.Where(x =>
                (x.FirstName.Contains(searchQuery) ||
                x.LastName.Contains(searchQuery)) || x.PhoneNumber.Contains(searchQuery));
            }

            return query;
        }
    }
}
