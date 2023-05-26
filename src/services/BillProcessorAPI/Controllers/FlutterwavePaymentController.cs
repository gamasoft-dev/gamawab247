using BillProcessorAPI.Dtos;
using BillProcessorAPI.Dtos.Common;
using BillProcessorAPI.Dtos.Flutterwave;
using BillProcessorAPI.Helpers;
using BillProcessorAPI.Services.Implementations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BillProcessorAPI.Controllers
{
    public partial class PaymentController : ControllerBase
    {

        [HttpPost("/flutterwave")]
        [ProducesResponseType(typeof(PaymentCreationResponse), 200)]
        [SwaggerOperation(Summary = "Endpoint to create transaction")]
        public async Task<IActionResult> CreateFlutterwavePayment(string email,  decimal amount, string billPaymentCode)
        {
            var response = await _transactionService.CreateTransaction(email, amount, billPaymentCode);
            return Ok(response);
        }


        /// <summary>
        /// Webhook notification to verify and process payment request.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("/flutterwave/notify")]
        [ProducesResponseType(typeof(TransactionVerificationResponseDto), 200)]
        [SwaggerOperation(Summary = "Webhook endpoint")]
        public async Task<IActionResult> FlutterwavePaymentNotification([FromBody] WebHookNotificationWrapper model)
        {
           
            try
            {
                _logger.LogInformation("Flutter wave request received");

                var response = await _transactionService.PaymentNotification(model);
                return Ok(response);
            }
            catch (PaymentVerificationException ex)
            {
                _logger.LogError(ex.ErrorMessage);
                return Ok($"Payement verification didnot complete succesfully but notification was recieved {ex.ToString()}");

            }
        }

        /// <summary>
        /// Test endpoint
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("/flutterwave/notify2")]
        [ProducesResponseType(typeof(TransactionVerificationResponseDto), 200)]
        [SwaggerOperation(Summary = "Test webhook endpoint")]
        public async Task<IActionResult> FlutterwavePaymentNotification2([FromBody] WebHookNotificationWrapper model)
        {

            try
            {
                _logger.LogInformation("Flutter wave request received");

                var response = await _transactionService.PaymentNotification(model);
                return Ok(response);
            }
            catch (PaymentVerificationException ex)
            {
                _logger.LogError(ex.ErrorMessage);
                return Ok($"Payement verification didnot complete succesfully but notification was recieved {ex.ToString()}");

            }
        }

        [HttpGet("/flutterwave/verify/{tx_ref}")]
        [ProducesResponseType(typeof(object), 200)]
        [SwaggerOperation(Summary = "Endpoint for transaction verification")]
        public async Task<IActionResult> VerifyPayment([FromRoute] string tx_ref)
        {
            try
            {
                var response = await _transactionService.VerifyTransaction(tx_ref);
                return Ok(response);
            }
            catch (PaymentVerificationException ex)
            {
                _logger.LogError(ex.ErrorMessage);
                return Ok($"Payement verification didnot complete succesfully but notification was recieved {ex.ToString()}");
            }
            
        }

        [HttpGet("/flutterwave/payment-confirmation/{status}/{tx_ref}/{transaction_id}")]
        [ProducesResponseType(typeof(PaymentConfirmationResponse), 200)]
        [SwaggerOperation(Summary = "Endpoint for redirect url")]
        public async Task<IActionResult> PaymentConfirmation([FromRoute] string status, string tx_ref, string transaction_id)
        {
            var response = await _transactionService.PaymentConfirmation(status,tx_ref,transaction_id);
            return Ok(response);
        }
       
    }
}
