using MessagePack;

using Reinforced.Typings.Attributes;

namespace StreamMaster.Domain.API
{
    [TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
    public class APIResponse
    {
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsError { get; set; }

        public static APIResponse ErrorWithMessage(Exception exception, string message, string? errorMessage)
        {
            return new APIResponse { ErrorMessage = errorMessage, IsError = true };
        }

        public static APIResponse ErrorWithMessage(string? errorMessage)
        {
            return new APIResponse { ErrorMessage = errorMessage, IsError = true };
        }

        [IgnoreMember]
        [TsIgnore]
        public static APIResponse Error => new() { IsError = true };

        [IgnoreMember]
        [TsIgnore]
        public static APIResponse Success => new();

        public static APIResponse ErrorWithMessage(Exception exception, string message)
        {
            APIResponse ok = new()
            {
                IsError = true,
                ErrorMessage = $"{message} : {exception}"
            };
            return ok;
        }


        public static APIResponse OkWithMessage(string message)
        {
            APIResponse ok = Success;
            ok.Message = message;
            return ok;
        }

        [IgnoreMember]
        [TsIgnore]
        public static APIResponse Ok => new();

        public static APIResponse NotFound =>
         new()
         {
             IsError = true,
             ErrorMessage = "Not Found"
         };
    }
}
