using System.Diagnostics;
using System.Text;

namespace StreamMaster.Streams.Streams;

/// <summary>
/// Handles the creation and management of multi-view playlist streams.
/// </summary>
public class MultiViewPlayListStream(ILogger<MultiViewPlayListStream> logger, ICacheManager cacheManager) : IMultiViewPlayListStream
{
    /// <inheritdoc/>
    public Task<GetStreamResult> HandleStream(IChannelBroadcaster channelBroadcaster, CancellationToken cancellationToken)
    {
        SMStreamInfo? smStreamInfo = channelBroadcaster.SMStreamInfo;
        if (smStreamInfo == null)
        {
            return Task.FromResult(new GetStreamResult(null, -1, new ProxyStreamError { Message = "SMStreamInfo is null" }));
        }

        logger.LogInformation("Getting MultiView stream for {StreamName}", channelBroadcaster.SourceName);

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            StringBuilder args = BuildArguments(channelBroadcaster);
            const string command = "ffmpeg";
            string? exec = FileUtil.GetExec(command);

            if (exec == null)
            {
                logger.LogCritical("Command \"{Command}\" not found", command);
                return Task.FromResult(new GetStreamResult(null, -1, new ProxyStreamError
                {
                    ErrorCode = ProxyStreamErrorCode.FileNotFound,
                    Message = $"{command} not found"
                }));
            }

            using Process process = new();
            ConfigureProcess(process, exec, args.ToString());

            cancellationToken.ThrowIfCancellationRequested();

            bool processStarted = process.Start();
            if (!processStarted)
            {
                logger.LogError("Failed to start process");
                return Task.FromResult(new GetStreamResult(null, -1, new ProxyStreamError
                {
                    ErrorCode = ProxyStreamErrorCode.ProcessStartFailed,
                    Message = "Failed to start process"
                }));
            }

            stopwatch.Stop();

            logger.LogInformation("Opened command with args \"{Options}\" in {ElapsedMilliseconds} ms",
                args, stopwatch.ElapsedMilliseconds);

            return Task.FromResult(new GetStreamResult(process.StandardOutput.BaseStream, process.Id, null));
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Operation canceled for MultiView stream: {StreamName}", channelBroadcaster.SourceName);
            return Task.FromResult(new GetStreamResult(null, -1, new ProxyStreamError
            {
                ErrorCode = ProxyStreamErrorCode.OperationCancelled,
                Message = "Operation canceled"
            }));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred while handling MultiView stream for {StreamName}", channelBroadcaster.SourceName);
            return Task.FromResult(new GetStreamResult(null, -1, new ProxyStreamError { Message = ex.Message }));
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    private StringBuilder BuildArguments(IChannelBroadcaster channelBroadcaster)
    {
        StringBuilder args = new();
        args.Append("-hide_banner -loglevel error ");

        foreach (SMChannelDto channel in channelBroadcaster.SMChannel.SMChannelDtos)
        {
            args.Append($"-i \"http://localhost:{BuildInfo.DEFAULT_PORT}/ v/{cacheManager.DefaultSG!.Id}/{channel.Id}\" ");
        }

        const string filterComplex = "[0:v]scale=960:540[top_left]; [1:v]scale=960:540[top_right]; [2:v]scale=960:540[bottom_left]; [3:v]scale=960:540[bottom_right]; [top_left][top_right]hstack=inputs=2[top]; [bottom_left][bottom_right]hstack=inputs=2[bottom]; [top][bottom]vstack=inputs=2[out]";
        args.Append($"-filter_complex \"{filterComplex}\" ");
        args.Append("-map \"[out]\" -c:v libx264 -preset fast -crf 23 -maxrate 3000k -bufsize 6000k -f mpegts pipe:1");

        return args;
    }

    private static void ConfigureProcess(Process process, string commandExec, string formattedArgs)
    {
        process.StartInfo.FileName = commandExec;
        process.StartInfo.Arguments = formattedArgs;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
    }
}
