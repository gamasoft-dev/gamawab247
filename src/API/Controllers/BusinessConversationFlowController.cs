using Application.DTOs;
using Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
	public partial class BusinessesController
	{
		/// <summary>
		/// Create a new conversation flow.
		/// </summary>
		/// <param name="model"></param>
		/// /// <param name="id">businessId</param>
		/// <returns></returns>
		[HttpPost("{id}/conversation-flow")]
		[ProducesResponseType(typeof(GetBusinessConversationFlowDto), (int)HttpStatusCode.Created)]
		public async Task<IActionResult> CreateConversationFlow([FromRoute] Guid id, [FromBody] CreateBusinessConversationFlowDto model)
		{
			var response = await _conversationFlowService.CreateConversationFlow(id, model);

			return CreatedAtAction( actionName: nameof(GetById), new { id = response.Data.Id }, response);
		}

		/// <summary>
		/// Update Conversation flow by id.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut("conversation-flow/{id}")]
		[ProducesResponseType(typeof(GetBusinessConversationFlowDto), (int)HttpStatusCode.OK)]
		public async Task<IActionResult> UpdateConversationFlow([FromRoute] Guid id, [FromBody] UpdateBusinessConversationFlowDto model)
		{
			var response = await _conversationFlowService.UpdateConversationFlow(id, model);

			return Ok(response);
		}

		/// <summary>
		/// Delete a conversaiton by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("conversation-flow/{id}")]
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
		[HttpGet("conversation-flow/{id}", Name = nameof(GetConversationFlowById))]
		[ProducesResponseType(typeof(GetBusinessConversationFlowDto), (int)HttpStatusCode.OK)]
		public async Task<IActionResult> GetConversationFlowById([FromRoute] Guid id)
		{
			return Ok(await _conversationFlowService.GetById(id));
		}

		/// <summary>
		/// Get all resource paramters with pagination and sorting functionality.
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		[HttpGet("conversation-flow")]
		[ProducesResponseType(typeof(PagedList<GetBusinessConversationFlowDto>), (int)HttpStatusCode.OK)]
		public async Task<IActionResult> GetAllConversationFlows([FromQuery] ResourceParameter parameter)
		{
			var response = await _conversationFlowService.GetConversationFlows(parameter);

			return Ok(response);
		}
	}
}
