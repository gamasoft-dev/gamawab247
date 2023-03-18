using BillProcessorAPI.Dtos;
using BillProcessorAPI.Services.Implementations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BillProcessorAPI.Controllers
{
    public partial class PaymentController : ControllerBase
    {
        //private readonly TransactionService _transactionService;
        //public PaymentController(TransactionService transactionService, Services.Interfaces.IPayThruService paythruService)
        //{
        //    _transactionService = transactionService;
        //    this._paythruService = paythruService;
        //}

        [HttpPost("/flutterwave")]
        [ProducesResponseType(typeof(object), 200)]
        [SwaggerOperation(Summary = "Endpoint to add transaction details into the db")]
        public async Task<IActionResult> CreateFlutterwavePayment(CreateUserBillTransactionInputDto input)
        {
            var response = await _transactionService.CreateUserBillTransaction(input);
            return Ok(response);
        }

        [HttpPost("/flutterwave/verify")]
        [ProducesResponseType(typeof(TransactionVerificationResponseDto), 200)]
        [SwaggerOperation(Summary = "Endpoint to verify transactions")]
        public async Task<IActionResult> VerifyFlutterwavePayment(TransactionVerificationInputDto input)
        {
            var response = await _transactionService.VerifyBillTransactionAsync(input);
            return Ok(response);
        }
    }
}
