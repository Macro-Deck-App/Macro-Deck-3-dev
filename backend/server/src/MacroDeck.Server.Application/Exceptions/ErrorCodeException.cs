using MacroDeck.Server.Domain.Enums;

namespace MacroDeck.Server.Application.Exceptions;

public class ErrorCodeException : Exception
{

	public ErrorCodeException(ErrorCode errorCode)
	{
		ErrorCode = errorCode;
	}

	public ErrorCode ErrorCode { get; }
}
