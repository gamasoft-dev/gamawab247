using BillProcessorAPI.Dtos;
using BillProcessorAPI.Services.Implementations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BillProcessorAPI.Controllers
{
    [ApiController]
    [Route("v1/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionService _transactionService;
        public TransactionsController(
            TransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("verify")]
        [ProducesResponseType(typeof(TransactionVerificationResponseDto), 200)]
        [SwaggerOperation(Summary = "Endpoint to verify transactions")]
        public async Task<IActionResult> VerifyTransaction(TransactionVerificationInputDto input)
        {
            var response = await _transactionService.VerifyBillTransactionAsync(input);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(object), 200)]
        [SwaggerOperation(Summary = "Endpoint to add transaction details into the db")]
        public async Task<IActionResult> CreateUserBillTransaction(CreateUserBillTransactionInputDto input)
        {
            var response = await _transactionService.CreateUserBillTransaction(input);
            return Ok(response);
        }
    }
}
