using Application.DTOs.BusinessDtos;
using Application.Helpers;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
	[ApiController]
	[ApiVersion("1.0")]
	[Route("api/v{version:apiVersion}/business-conversation")]
	public class BusinessConversationController : ControllerBase
	{
		private readonly IBusinessConversation _businessConversation;
		private readonly ILogger<IndustryController> _logger;
		public BusinessConversationController(IBusinessConversation businessConversation, ILogger<IndustryController> logger)
		{
			_businessConversation = businessConversation;
			_logger = logger;
		}

		[HttpGet(Name = nameof(GetAllConversation))]
		[ProducesResponseType(typeof(PagedResponse<BusinessConversationDto>),200)]
		public async Task<IActionResult> GetAllConversation([FromQuery] ResourceParameter parameter)
		{
			try
			{
				var result = await _businessConversation.GetAllBusinessConversations(parameter, nameof(GetAllConversation), Url);
				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex.Message, ex);
				throw;
			}
		}

		[HttpGet("{id}", Name = nameof(GetConversationById))]
		[ProducesResponseType(typeof(SuccessResponse<BusinessConversationDto>), 200)]
		public async Task<IActionResult> GetConversationById([FromQuery] Guid id)
		{
			try
			{
				var result = await _businessConversation.GetBusinessConversationByBusinessId(id);
				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex.Message, ex);
				throw;
			}
		}

		[HttpPost]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.Created)]
		public async Task<IActionResult> CreateConversation([FromBody] CreateBusinessConversationDtoo model)
		{
			try
			{
				var result = await _businessConversation.CreateMessage(model);
				return CreatedAtAction(nameof(GetConversationById), new { id = result.Data.Id }, result);
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex.Message, ex);
				throw;
			}
		}


		[HttpPut]
		[ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
		public async Task<IActionResult> UpdateConversation([FromQuery] UpdateBusinessConversationDto model)
		{
			try
			{
				var result = await _businessConversation.UpdateBusinessConversation(model);
				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex.Message, ex);
				throw;
			}
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
		public async Task<IActionResult> DeleteConversation([FromQuery] Guid id)
		{
			try
			{
				await _businessConversation.DeleteBusinessConversation(id);
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
