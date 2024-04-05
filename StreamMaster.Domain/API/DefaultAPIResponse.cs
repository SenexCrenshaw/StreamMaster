using Reinforced.Typings.Attributes;

namespace StreamMaster.Domain.API
{
    [TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
    public class DefaultAPIResponse
    {
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsError { get; set; }


        public static DefaultAPIResponse Error<T>(string errorMessage)
        {
            return new DefaultAPIResponse { ErrorMessage = errorMessage, IsError = true };
        }

        public static DefaultAPIResponse Success => new();

        public static DefaultAPIResponse ErrorWithMessage(Exception exception, string message)
        {
            DefaultAPIResponse ok = new()
            {
                IsError = true,
                ErrorMessage = $"{message} : {exception}"
            };
            return ok;
        }


        public static DefaultAPIResponse ErrorWithMessage(string message)
        {
            DefaultAPIResponse ok = new()
            {
                IsError = true,
                ErrorMessage = message
            };
            return ok;
        }

        public static DefaultAPIResponse OkWithMessage(string message)
        {
            DefaultAPIResponse ok = Success;
            ok.Message = message;
            return ok;
        }

        public static DefaultAPIResponse Ok => new();

        public static DefaultAPIResponse NotFound =>
         new()
         {
             IsError = true,
             ErrorMessage = "Not Found"
         };
    }
}
