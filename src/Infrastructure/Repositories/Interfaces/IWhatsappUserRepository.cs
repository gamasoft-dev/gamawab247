using Domain.Entities;
using System.Linq;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IWhatsappUserRepository : IRepository<WhatsappUser>
    { 
        IQueryable<WhatsappUser> GetWhatsappUsersQuery(string search = null);
    }
}
