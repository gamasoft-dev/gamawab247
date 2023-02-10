using BillProcessorAPI.Dtos;
using BillProcessorAPI.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> VerifyTransaction(TransactionVerificationInputDto input)
        {
            var response = await _transactionService.VerifyBillTransactionAsync(input);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> CreateUserBillTransaction(CreateUserBillTransactionInputDto input)
        {
            var response = await _transactionService.CreateUserBillTransaction(input);
            return Ok(response);
        }

        [HttpPost("calculate-charge")]
        [ProducesResponseType(typeof(ChargesResponseDto), 200)]
        public IActionResult CalculateBillChargesOnAmount(ChargesInputDto input)
        {
            var response = _transactionService.CalculateBillChargesOnAmount(input);
            return Ok(response);
        }

        [HttpPost("charges")]
        [ProducesResponseType(typeof(ChargesResponseDto), 200)]
        public async Task<IActionResult> CreateBillCharges(CreateBillChargeInputDto input)
        {
            var response = await _transactionService.CreateBillCharges(input);
            return Ok(response);
        }

        [HttpGet("charges")]
        [ProducesResponseType(typeof(IEnumerable<ChargesResponseDto>), 200)]
        public async Task<IActionResult> GetBillCharges()
        {
            var response = await _transactionService.GetBillCharges();
            return Ok(response);
        }
    }
}
