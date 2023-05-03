
using System;
using System.Net;
using System.Threading.Tasks;
using Application.DTOs.InteractiveMesageDto;
using Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Gamawabs247API.Controllers
{
	/// <summary>
	/// This module is responsible to receive template message that needs to be sent out directly 
	/// </summary>
	public class MessageController: ControllerBase
	{
		public MessageController()
		{
		}



        [HttpPost("business/{phoneNumber}/direct-broadcast")]
        [ProducesResponseType(typeof(SuccessResponse<>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SendTextMessage()
		{
			throw new NotImplementedException("endpoint not implemented");
		}
	}
}