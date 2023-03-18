using BillProcessorAPI.Dtos;
using BillProcessorAPI.Dtos.Paythru;
using BillProcessorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BillProcessorAPI.Controllers
{

    [ApiController]
	[ApiVersion("1.0")]
	[Route("v1/paythru")]
	[Produces("application/json")]
	public class PaythruPaymentController : ControllerBase
	{
		private readonly IPayThruService _paythruService;
		public PaythruPaymentController(IPayThruService paythruService)
		{
            _paythruService = paythruService;
		}

		[HttpPost]
        [ProducesResponseType(typeof(PayThruPaymentRequestDto), 200)]
        [SwaggerOperation(Summary = "Endpoint to create a payment transaction with paythru")]
        public async Task<IActionResult> CreateTransaction(int amount, string billCode)
		{
			var response = await _paythruService.CreatePayment(amount,billCode);
			return Ok(response);
		}

		[HttpGet("payment-verification")]
		[ProducesResponseType(typeof(PaymentVerificationResponseDto), 200)]
		[SwaggerOperation(Summary = "Endpoint to verify paythru payment")]
		public async Task<IActionResult> PaymentVerification([FromRoute] NotificationRequestWrapper model)
		{
			var response = await _paythruService.VerifyPayment(model);
			return Ok(response);
		}
	}
}
