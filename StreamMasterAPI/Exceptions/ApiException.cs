using System.Net;

namespace StreamMasterAPI.Exceptions
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
    }

    public abstract class ApiException : Exception
    {
        public object? Content { get; private set; }

        public HttpStatusCode StatusCode { get; private set; }

        protected ApiException(HttpStatusCode statusCode, object? content = null)
            : base(GetMessage(statusCode, content))
        {
            StatusCode = statusCode;
            Content = content;
        }

        private static string GetMessage(HttpStatusCode statusCode, object? content)
        {
            var result = statusCode.ToString();

            if (content != null)
            {
                result = $"{result}: {content}";
            }

            return result;
        }
    }
}
