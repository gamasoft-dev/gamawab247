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
        private readonly IFlutterwaveService _transactionService;
        private readonly ILogger<PaymentController> _logger;
        public PaymentController(IPayThruService paythruService,
            IFlutterwaveService transactionService,
            ILogger<PaymentController> logger)
        {
            _paythruService = paythruService;
            _transactionService = transactionService;
            _logger = logger;
        }
    }
}
