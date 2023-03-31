using System;
using System.Net;

namespace BillProcessorAPI.Helpers
{
    public class PaymentVerificationException : Exception
    {
        public string ErrorMessage { get; set; }
        public HttpStatusCode Code { get; }
        public object Errors { get; }
        /// <summary>
        /// Helps set the rest exception error message as the message value for the exception base class.
        /// </summary>
		public override string Message { get { return !string.IsNullOrEmpty(ErrorMessage) ? ErrorMessage : base.Message; } }

        public PaymentVerificationException(HttpStatusCode code, string message, object errors = null)
        {
            ErrorMessage = message;
            Code = code;
            Errors = errors;
        }
    }
}
