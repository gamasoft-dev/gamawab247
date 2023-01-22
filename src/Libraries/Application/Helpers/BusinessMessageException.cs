using System;

namespace Application.Helpers;

public class BusinessMessageException: Exception
{
    public string ErrorMessage { get; set; }
    public object Errors { get; }

    public BusinessMessageException(string errorMessage, object errors)
    {
        ErrorMessage = errorMessage;
        Errors = errors;
    }
}