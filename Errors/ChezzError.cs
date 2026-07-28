namespace Chezz.Errors
{
	public class ChezzError(int statusCode, string message) : Exception(message)
	{
		public int StatusCode { get; } = statusCode;
	}

	public sealed class NotFoundException(string message)
		: ChezzError(StatusCodes.Status404NotFound, message);

	public sealed class BadRequestException(string message)
		: ChezzError(StatusCodes.Status400BadRequest, message);

	public sealed class ConflictException(string message)
		: ChezzError(StatusCodes.Status409Conflict, message);

	public sealed class ForbiddenException(string message)
		: ChezzError(StatusCodes.Status403Forbidden, message);

	public sealed class InternalServerError(string message)
		: ChezzError(StatusCodes.Status500InternalServerError, message);
}
