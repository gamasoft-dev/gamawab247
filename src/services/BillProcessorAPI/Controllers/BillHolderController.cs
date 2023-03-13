using BillProcessorAPI.Dtos;
using BillProcessorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillProcessorAPI.Controllers
{

	[ApiController]
	[ApiVersion("1.0")]
	[Route("v1/abc")]
	[Produces("application/json")]
	public class BillHolderController : ControllerBase
	{
		private readonly IBillService _revpay;
		public BillHolderController(IBillService revpay)
		{
			_revpay = revpay;
		}

		[HttpGet("reference-verification")]
		public async Task<IActionResult> ReferenceVerification([FromQuery] BillRequestDto model)
		{
			var response = await _revpay.ReferenceVerification(model);
			return Ok(response);
		}

		[HttpGet("payment-verification")]
		public async Task<IActionResult> PaymentVerification([FromQuery] BillRequestDto model)
		{
			var response = await _revpay.PaymentVerification(model);
			return Ok(response);
		}
	}
}
