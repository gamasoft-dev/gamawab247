using BillProcessorAPI.Dtos;
using BillProcessorAPI.Dtos.Paythru;
using BillProcessorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BillProcessorAPI.Controllers
{

	public partial class PaymentController : ControllerBase
	{

        [HttpPost("/paythru")]
        [ProducesResponseType(typeof(PaythruPaymentResponseDto), 200)]
        [SwaggerOperation(Summary = "Endpoint to create a payment transaction with paythru")]
        public async Task<IActionResult> CreateTransaction(int amount, string billCode)
		{
			var response = await _paythruService.CreatePayment(amount,billCode);
			return Ok(response);
		}

		[HttpPost("/paythru/notify")]
		[ProducesResponseType(typeof(PaymentVerificationResponseDto), 200)]
		[SwaggerOperation(Summary = "Endpoint to verify paythru payment")]
		public async Task<IActionResult> PaymentVerification([FromBody] NotificationRequestWrapper model)
		{
			var response = await _paythruService.VerifyPayment(model);
			return Ok(response);
		}
	}
}
