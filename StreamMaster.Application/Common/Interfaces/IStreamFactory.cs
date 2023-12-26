namespace StreamMaster.Application.Common.Interfaces;

public interface IStreamFactory
{
    Task<Stream> CreateStreamAsync(string streamUrl);
}
