using System.Diagnostics;
using System.IO.Pipelines;
using System.Text.Json;

using StreamMaster.Domain.Extensions;

namespace StreamMaster.Streams.Plugins;

public class VideoInfoEventArgs(VideoInfo videoInfo, string id) : EventArgs
{
    public VideoInfo VideoInfo { get; } = videoInfo;
    public string Id { get; } = id;
}

public class VideoInfoPlugin : IDisposable
{
    private readonly ILogger<VideoInfoPlugin> _logger;
    private readonly IOptionsMonitor<Setting> _settingsMonitor;
    private readonly string _name;
    private readonly string _id;
    private readonly CircularBuffer _circularBuffer;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly Task _pipeReaderTask;
    private readonly Task _videoInfoTask;

    public Pipe Pipe { get; } = new();
    public event EventHandler<VideoInfoEventArgs>? VideoInfoUpdated;

    public VideoInfoPlugin(
        ILogger<VideoInfoPlugin> logger,
        IOptionsMonitor<Setting> settingsMonitor,
        string id,
        string name,
        int bufferSize = 128 * 1024) // 128 KB buffer
    {
        _logger = logger;
        _settingsMonitor = settingsMonitor;
        _id = id;
        _name = name;
        _circularBuffer = new CircularBuffer(bufferSize);

        // Start the pipeline reader and video info processing tasks
        _pipeReaderTask = ReadFromPipeAsync(_cancellationTokenSource.Token);
        _videoInfoTask = StartVideoInfoLoopAsync(_cancellationTokenSource.Token);
    }

    private async Task ReadFromPipeAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ReadResult result = await Pipe.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
                System.Buffers.ReadOnlySequence<byte> buffer = result.Buffer;

                foreach (ReadOnlyMemory<byte> segment in buffer)
                {
                    _circularBuffer.Write(segment.Span); // Write to the circular buffer
                }

                Pipe.Reader.AdvanceTo(buffer.End);

                if (result.IsCompleted)
                {
                    break; // Exit when the pipe is completed
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Graceful cancellation
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading from pipe for {name}", _name);
        }
        finally
        {
            await Pipe.Reader.CompleteAsync().ConfigureAwait(false);
        }
    }

    private async Task StartVideoInfoLoopAsync(CancellationToken cancellationToken)
    {
        TimeSpan initialDelay = TimeSpan.FromSeconds(15);
        TimeSpan normalDelay = TimeSpan.FromMinutes(5);
        TimeSpan errorDelay = TimeSpan.FromMinutes(1);

        try
        {
            await Task.Delay(initialDelay, cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                VideoInfo? videoInfo = null;
                try
                {
                    // Extract video info from the circular buffer
                    if (_circularBuffer.Size >= _circularBuffer.Capacity)
                    {
                        byte[] latestData = _circularBuffer.ReadAll();
                        videoInfo = await ExtractVideoInfoAsync(latestData, cancellationToken);
                    }

                    if (videoInfo != null)
                    {
                        OnVideoInfoUpdated(videoInfo);
                        await Task.Delay(normalDelay, cancellationToken);
                    }
                    else
                    {
                        await Task.Delay(errorDelay, cancellationToken);
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Error processing video info for {name}", _name);
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
            _logger.LogError(ex, "Unexpected error in video info loop for {name}", _name);
        }
    }

    private async Task<VideoInfo?> ExtractVideoInfoAsync(byte[] videoMemory, CancellationToken cancellationToken)
    {
        try
        {
            Setting settings = _settingsMonitor.CurrentValue;
            string? ffProbeExec = FileUtil.GetExec(settings.FFProbeExecutable);

            if (string.IsNullOrEmpty(ffProbeExec))
            {
                _logger.LogError("FFProbe executable not found.");
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
                _logger.LogError("Failed to start FFProbe process.");
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
                _logger.LogError("No output received from FFProbe.");
                return null;
            }

            using JsonDocument document = JsonDocument.Parse(output);
            string formattedJson = JsonSerializer.Serialize(document.RootElement, BuildInfo.JsonIndentOptions);

            return new VideoInfo
            {
                JsonOutput = formattedJson,
                StreamId = _id,
                StreamName = _name,
                Created = SMDT.UtcNow
            };
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error extracting video info for {name}", _name);
            return null;
        }
    }

    protected virtual void OnVideoInfoUpdated(VideoInfo videoInfo)
    {
        VideoInfoUpdated?.Invoke(this, new VideoInfoEventArgs(videoInfo, _id));
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
        Pipe.Writer.Complete();
        Pipe.Reader.Complete();
    }

    public void Dispose()
    {
        Stop();
        _cancellationTokenSource.Dispose();
        GC.SuppressFinalize(this);
    }
}
