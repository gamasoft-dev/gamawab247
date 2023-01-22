using System;

namespace Application.Helpers;

public class BackgroundJobException : Exception
{
    private string ErrorMessage { get; set; }
    private object Errors { get; }

    public BackgroundJobException(string errorMessage, string source, Object errors)
    {
        ErrorMessage = errorMessage;
        Errors = errors ?? base.InnerException;
        base.Source = source;
    }
    public BackgroundJobException(string errorMessage, string source)
    {
        ErrorMessage = errorMessage;
        Errors = base.InnerException;
        base.Source = source;
    }
}