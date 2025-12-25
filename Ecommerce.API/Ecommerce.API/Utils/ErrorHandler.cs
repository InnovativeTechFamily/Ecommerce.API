namespace Ecommerce.API.Utils
{
    public class ErrorHandler : Exception
    {
        public int StatusCode { get; set; }

        public ErrorHandler(string message, int statusCode = 500) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
