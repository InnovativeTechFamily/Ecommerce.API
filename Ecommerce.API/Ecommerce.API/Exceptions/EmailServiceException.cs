namespace Ecommerce.API.Exceptions
{
    public class EmailServiceException : Exception
    {
        public EmailServiceException(string message) : base(message)
        {
        }

        public EmailServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
