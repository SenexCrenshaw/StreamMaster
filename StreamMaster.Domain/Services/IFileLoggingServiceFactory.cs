namespace StreamMaster.Domain.Services;

public interface IFileLoggingServiceFactory
{
    IFileLoggingService Create(string key);
}
