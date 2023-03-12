using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.AutofacDI;
using Application.Helpers;
using Domain.Entities.FormProcessing;

namespace Application.Services.Interfaces.FormProcessing
{
    public interface IFormRequestResponseService : IAutoDependencyService
    {
        Task<FormRequestResponse> GetFirstOrDefault(Expression<Func<FormRequestResponse, bool>> func);
        IEnumerable<FormRequestResponse> GetAll(Expression<Func<FormRequestResponse, bool>> func, int skip, int take, bool includeBusinessForm = true);
        Task Update(FormRequestResponse formElement);
        Task Delete(Guid id);
        Task Create(List<FormRequestResponse> formRequestResponses);
        Task Create(FormRequestResponse formRequest);
        Task Update(IEnumerable<FormRequestResponse> formElement);
    }
}

