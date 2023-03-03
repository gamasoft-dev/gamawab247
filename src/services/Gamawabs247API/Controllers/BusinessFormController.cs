using Application.DTOs.BusinessDtos;
using Application.Helpers;
using Application.Services.Interfaces.FormProcessing;
using Domain.Entities.FormProcessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/business-form")]
    [Produces("application/json")]    

    public class BusinessFormController : ControllerBase
    {
        private readonly IBusinessFormService _businessFormService;

        public BusinessFormController(IBusinessFormService businessFormService)
        {
            _businessFormService = businessFormService;
        }

        [HttpPost()]
        [ProducesResponseType(typeof(SuccessResponse<BusinessFormDto>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateBusinessForm(CreateBusinessFormDto businessFormDto)
        {
            var create = await _businessFormService.CreateBusinessForm(businessFormDto);
            return Ok(create);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateBusinessForm(Guid id, UpdateBusinessFormDto businessFormDto)
        {
            await _businessFormService.UpdateBusinessForm(id, businessFormDto);
            return Ok();
        }

        [HttpGet()]
        [ProducesResponseType(typeof(SuccessResponse<BusinessFormDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> BusinessFormById(Guid id)
        {
            var get = await _businessFormService.GetBusinessFormById(id);
            return Ok(get);
        }

        [HttpGet("get-form-by-businessid/{businessId}")]
        [ProducesResponseType(typeof(SuccessResponse<BusinessForm>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> BusinessFormByBusinessId(Guid businessId)
        {
            var get = await _businessFormService.GetBusinessFormByBusinessId(businessId);
            return Ok(get);
        }

        [HttpGet("get-all-business-forms")]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<BusinessForm>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllBusinessFormById(string search, string name, [FromQuery]ResourceParameter parameter)
        {
            var getAll = await _businessFormService.GetAllBusinessFormByUserId(search, nameof(BusinessForm), parameter, Url);
            return Ok(getAll);
        }
    }
}