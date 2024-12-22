using System.Diagnostics;

namespace StreamMaster.Streams.Streams;

public class CustomPlayListStream(ILogger<CustomPlayListStream> logger, ICommandExecutor commandExecutor) : ICustomPlayListStream
{
    public const string CustomPlayListFFMpegOptions = "-map 0:v -map 0:a? -map 0:s? -c copy";
    public async Task<GetStreamResult> HandleStream(SMStreamInfo smStreamInfo, string clientUserAgent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting custom stream for {streamName}", smStreamInfo.Name);
        Stopwatch stopwatch = Stopwatch.StartNew();

        GetStreamResult result = commandExecutor.ExecuteCommand(smStreamInfo.CommandProfile, smStreamInfo.Url, clientUserAgent, smStreamInfo.SecondsIn, cancellationToken);
        stopwatch.Stop();
        logger.LogInformation("Got custom stream for {streamName} in {ElapsedMilliseconds} ms", smStreamInfo.Name, stopwatch.ElapsedMilliseconds);

        return await Task.FromResult(result);
    }
}
