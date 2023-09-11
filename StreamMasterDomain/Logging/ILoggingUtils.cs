namespace StreamMasterDomain.Logging
{
    public interface ILoggingUtils
    {
        string GetLoggableURL(string sourceUrl);
        Task<string> GetLoggableURLAsync(string sourceUrl);
    }
}