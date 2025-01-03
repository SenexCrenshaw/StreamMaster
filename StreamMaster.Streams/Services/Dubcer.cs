using System.Diagnostics;
using System.Threading.Channels;

using StreamMaster.Streams.Domain.Helpers;

namespace StreamMaster.Streams.Services;

/// <summary>
/// Manages a process (e.g., ffmpeg) for streaming and transcoding using channels for data flow.
/// </summary>
public class Dubcer : IDisposable
{
    private readonly ILogger<Dubcer> logger;
    private readonly Channel<byte[]> inputChannel = ChannelHelper.GetChannel();
    private Process? ffmpegProcess;
    private readonly CancellationTokenSource cancellationTokenSource = new();

    public string SourceName { get; set; } = string.Empty;

    public Channel<byte[]> DubcerChannel { get; } = ChannelHelper.GetChannel();

    public Dubcer(ILogger<Dubcer> logger)
    {
        this.logger = logger;
        StartFFmpegProcess();
        StartFeedingFFmpeg();
    }

    private void StartFeedingFFmpeg()
    {
        _ = Task.Run(async () =>
        {
            try
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await foreach (byte[] data in DubcerChannel.Reader.ReadAllAsync(cancellationTokenSource.Token))
                    {
                        await inputChannel.Writer.WriteAsync(data, cancellationTokenSource.Token).ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Data feeding to ffmpeg canceled.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error feeding data to ffmpeg");
            }
            finally
            {
                inputChannel.Writer.TryComplete();
                ffmpegProcess?.StandardInput.Close();
            }
        }, cancellationTokenSource.Token);
    }

    private void StartFFmpegProcess()
    {
        string? exec = FileUtil.GetExec("ffmpeg");
        if (exec == null)
        {
            logger.LogError("FFmpeg executable not found.");
            return;
        }

        ProcessStartInfo startInfo = new()
        {
            FileName = exec,
            Arguments = "-hide_banner -loglevel error -hwaccel cuda -i pipe:0 -c:v h264_nvenc -preset fast -cq 23 -c:a aac -b:a 128k -f mpegts pipe:1",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        ffmpegProcess = Process.Start(startInfo);

        if (ffmpegProcess == null)
        {
            logger.LogError("Failed to start ffmpeg process.");
            return;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                byte[] buffer = new byte[BuildInfo.BufferSize];
                while (true)
                {
                    int bytesRead = await ffmpegProcess.StandardOutput.BaseStream.ReadAsync(buffer, cancellationTokenSource.Token).ConfigureAwait(false);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    await DubcerChannel.Writer.WriteAsync(buffer.AsMemory(0, bytesRead).ToArray(), cancellationTokenSource.Token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Reading from ffmpeg process canceled.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error reading from ffmpeg");
            }
            finally
            {
                DubcerChannel.Writer.TryComplete();
            }
        }, cancellationTokenSource.Token);
    }

    public void Stop()
    {
        if (cancellationTokenSource.IsCancellationRequested)
        {
            return;
        }

        cancellationTokenSource.Cancel();

        try
        {
            inputChannel.Writer.TryComplete();
            DubcerChannel.Writer.TryComplete();

            if (ffmpegProcess?.HasExited == false)
            {
                try
                {
                    ffmpegProcess.StandardInput.Close();
                    ffmpegProcess.WaitForExit();
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error waiting for ffmpeg process to exit.");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during shutdown.");
        }
        finally
        {
            Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                ffmpegProcess?.Dispose();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error disposing resources.");
            }
        }
    }
}
