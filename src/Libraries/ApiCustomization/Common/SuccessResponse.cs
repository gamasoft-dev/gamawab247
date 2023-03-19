using System;
namespace ApiCustomization.Common
{
    internal class CustomizationSuccessResponse<T> : BaseResponse
    {
        internal CustomizationSuccessResponse()
        {
            Success = true;
        }
        public T Data { get; set; }
    }

    internal class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}

