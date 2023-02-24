using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.AutofacDI;
using Application.Helpers;
using Domain.Entities.FormProcessing;

namespace Application.Services.Interfaces.FormProcessing
{
    public interface IUserFormDataService : IAutoDependencyService
    {
        Task<UserFormData> GetUserDateFirstOrDefault(Expression<Func<UserFormData, bool>> func);
        ICollection<UserFormData> GetUsersData(Expression<Func<UserFormData, bool>> func, int size, int count);
        Task Update(UserFormData formElement);
        Task Delete(Guid id);
    }
}

