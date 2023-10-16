using StreamMasterApplication.Common.Interfaces;

namespace StreamMasterInfrastructure.VideoStreamManager.Factories;

public class DefaultStreamFactory : IStreamFactory
{
    public async Task<Stream> CreateStreamAsync(string streamUrl)
    {

        return await Task.FromResult(new MemoryStream()); // Dummy implementation
    }
}
