namespace ExceptionHandling.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string errorCode = "NotFound") : base(errorCode) { }
    }
}
