namespace StreamMaster.Streams.Domain.Interfaces;

public interface IStreamFactory
{
    Task<Stream> CreateStreamAsync(string streamUrl);
}
