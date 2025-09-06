using MacroDeck.Server.Application.Exceptions;
using MacroDeck.Server.Application.Extensions;
using MacroDeck.Server.Domain.Enums;
using MacroDeck.Server.Models.Common;
using Serilog;
using ILogger = Serilog.ILogger;

namespace MacroDeck.Server.Middlewares;

public class ErrorHandlingMiddleware
{
	private readonly ILogger _logger = Log.ForContext<ErrorHandlingMiddleware>();

	private readonly RequestDelegate _next;

	public ErrorHandlingMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			await HandleException(context, ex);
		}
	}

	private async Task HandleException(HttpContext context, Exception exception)
	{
		if (exception is UnauthorizedAccessException)
		{
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			return;
		}

		if (exception is not ErrorCodeException errorCodeException)
		{
			_logger.Fatal(exception, "Unhandled error on request {Request}", context.Request.Path);
			errorCodeException = new ErrorCodeException(ErrorCode.Unknown);
		}

		var description = errorCodeException.ErrorCode.GetDescription();

		var errorMessage = new ErrorCodeExceptionModel
						   {
							   Message = description,
							   Code = (int)errorCodeException.ErrorCode
						   };

		var statusCode = errorCodeException.ErrorCode switch
						 {
							 ErrorCode.Unknown => StatusCodes.Status500InternalServerError,
							 ErrorCode.NotFound => StatusCodes.Status404NotFound,
							 _ => StatusCodes.Status400BadRequest
						 };

		context.Response.StatusCode = statusCode;
		await context.Response.WriteAsJsonAsync(errorMessage);
	}
}
