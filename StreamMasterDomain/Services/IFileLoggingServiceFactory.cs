namespace StreamMasterDomain.Services;

public interface IFileLoggingServiceFactory
{
    IFileLoggingService Create(string key);
}
