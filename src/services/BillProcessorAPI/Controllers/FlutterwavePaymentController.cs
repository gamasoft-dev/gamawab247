using System.Net;
using AsyncAwaitBestPractices;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Dtos.Common;
using BillProcessorAPI.Dtos.Flutterwave;
using BillProcessorAPI.Helpers;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BillProcessorAPI.Controllers
{
    public partial class PaymentController : ControllerBase
    {

        [HttpPost("/flutterwave")]
        [ProducesResponseType(typeof(PaymentCreationResponse), 200)]
        [SwaggerOperation(Summary = "Endpoint to create transaction")]
        public async Task<IActionResult> CreateFlutterwavePayment(string email,  
            decimal amount, string billPaymentCode, string phoneNumber)
        {
            throw new RestException(HttpStatusCode.ExpectationFailed, "Payment service temporarily unavailable");
            // var response = await _flutterwaveMgtService.CreateTransaction(email, amount, billPaymentCode, phoneNumber);
            // return Ok(response);
        }


        /// <summary>
        /// Webhook notification to verify and process payment request.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("/flutterwave/notify")]
        [ProducesResponseType(typeof(TransactionVerificationResponseDto), 200)]
        [SwaggerOperation(Summary = "Webhook endpoint")]
        public IActionResult FlutterwavePaymentNotification([FromBody] WebHookNotificationWrapper model)
        {
            _logger.LogInformation("Flutter wave request received");
                
            _flutterwaveMgtService.PaymentNotification(model).SafeFireAndForget(exception =>
            {
                _logger.LogError($"An error occurred on receipt of flutter wave payment notification {exception}");
            }, continueOnCapturedContext: false);

            return Ok("Transaction Status received");
        }
        
        [HttpPost("/flutterwave/resend-webhook")]
        [ProducesResponseType(typeof(FailedWebhookResponseModel), 200)]
        [SwaggerOperation(Summary = "Endpoint for resending failed webhooks")]
        public async Task<IActionResult> ResendWebhook([FromBody] FailedWebhookRequest model)
        {
            var response = await _flutterwaveMgtService.ResendWebhook(model);
            return Ok(response);
        }

        [HttpGet("/flutterwave/verify/{tranRef}")]
        [ProducesResponseType(typeof(object), 200)]
        [SwaggerOperation(Summary = "Endpoint for transaction verification")]
        public async Task<IActionResult> VerifyPayment([FromRoute] string tranRef)
        {
            try
            {
                var k = "".Substring(1, 3);
                var response = await _flutterwaveMgtService.VerifyTransaction(tranRef);
                return Ok(response);
            }
            catch (PaymentVerificationException ex)
            {
                _logger.LogError(ex.ErrorMessage);
                return Ok($"Payment verification did not complete successfully but notification was received {ex}");
            }
        }

        [HttpGet("/flutterwave/payment-confirmation/{status}/{tx_ref}/{transaction_id}")]
        [ProducesResponseType(typeof(PaymentConfirmationResponse), 200)]
        [SwaggerOperation(Summary = "Endpoint for redirect url")]
        public async Task<IActionResult> PaymentConfirmation([FromRoute] string status, string tx_ref, string transaction_id)
        {
            await Task.Delay(paymentConfirmationDelayInSec.Flutterwave);
            
            var response = await _flutterwaveMgtService.PaymentConfirmation(status, tx_ref, transaction_id);
            return Ok(response);
        }
    }
}
