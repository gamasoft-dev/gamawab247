using Application.Exceptions;
using Application.Helpers;
using Application.Services.Interfaces.FormProcessing;
using Domain.Common;
using Domain.Entities.FormProcessing;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class FormRequestResponseService : IFormRequestResponseService
    {
        private readonly IRepository<FormRequestResponse> _formRequestResponseRepository;

        public FormRequestResponseService(IRepository<FormRequestResponse> formRequestResponseRepository)
        {
            _formRequestResponseRepository = formRequestResponseRepository;
        }

        public async Task Delete(Guid id)
        {
            var formRequestResponse = await _formRequestResponseRepository.FirstOrDefault(x => x.Id == id);
            if (formRequestResponse is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserFormRequestNotFound);

            _formRequestResponseRepository.Remove(formRequestResponse);
            await _formRequestResponseRepository.SaveChangesAsync();
        }

        public IEnumerable<FormRequestResponse> GetAll(Expression<Func<FormRequestResponse, bool>> func, int skip, int take, bool includeBusinessForm = true)
        {
            var formRequestsResponse = _formRequestResponseRepository.Query(func).Include(x=>x.BusinessForm);

            //if (includeBusinessForm)
            //    formRequestsResponse.Include(x => x.BusinessForm);
            
            var items = formRequestsResponse.Skip(skip).Take(take).OrderBy(x=>x.CreatedAt);

            var result = items.ToList();
            return result;
        }

        public async Task<FormRequestResponse> GetFirstOrDefault(Expression<Func<FormRequestResponse, bool>> expression)
        {
            return await _formRequestResponseRepository.FirstOrDefault(expression);
        }

        public async Task Update(FormRequestResponse formElement)
        {
            var formElementToUpdate = await _formRequestResponseRepository.FirstOrDefault(x => x.Id == formElement.Id);

            if (formElementToUpdate is null)
                throw new NotFoundException($"FormRequestResponse with provided {formElement.Id} not found", formElement.Id);

            _formRequestResponseRepository.Update(formElement);
            await _formRequestResponseRepository.SaveChangesAsync();
        }

        public async Task Create(FormRequestResponse formRequest)
        {
            await _formRequestResponseRepository.AddAsync(formRequest);
            await _formRequestResponseRepository.SaveChangesAsync();
        }

        public async Task Create(List<FormRequestResponse> formRequestResponses)
        {
            await _formRequestResponseRepository.AddRangeAsync(formRequestResponses);
            await _formRequestResponseRepository.SaveChangesAsync();
        }

        public async Task Update(IEnumerable<FormRequestResponse> formElement)
        {
             _formRequestResponseRepository.UpdateRange(formElement);
            await _formRequestResponseRepository.SaveChangesAsync();
        }
    }
}
