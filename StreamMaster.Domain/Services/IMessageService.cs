namespace StreamMaster.Domain.Services
{
    public interface IMessageService
    {
        Task SendError(string message, Exception? ex);
        Task SendError(string message, string? header = null);
        Task SendInfo(string message, string? header = null);
        Task SendMessage(SMMessage smMessage);
        Task SendWarn(string message);
        Task SendSuccess(string message, string? header = null);
    }
}