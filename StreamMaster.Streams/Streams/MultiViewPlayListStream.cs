using System.Diagnostics;
using System.Text;
namespace StreamMaster.Streams.Streams;

public class MultiViewPlayListStream(ILogger<MultiViewPlayListStream> logger, ICacheManager cacheManager) : IMultiViewPlayListStream
{
    public Task<(Stream? stream, int processId, ProxyStreamError? error)> HandleStream(IChannelBroadcaster channelBroadcaster, CancellationToken cancellationToken)
    {
        SMStreamInfo? smStreamInfo = channelBroadcaster.SMStreamInfo;
        if (smStreamInfo == null)
        {
            return Task.FromResult<(Stream?, int, ProxyStreamError?)>((null, -1, null));
        }
        logger.LogInformation("Getting MultiView stream for {streamName}", channelBroadcaster.SourceName);
        Stopwatch stopwatch = Stopwatch.StartNew();

        StringBuilder args = new();

        args.Append("-hide_banner -loglevel error ");

        foreach (SMChannelDto channel in channelBroadcaster.SMChannel.SMChannelDtos)
        {
            args.Append($"-i \"http://localhost:7095/v/{cacheManager.DefaultSG!.Id}/{channel.Id}\" ");
        }

        const string filter_complex = "[0:v]scale=960:540[top_left]; [1:v]scale=960:540[top_right]; [2:v]scale=960:540[bottom_left];[3:v]scale=960:540[bottom_right];[top_left][top_right]hstack=inputs=2[top]; [bottom_left][bottom_right]hstack=inputs=2[bottom]; [top][bottom]vstack=inputs=2[out]";

        args.Append($"-filter_complex \"{filter_complex}\" ");

        args.Append("-map \"[out]\" -c:v libx264 -preset fast -crf 23 -maxrate 3000k -bufsize 6000k -f mpegts pipe:1");

        const string command = "ffmpeg";
        string? exec = FileUtil.GetExec(command);
        if (exec == null)
        {
            logger.LogCritical("Command \"{command}\" not found", command);
            return Task.FromResult<(Stream?, int, ProxyStreamError?)>((null, -1, new ProxyStreamError { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = $"{command} not found" }));
        }

        using Process process = new();
        ConfigureProcess(process, exec, args.ToString());
        cancellationToken.ThrowIfCancellationRequested();

        bool processStarted = process.Start();

        if (!processStarted)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ProcessStartFailed, Message = "Failed to start process" };
            logger.LogError("Error: {ErrorMessage}", error.Message);
            return Task.FromResult<(Stream?, int, ProxyStreamError?)>((null, -1, error));
        }
        stopwatch.Stop();

        logger.LogInformation("Opened command with args \"{options}\" in {ElapsedMilliseconds} ms", command + ' ' + args, stopwatch.ElapsedMilliseconds);
        return Task.FromResult<(Stream?, int, ProxyStreamError?)>((process.StandardOutput.BaseStream, process.Id, null));
    }

    private static void ConfigureProcess(Process process, string commandExec, string formattedArgs)
    {
        process.StartInfo.FileName = commandExec;
        process.StartInfo.Arguments = formattedArgs;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
    }
}
