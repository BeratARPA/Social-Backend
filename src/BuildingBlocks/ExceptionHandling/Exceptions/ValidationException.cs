namespace ExceptionHandling.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(string errorCode = "ValidationFailed") : base(errorCode) { }
    }
}
