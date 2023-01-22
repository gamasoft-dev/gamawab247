using System;
using System.Linq;
using System.Linq.Expressions;
using Domain.Entities;
using Domain.Entities.Identities;
using Infrastructure.Data.DbContext;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories.Implementations
{
    public class UserActivityRepository: Repository<UserActivity>, IUserActivityRepository
    {
        public UserActivityRepository(AppDbContext context): base(context)
        {

        }
    }
}
