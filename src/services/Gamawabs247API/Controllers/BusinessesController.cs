using Application.DTOs;
using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Application.DTOs.BusinessDtos;
using Application.Helpers;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/business")]
    public partial class BusinessesController : ControllerBase
    {
       
        #region Business region
        /// <summary>
        /// Creates new business. Allow both authorized and anonymous...
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(SuccessResponse<BusinessDto>), 201)]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> CreateBusiness (CreateBusinessDto dto)
        {
            try
            {
                var create = await _businessService.ProcessCreateBusiness(dto);

                return CreatedAtAction(nameof(GetById), new { id = create.Data.Id }, create);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }
        
        [HttpGet(Name = nameof(GetBusinesses))]
        [ProducesResponseType(typeof(PagedResponse<BusinessDto>), 200)]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> GetBusinesses([FromQuery] ResourceParameter parameter)
        {
            try
            {
                var result = await _businessService.GetAllBusinesses(parameter, nameof(GetBusinesses), Url);;
                return Ok(result);
            }
            catch (Exception e)
            { 
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }

        /// <summary>
        /// Gets user's business by the business  identifier... E.G Id=jwbhe-22kdwe-wehwe
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetById))]
        [ProducesResponseType(typeof(SuccessResponse<BusinessDto>), 200)]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var business = await _businessService.GetBusinessByBusinessId(id);
                return Ok(business);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }
        [HttpGet("email")]
        [ProducesResponseType(typeof(SuccessResponse<IEnumerable<Business>>), 200)]
        [Authorize]
        public async Task<IActionResult> GetBusinessesByUserEmail(string email)
        {
            var businesses = await _businessService.GetAllBusinessesByUserEmail(email);
            return Ok(businesses);
        }

        /// <summary>
        /// Updates business info
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(SuccessResponse<BusinessDto>), 200)]
        [Authorize(Roles = "SUPERADMIN, ADMIN")]
        public async Task<IActionResult> UpdateBusinessInfo(Business model)
        {
            try
            {
                var update = await _businessService.UpdateBusinessInfo(model);

                return Ok(update);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }

        /// <summary>
        /// Deletes business info by business identifier...
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> DeleteBusinessInfo(Guid id)
        {
            try
            {
                await _businessService.DeleteBusinessById(id); 
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }


        /// <summary>
        /// Endpoint to get a paginated list of users
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet("{id}/users", Name = nameof(GetBusinessUsers))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<UserResponse>>), 200)]
        public async Task<IActionResult> GetBusinessUsers(Guid id, [FromQuery] ResourceParameter parameter)
        {
            var response = await _userService.GetUsersByBusinessId(id, parameter, nameof(UserResponse), Url);

            return Ok(response);
        }
        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Business message setup region
        /// <summary>
        /// Creates business message template. Only allows authorized users.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("{id}/business-setup")]
        [ProducesResponseType(typeof(SuccessResponse<string>), 201)]
        [Authorize(Roles = "SUPERADMIN, ADMIN")]
        public async Task<IActionResult> CreateBusinessMessageSetup(Guid id, CreateBusinessSetupDto dto)
        {
            try
            {
                var response = await _businessSettingsService.ProcessCreateBusinessSetting(id, dto);
                return CreatedAtAction(nameof(GetSetupByBusinessSetupId),new { response.Data.Id}, response);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }

        /// <summary>
        /// Gets business message by business identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/business-setup",Name =nameof(GetSetupByBusinessSetupId))]
        [ProducesResponseType(typeof(SuccessResponse<BusinessMessageSettings>), 200)]
        [Authorize]
        public async Task<IActionResult> GetSetupByBusinessSetupId(Guid id)
        {
            try
            {
                var businessSettings = await _businessSettingsService.GetByBusinessId(id);

                return Ok(businessSettings);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }

        /// <summary>
        /// Get business by business-setup id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("business-setup/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<BusinessMessageSettings>), 200)]
        [Authorize]
        public async Task<IActionResult> GetBySetupId(Guid id)
        {
            try
            {
                var businessSettings = await _businessSettingsService.GetById(id);

                return Ok(businessSettings);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }

        /// <summary>
        /// Updates the business setup properties/model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("business-setup/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<bool>), 200)]
        [Authorize(Roles = "SUPERADMIN, ADMIN")]
        public async Task<IActionResult> UpdateBusinessSetupInfo(Guid id, BusinessMessageSettings dto)
        {
            try
            {
                dto.Id = id;
                await _businessSettingsService.Update(dto);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }
        #endregion
    }
}
