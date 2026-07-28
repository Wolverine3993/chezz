using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Chezz.Errors
{
	public sealed class AppExceptionHandler(
		IProblemDetailsService problemDetailsService,
		ILogger<AppExceptionHandler> logger) : IExceptionHandler
	{
		public async ValueTask<bool> TryHandleAsync(
			HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
		{
			var status = exception is ChezzError app
				? app.StatusCode
				: StatusCodes.Status500InternalServerError;

			if (status >= StatusCodes.Status500InternalServerError)
			{
				logger.LogError(exception, "Unhandled exception processing {Path}", httpContext.Request.Path);
			}

			httpContext.Response.StatusCode = status;

			return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
			{
				HttpContext = httpContext,
				Exception = exception,
				ProblemDetails = new ProblemDetails
				{
					Status = status,
					Title = exception is ChezzError ? exception.Message : "An unexpected error occurred.",
				}
			});
		}
	}
}
