using System;
namespace Domain.Exceptions
{
    public class BadRequestException : HttpException
    {
        public BadRequestException(String message, Exception innerException = null) : base(message, innerException)
        {
            if (String.IsNullOrEmpty(message))
                message = "An error occurred whilst processing this request, Verify request data and try again";

            base.StatusCode = 400;
        }

        public BadRequestException(String message, int statusErrorCode, Exception innerException = null) : base(message, innerException)
        {
            if (String.IsNullOrEmpty(message))
                message = "An error occurred whilst processing this request, Verify request data and try again";

            if (statusErrorCode == 0)
                this.StatusCode = 400;

        }
    }
}

