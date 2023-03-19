using BillProcessorAPI.Dtos;
using BillProcessorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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

		[HttpGet("reference-verification/{billPaymentCode}")]
        [ProducesResponseType(typeof(BillReferenceResponseDto), 200)]
        [SwaggerOperation(Summary = "Endpoint to get bill payer reference")]
        public async Task<IActionResult> ReferenceVerification([FromRoute] string phone,string billPaymentCode)
		{
			var response = await _revpay.ReferenceVerification(phone, billPaymentCode);
			return Ok(response);
		}

		[HttpGet("payment-verification/{billPaymentCode}")]
        [ProducesResponseType(typeof(BillPaymentVerificationResponseDto), 200)]
        [SwaggerOperation(Summary = "Endpoint to get verify bill payment")]
        public async Task<IActionResult> PaymentVerification([FromRoute] string billPaymentCode)
		{
			var response = await _revpay.PaymentVerification(billPaymentCode);
			return Ok(response);
		}
	}
}
