using System.Diagnostics;
using System.IO.Pipelines;
using System.Text.Json;

using StreamMaster.Domain.Extensions;

namespace StreamMaster.Streams.Plugins;

/// <summary>
/// A plugin to extract and process video information from a data stream.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="VideoInfoPlugin"/> class.
/// </remarks>
/// <param name="logger">Logger instance for logging events.</param>
/// <param name="settingsMonitor">Monitor for application settings.</param>
/// <param name="id">Unique identifier for the plugin.</param>
/// <param name="name">Name of the video source.</param>
/// <param name="bufferSize">The buffer size for storing video data.</param>
public class VideoInfoPlugin(
    ILogger<VideoInfoPlugin> logger,
    IOptionsMonitor<Setting> settingsMonitor,
    string id,
    string name,
    int bufferSize = 128 * 1024) : IStreamDataToClients, IDisposable
{
    private readonly CircularBuffer _circularBuffer = new(bufferSize);
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    /// <summary>
    /// Starts the video information processing loop.
    /// </summary>
    public void Start()
    {
        _ = StartVideoInfoLoopAsync();
    }

    /// <summary>
    /// Pipe for streaming data to clients.
    /// </summary>
    public Pipe Pipe { get; } = new();

    /// <summary>
    /// Event triggered when video information is updated.
    /// </summary>
    public event EventHandler<VideoInfoEventArgs>? VideoInfoUpdated;

    /// <summary>
    /// Streams data to clients by writing it into the circular buffer.
    /// </summary>
    /// <param name="buffer">The data to stream.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    public Task StreamDataToClientsAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
    {
        _circularBuffer.Write(buffer.Span);
        return Task.CompletedTask;
    }

    private async Task StartVideoInfoLoopAsync()
    {
        CancellationToken cancellationToken = _cancellationTokenSource.Token;

        TimeSpan initialDelay = TimeSpan.FromSeconds(15);
        TimeSpan normalDelay = TimeSpan.FromMinutes(5);
        TimeSpan errorDelay = TimeSpan.FromMinutes(15);

        try
        {
            await Task.Delay(initialDelay, cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (_circularBuffer.IsFull)
                    {
                        ReadOnlyMemory<byte> readableData = _circularBuffer.ReadAll();
                        if (!readableData.IsEmpty)
                        {
                            VideoInfo? videoInfo = await ExtractVideoInfoAsync(readableData, cancellationToken);
                            _circularBuffer.MarkRead(readableData.Length);

                            if (videoInfo != null)
                            {
                                OnVideoInfoUpdated(videoInfo);
                                await Task.Delay(normalDelay, cancellationToken);
                            }
                        }
                    }
                    else
                    {
                        await Task.Delay(errorDelay, cancellationToken);
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    logger.LogError(ex, "Error processing video info for {Name}", name);
                    await Task.Delay(errorDelay, cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Graceful cancellation
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in video info loop for {Name}", name);
        }
    }

    private async Task<VideoInfo?> ExtractVideoInfoAsync(ReadOnlyMemory<byte> videoMemory, CancellationToken cancellationToken)
    {
        try
        {
            Setting settings = settingsMonitor.CurrentValue;
            string? ffProbeExec = FileUtil.GetExec(settings.FFProbeExecutable);

            if (string.IsNullOrEmpty(ffProbeExec))
            {
                logger.LogError("FFProbe executable not found.");
                return null;
            }

            const string options = "-loglevel error -print_format json -show_format -sexagesimal -show_streams -";
            ProcessStartInfo startInfo = new()
            {
                FileName = ffProbeExec,
                Arguments = options,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
            };

            using Process? ffprobeProcess = Process.Start(startInfo);
            if (ffprobeProcess == null)
            {
                logger.LogError("Failed to start FFProbe process.");
                return null;
            }

            await using (Stream stdin = ffprobeProcess.StandardInput.BaseStream)
            {
                await stdin.WriteAsync(videoMemory, cancellationToken).ConfigureAwait(false);
                await stdin.FlushAsync(cancellationToken).ConfigureAwait(false);
            }

            string output = await ffprobeProcess.StandardOutput.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
            await ffprobeProcess.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrEmpty(output))
            {
                logger.LogError("No output received from FFProbe.");
                return null;
            }

            using JsonDocument document = JsonDocument.Parse(output);
            string formattedJson = JsonSerializer.Serialize(document.RootElement, BuildInfo.JsonIndentOptions);

            return new VideoInfo
            {
                JsonOutput = formattedJson,
                StreamId = id,
                StreamName = name,
                Created = SMDT.UtcNow
            };
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting video info for {Name}", name);
            return null;
        }
    }

    protected virtual void OnVideoInfoUpdated(VideoInfo videoInfo)
    {
        VideoInfoUpdated?.Invoke(this, new VideoInfoEventArgs(videoInfo, id));
    }

    /// <summary>
    /// Stops the video info loop and releases resources.
    /// </summary>
    public void Stop()
    {
        _cancellationTokenSource.Cancel();
        Pipe.Writer.Complete();
        Pipe.Reader.Complete();
    }

    /// <summary>
    /// Disposes the plugin and releases resources.
    /// </summary>
    public void Dispose()
    {
        Stop();
        _cancellationTokenSource.Dispose();
        GC.SuppressFinalize(this);
    }
}
