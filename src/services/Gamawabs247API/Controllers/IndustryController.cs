using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Application.Helpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/industry")]
    public class IndustryController : ControllerBase
    {
        private readonly ILogger<IndustryController> _logger;
        private readonly IIndustryService _industryService;
        public IndustryController(ILogger<IndustryController> logger, IIndustryService industryService)
        {
            _logger = logger;
            _industryService = industryService;
        }


        [HttpGet(Name = nameof(GetAllIndustry))]
        [ProducesResponseType(typeof(IEnumerable<IndustryDto>), 200)]
        [Authorize]
        public async Task<IActionResult> GetAllIndustry([FromQuery] ResourceParameter parameter)
        {
            try
            {
                return Ok(await _industryService.GetAllIndustry(parameter, nameof(GetAllIndustry), Url));
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }

        // GET api/<IndustryController>/5
        /// <summary>
        /// Get industry by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetIndustryById))]
        [ProducesResponseType(typeof(SuccessResponse<IndustryDto>),200)]
        public async Task<IActionResult> GetIndustryById(Guid id)
        {
            try
            {
                return Ok(await _industryService.GetIndustryById(id));
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }

        /// <summary>
        /// Creates industry
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(SuccessResponse<IndustryDto>), (int)HttpStatusCode.Created)]
        [Authorize]//for admin usage
        public async Task<IActionResult> Post([FromBody] CreateIndustryDto dto)
        {
            try
            {
                var result = await _industryService.CreateIndustry(dto);
                return CreatedAtAction(nameof(GetIndustryById), new { id = result.Data.Id },dto);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Updates industry information
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        // PUT api/<IndustryController>/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<IndustryDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(SuccessResponse<IndustryDto>), (int)HttpStatusCode.BadRequest)]
        [Authorize]//for admin usage
        public async Task<IActionResult> Put(Guid id,[FromBody] UpdateIndustryDto dto)
        {
            try
            {
                var update = await _industryService.UpdateIndustry(id, dto);
                return Ok(update);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// deletes industry by industry identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE api/<IndustryController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<bool>), (int)HttpStatusCode.OK)]
        [Authorize]//for admin usage
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _industryService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }
    }
}
