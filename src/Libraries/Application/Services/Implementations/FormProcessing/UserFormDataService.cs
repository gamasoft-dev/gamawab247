using Application.Helpers;
using Application.Services.Interfaces;
using Application.Services.Interfaces.FormProcessing;
using Domain.Common;
using Domain.Entities.FormProcessing;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace Application.Services.Implementations.FormProcessing
{
    public class UserFormDataService : IUserFormDataService
    {
        private readonly IRepository<UserFormData> _userFormDataRepository;

        public UserFormDataService(IRepository<UserFormData> userFormDataRepository)
        {
            _userFormDataRepository = userFormDataRepository;
        }

        public async Task Delete(Guid id)
        {

            var userFormData = await _userFormDataRepository.FirstOrDefault(x => x.Id == id);
            if (userFormData == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserFormDataNotFound);

            _userFormDataRepository.Remove(userFormData);
            await _userFormDataRepository.SaveChangesAsync();
        }

        public async Task<UserFormData> GetUserDateFirstOrDefault(Expression<Func<UserFormData, bool>> func)
        {
            var userFormData = await _userFormDataRepository.FirstOrDefault(func);
            if (userFormData == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserFormDataNotFound);

            return userFormData;
        }

        public ICollection<UserFormData> GetUsersData(Expression<Func<UserFormData, bool>> func, int skip, int take)
        {
            var usersData = _userFormDataRepository.Query(func);


            var items = usersData.Skip(skip).Take(take);

            return new Collection<UserFormData>(items.ToList());
        }

        public async Task Update(UserFormData formElement)
        {
            var formElementToUpdate = await _userFormDataRepository.FirstOrDefault(x => x.Id == formElement.Id);
            _userFormDataRepository.Update(formElement);
            await _userFormDataRepository.SaveChangesAsync();
        }
    }
}
