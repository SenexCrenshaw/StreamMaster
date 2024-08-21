namespace StreamMaster.Streams.Streams;

public class CustomPlayListStream(ILogger<CustomPlayListStream> logger, ICommandExecutor commandExecutor) : ICustomPlayListStream
{
    public const string CustomPlayListFFMpegOptions = "-map 0:v -map 0:a? -map 0:s? -c copy";
    public async Task<(Stream? stream, int processId, ProxyStreamError? error)> HandleStream(SMStreamInfo smStreamInfo, string clientUserAgent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Opened custom stream for {streamName}", smStreamInfo.Name);
        (Stream? stream, int processId, ProxyStreamError? error) result = commandExecutor.ExecuteCommand(smStreamInfo.CommandProfile, smStreamInfo.Url, clientUserAgent, smStreamInfo.SecondsIn, cancellationToken);
        return await Task.FromResult(result);
    }
}
