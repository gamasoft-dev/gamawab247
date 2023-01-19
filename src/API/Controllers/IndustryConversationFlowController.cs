using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
	[ApiController]
	[ApiVersion("1.0")]
	[Route("api/{version:apiVersion}/industry-conversation")]
	public class IndustryConversationFlowController: ControllerBase
	{
		private readonly IIndustryConversationFlowService _conversationFlowService;
		public IndustryConversationFlowController(IIndustryConversationFlowService industryConversationFlowService)
		{
			_conversationFlowService = industryConversationFlowService;
		}

		/// <summary>
		/// Create a new conversation flow.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[ProducesResponseType(typeof(GetIndustryConversationFlowDto), (int)HttpStatusCode.Created)]
		public async Task<IActionResult> CreateConversationFlow([FromBody] CreateIndustryConversationFlowDto model)
		{
			var response = await _conversationFlowService.CreateConversationFlow(model);

			return CreatedAtAction( actionName: nameof(GetById), new { id = response.Data.Id }, response);
		}

		/// <summary>
		/// Update Conversation flow by id.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut("{id}")]
		[ProducesResponseType(typeof(GetIndustryConversationFlowDto), (int)HttpStatusCode.OK)]
		public async Task<IActionResult> UpdateConversationFlow([FromRoute] Guid id, [FromBody] UpdateIndustryConversationFlowDto model)
		{
			var response = await _conversationFlowService.UpdateConversationFlow(id, model);

			return Ok(response);
		}

		/// <summary>
		/// Delete a conversaiton by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("{id}")]
		[ProducesResponseType((int)HttpStatusCode.NoContent)]
		public async Task<IActionResult> DeleteConversationFlow ([FromRoute] Guid id)
		{
			await _conversationFlowService.Delete(id);

			return NoContent();
		}

		/// <summary>
		/// Get conversation by Id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}", Name = nameof(GetById))]
		[ProducesResponseType(typeof(GetIndustryConversationFlowDto), (int)HttpStatusCode.OK)]
		public async Task<IActionResult> GetById([FromRoute] Guid id)
		{
			return Ok(await _conversationFlowService.GetById(id));
		}

		/// <summary>
		/// Get all resource paramters with pagination and sorting functionality.
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		[HttpGet]
		[ProducesResponseType(typeof(PagedList<GetIndustryConversationFlowDto>), (int)HttpStatusCode.OK)]
		public async Task<IActionResult> GetAllConversationFlows([FromQuery] ResourceParameter parameter)
		{
			var response = await _conversationFlowService.GetConversationFlows(parameter);

			return Ok(response);
		}
	}
}
