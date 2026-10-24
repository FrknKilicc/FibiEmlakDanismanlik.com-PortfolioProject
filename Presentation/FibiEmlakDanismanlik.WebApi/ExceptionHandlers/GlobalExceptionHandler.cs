using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.WebApi.ExceptionHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (httpContext.Response.HasStarted)
            {
                return false;
            }

            if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
            {
                return false;
            }

            ProblemDetails problemDetails;

            if (exception is KeyNotFoundException)
            {
                _logger.LogWarning("Kayıt bulunamadı: {Path}", httpContext.Request.Path);

                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Kayıt bulunamadı"
                };
            }
            else
            {
                _logger.LogError(exception, "Beklenmeyen bir hata oluştu: {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Beklenmeyen bir hata oluştu",
                    Detail = "İşlem sırasında bir sorun oluştu. Lütfen daha sonra tekrar deneyin."
                };
                problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
            }

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}
