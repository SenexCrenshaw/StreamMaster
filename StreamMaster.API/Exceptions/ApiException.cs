using System.Net;

namespace StreamMaster.API.Exceptions
{
    public abstract class NzbDroneException : ApplicationException
    {
        protected NzbDroneException(string message, params object[] args)
            : base(string.Format(message, args))
        {
        }

        protected NzbDroneException(string message)
            : base(message)
        {
        }

        protected NzbDroneException(string message, Exception innerException, params object[] args)
            : base(string.Format(message, args), innerException)
        {
        }

        protected NzbDroneException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NzbDroneException() : base()
        {
        }
    }

    public abstract class ApiException : Exception
    {
        public object? Content { get; }

        public HttpStatusCode StatusCode { get; }

        protected ApiException(HttpStatusCode statusCode, object? content = null)
            : base(GetMessage(statusCode, content))
        {
            StatusCode = statusCode;
            Content = content;
        }

        protected ApiException() : base()
        {
        }

        protected ApiException(string? message) : base(message)
        {
        }

        protected ApiException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        private static string GetMessage(HttpStatusCode statusCode, object? content)
        {
            string result = statusCode.ToString();

            if (content != null)
            {
                result = $"{result}: {content}";
            }

            return result;
        }
    }
}
