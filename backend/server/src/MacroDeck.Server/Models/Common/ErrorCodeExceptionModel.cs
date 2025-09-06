namespace MacroDeck.Server.Models.Common;

public class ErrorCodeExceptionModel
{
	public required string Message { get; set; }

	public required int Code { get; set; }
}
