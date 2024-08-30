using StreamMaster.Streams.Domain.Events;
using StreamMaster.Streams.Domain.Helpers;

using System.Diagnostics;
using System.Threading.Channels;
namespace StreamMaster.Streams.Handlers;
public class VideoCombiner : BroadcasterBase, IVideoCombiner
{
    private readonly ILogger logger;
    private int _isStopped;

    //public VideoCombiner() : base(null, null)
    //{

    //}

    public VideoCombiner(ILogger<IBroadcasterBase> logger, int Id, string Name, IOptionsMonitor<Setting> _settings) : base(logger, _settings)
    {
        this.logger = logger;
        this.Id = Id;
        this.Name = Name;
    }

    public event EventHandler<VideoCombinerStopped>? OnVideoCombinerStoppedEvent;

    public override string StringId()
    {
        return Id.ToString();
    }

    /// <inheritdoc/>
    public int Id { get; }

    public override void Stop()
    {
        if (Interlocked.CompareExchange(ref _isStopped, 1, 0) == 0)
        {
            // Derived-specific logic before stopping
            logger.LogInformation("Source Broadcaster stopped: {Name}", Name);

            // Call base class stop logic
            base.Stop();

            // Additional cleanup or finalization
            OnVideoCombinerStoppedEvent?.Invoke(this, new VideoCombinerStopped(Id, Name));
        }
    }
    public async Task CombineVideosAsync(IChannelBroadcaster channelBroadcaster1, IChannelBroadcaster channelBroadcaster2, IChannelBroadcaster channelBroadcaster3, IChannelBroadcaster channelBroadcaster4, CancellationToken cancellationToken)
    {
        _ = Task.Run(async () =>
        {
            Process ffmpegProcess = StartFFmpegProcess() ?? throw new Exception("Failed to start ffmpeg process");
            Stream ffmpegInputStream = ffmpegProcess.StandardInput.BaseStream;
            Stream ffmpegOutputStream = ffmpegProcess.StandardOutput.BaseStream;

            Task[] readTasks =
            [
                ReadFromStreamAsync(channelBroadcaster1, ffmpegInputStream, cancellationToken),
            ReadFromStreamAsync(channelBroadcaster2, ffmpegInputStream, cancellationToken),
            ReadFromStreamAsync(channelBroadcaster3, ffmpegInputStream, cancellationToken),
            ReadFromStreamAsync(channelBroadcaster4, ffmpegInputStream, cancellationToken)
            ];

            SetSourceStream(ffmpegOutputStream, Name, cancellationToken);

            //Task writeTask = WriteToChannelAsync(ffmpegOutputStream, writer, cancellationToken);

            await Task.WhenAll(readTasks);
            ffmpegProcess.StandardInput.Close(); // Signal FFmpeg that no more input is coming
                                                 //await writeTask;

            ffmpegProcess.WaitForExit();
            OnVideoCombinerStoppedEvent?.Invoke(this, new VideoCombinerStopped(Id, Name));
        }, cancellationToken);
    }

    private static Process? StartFFmpegProcess()
    {
        string? exec = FileUtil.GetExec("ffmpeg");
        if (exec == null)
        {
            return null;
        }

        ProcessStartInfo ffmpegStartInfo = new()
        {
            FileName = exec,
            Arguments = "-f rawvideo -pixel_format yuv420p -video_size 1280x720 -i - -filter_complex \"[0:v][1:v][2:v][3:v]xstack=inputs=4:layout=0_0|w0_0|0_h0|w0_h0[v]\" -map \"[v]\" -f rawvideo -",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        return Process.Start(ffmpegStartInfo);
    }

    private static async Task ReadFromStreamAsync(IChannelBroadcaster ChannelBroadcaster, Stream ffmpegInputStream, CancellationToken cancellationToken)
    {
        Channel<byte[]> channel = ChannelHelper.GetChannel();
        ChannelBroadcaster.AddChannelStreamer(ChannelBroadcaster.Id, channel.Writer);

        byte[] read = await channel.Reader.ReadAsync(cancellationToken);
        await ffmpegInputStream.WriteAsync(read.AsMemory(0, read.Length), cancellationToken);
    }

    private static async Task WriteToChannelAsync(Stream ffmpegOutputStream, ChannelWriter<byte[]> writer, CancellationToken cancellationToken)
    {
        byte[] buffer = new byte[BuildInfo.BufferSize];
        int bytesRead;
        while ((bytesRead = await ffmpegOutputStream.ReadAsync(buffer, cancellationToken)) > 0)
        {
            byte[] data = new byte[bytesRead];
            Buffer.BlockCopy(buffer, 0, data, 0, bytesRead);
            await writer.WriteAsync(data, cancellationToken);
        }

        writer.Complete();
    }
}
