using System.Net;
using Application.Exceptions;
using Application.Helpers;
using Domain.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BillProcessorAPI.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, _logger);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger<ErrorHandlingMiddleware> logger)
        {
            object errors = null;
            var message = string.Empty;
            switch (ex)
            {
                case RestException re:
                    logger.LogError(ex, "Rest Error");
                    message = re.ErrorMessage;
                    errors = re.Errors;
                    context.Response.StatusCode = (int)re.Code;
                    break;
                
                case ArgumentException argEx:
                    logger.LogError(argEx, "Argument Exception");
                    message = argEx.Message;
                    errors = argEx.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                
                case InvalidOperationException opExt:
                    logger.LogError(opExt, "Operation not valid Exception");
                    message = opExt.Message;
                    errors = opExt.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    break;

                case BadRequestException badRequest:
                    logger.LogError(ex, "Bad Request Exception");
                    message = badRequest.Message;
                    errors = badRequest.InnerException;
                    context.Response.StatusCode = badRequest.StatusCode;
                    break;
                
                case NotFoundException notFoundException:
                    logger.LogError(ex, "Bad Request Exception");
                    message = notFoundException.Message;
                    errors = notFoundException.InnerException;
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                case InternalServerException internalServerError:
                    logger.LogError(ex, "Internal server Exception");
                    message = "Error whilst processing this request, please contact the admin";
                        //internalServerError.Message;
                    errors = internalServerError.InnerException;
                    context.Response.StatusCode = internalServerError.StatusCode;
                    break;
                
                case AuthenticationException authenticationException:
                    logger.LogError(ex, "Authentication Exception");
                    message = !string.IsNullOrEmpty(authenticationException.Message)? authenticationException.Message : "You are not authenticated to perform this operation, kindly login";
                    //internalServerError.Message;
                    errors = authenticationException.InnerException;
                    context.Response.StatusCode = (int)authenticationException.StatusCode;
                    break;
                    
                case HttpException httpException:
                    logger.LogError(ex, "Http Exception");
                    message = "An error occurred whilst processing this request.";
                    //internalServerError.Message;
                    errors = httpException.InnerException;
                    context.Response.StatusCode = httpException.StatusCode;
                    break;
                
                case Exception e:
                    logger.LogError(ex, "Server Error");
                    message = e.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            var response = new ErrorResponse<object>
            {
                Message = message,
                Error = errors
            };
            
            context.Response.ContentType = "application/json";
            
            var result = JsonConvert.SerializeObject(response, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });
            await context.Response.WriteAsync(result);
            
        }
    }
    
    public static class ErrorHandlingMiddlewareExtension
    {
        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
