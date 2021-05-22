using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Basket.API.Infrastructure.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> logger;
        private readonly IWebHostEnvironment env;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IWebHostEnvironment env)
        {
            this.logger = logger;
            this.env = env;
        }
        public void OnException(ExceptionContext context)
        {
            Exception exception = context.Exception;
            logger.LogError(new EventId(exception.HResult), exception, exception.Message);

            if (exception is BasketDomainException)
            {

                var problemDetails = new ValidationProblemDetails()
                {
                    Instance = context.HttpContext.Request.Path,
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Please refer to the errors property for additional details."
                };

                problemDetails.Errors.Add("DomainValidations", new string[] { context.Exception.Message.ToString() });

                context.Result = new BadRequestObjectResult(problemDetails);
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            else
            {
                var errorDetails = new
                {
                    Messages = "Error occured",
                    DeveloperMessage = env.IsDevelopment() ? context.Exception : default
                };
                context.Result = new BadRequestObjectResult(errorDetails);
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }

            context.ExceptionHandled = true;
        }
    }
}
