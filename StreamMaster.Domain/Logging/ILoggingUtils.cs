namespace StreamMaster.Domain.Logging
{
    public interface ILoggingUtils
    {
        string GetLoggableURL(string sourceUrl);
        Task<string> GetLoggableURLAsync(string sourceUrl);
    }
}