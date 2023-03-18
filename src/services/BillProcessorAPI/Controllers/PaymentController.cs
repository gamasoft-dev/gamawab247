using BillProcessorAPI.Services.Implementations;
using BillProcessorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillProcessorAPI.Controllers
{
    [ApiController]
    [Route("v1/payment")]
    [Produces("application/json")]
    public partial class PaymentController : ControllerBase
    {
        private readonly IPayThruService _paythruService;
        private readonly ITransactionService _transactionService;
        public PaymentController(IPayThruService paythruService, ITransactionService transactionService)
        {
            _paythruService = paythruService;
            _transactionService = transactionService;
        }
    }
}
