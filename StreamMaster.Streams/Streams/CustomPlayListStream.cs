using System.Diagnostics;

namespace StreamMaster.Streams.Streams;

public class CustomPlayListStream(ILogger<CustomPlayListStream> logger, IProfileService profileService, ICommandExecutor commandExecutor) : ICustomPlayListStream
{
    public const string CustomPlayListFFMpegOptions = "-map 0:v -map 0:a? -map 0:s? -c copy";
    public async Task<(Stream? stream, int processId, ProxyStreamError? error)> HandleStream(SMStreamInfo smStreamInfo, string clientUserAgent, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        //CommandProfileDto profile = profileService.GetCommandProfile("SMFFMPEG");



        stopwatch.Stop();
        logger.LogInformation("Opened custom stream for {streamName} in {ElapsedMilliseconds} ms", smStreamInfo.Name, stopwatch.ElapsedMilliseconds);
        return commandExecutor.ExecuteCommand(smStreamInfo.CommandProfile, smStreamInfo.Url, clientUserAgent, smStreamInfo.SecondsIn, cancellationToken);
    }
}
