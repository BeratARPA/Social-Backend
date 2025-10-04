namespace ExceptionHandling.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string errorCode = "Unauthorized") : base(errorCode) { }
    }
}
