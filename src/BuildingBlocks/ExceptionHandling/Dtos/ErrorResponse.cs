namespace ExceptionHandling.Dtos
{
    public class ErrorResponse
    {
        public int StatusCode { get; }
        public string Message { get; }
        public string? Details { get; }

        public ErrorResponse(int statusCode, string message, string? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }
    }
}
