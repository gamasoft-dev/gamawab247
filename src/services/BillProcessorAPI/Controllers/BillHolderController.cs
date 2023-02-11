using BillProcessorAPI.Dtos;
using BillProcessorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillProcessorAPI.Controllers
{

	[ApiController]
	[ApiVersion("1.0")]
	[Route("api/v{version:apiVersion}/business-form")]
	[Produces("application/json")]
	public class BillHolderController : ControllerBase
	{
		private readonly IBillService _revpay;
		public BillHolderController(IBillService revpay)
		{
			_revpay = revpay;
		}

		[HttpPost("reference-verification")]
		public async Task<IActionResult> ReferenceVerification(BillReferenceRequestDto model)
		{
			var response = await _revpay.ReferenceVerification(model);
			return Ok(response);
		}

		[HttpPost("payment-veification")]
		public async Task<IActionResult> PaymentVerification(BillPaymentVerificationRequestDto model)
		{
			var response = await _revpay.PaymentVerification(model);
			return Ok(response);
		}
	}
}
