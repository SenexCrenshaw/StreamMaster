namespace StreamMaster.Domain.Services
{
    public interface IMessageSevice
    {
        Task SendError(string message, string? details = null);
        Task SendSMInfo(string message);
        Task SendSMMessage(SMMessage smMessage);
        Task SendSMWarn(string message);
        Task SendSuccess(string message);
    }
}