namespace ExceptionHandling.Dtos
{
    public class ErrorResponse
    {
        public int StatusCode { get; init; }
        public string ErrorCode { get; init; }
        public string? Details { get; init; }

        public ErrorResponse(int statusCode, string errorCode, string? details = null)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Details = details;
        }
    }
}
