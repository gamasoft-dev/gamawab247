using Domain.Entities.Identities;
using System;
using System.Linq;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        IQueryable<User> GetUsersQuery(string search = null);
        IQueryable<User> GetBusinessUsersQuery(Guid businessId, string search = null);
    }
}
