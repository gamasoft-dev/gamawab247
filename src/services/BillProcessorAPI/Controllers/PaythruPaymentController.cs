using BillProcessorAPI.Dtos;
using BillProcessorAPI.Dtos.Common;
using BillProcessorAPI.Dtos.Paythru;
using BillProcessorAPI.Helpers;
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
        [ProducesResponseType(typeof(PaymentCreationResponse), 200)]
        [SwaggerOperation(Summary = "Endpoint to create a payment transaction with paythru")]
        public async Task<IActionResult> CreateTransaction(int amount, string billCode)
		{
			var response = await _paythruService.CreatePayment(amount,billCode);
			return Ok(response);
		}

		/// <summary>
		/// This is a webhook for payment notification by Paythru
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost("/paythru/notify")]
		[ProducesResponseType(typeof(PaymentVerificationResponseDto), 200)]
		[SwaggerOperation(Summary = "Endpoint to verify paythru payment")]
		public async Task<IActionResult> PaymentVerification([FromBody] NotificationRequestWrapper model)
		{
			try
			{
                var response = await _paythruService.VerifyPayment(model);
            }
			catch (PaymentVerificationException ex)
			{
				_logger.LogError(ex.ErrorMessage);
				return Ok($"Payement verification didnot complete succesfully but notification was recieved {ex.ToString()}");
			}
			return Ok();
		}

        [HttpPost("/paythru/payment-confirmation")]
        [ProducesResponseType(typeof(PaymentConfirmationResponse), 200)]
        [SwaggerOperation(Summary = "Paythru redirect endpoint")]
        public async Task<IActionResult> PaymentConfirmation([FromBody] ConfirmPaymentRequest model)
        {
            var response = await _paythruService.ConfirmPayment(model);
            return Ok(response);
        }
    }
}
