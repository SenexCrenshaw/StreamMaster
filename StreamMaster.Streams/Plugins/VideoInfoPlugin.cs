using System.Diagnostics;
using System.Text.Json;
using System.Threading.Channels;

namespace StreamMaster.Streams.Plugins
{
    public class VideoInfoEventArgs : EventArgs
    {
        public VideoInfo VideoInfo { get; }
        public string Key { get; }

        public VideoInfoEventArgs(VideoInfo videoInfo, string key)
        {
            VideoInfo = videoInfo;
            Key = key;
        }
    }

    public class VideoInfoPlugin
    {
        private readonly ILogger<VideoInfoPlugin> _logger;
        private readonly IOptionsMonitor<Setting> _settingsMonitor;
        private readonly ChannelReader<byte[]> _channelReader;
        private readonly AutoResetEvent _videoInfoUpdatedEvent;
        private readonly string key;
        private readonly CancellationTokenSource cancellationTokenSource = new();
        public event EventHandler<VideoInfoEventArgs>? VideoInfoUpdated;
        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            WriteIndented = true
        };

        public VideoInfoPlugin(ILogger<VideoInfoPlugin> logger, IOptionsMonitor<Setting> settingsMonitor, ChannelReader<byte[]> channelReader, string key)
        {
            this.key = key;
            _logger = logger;
            _settingsMonitor = settingsMonitor;
            _channelReader = channelReader;
            _videoInfoUpdatedEvent = new AutoResetEvent(false);

            // Start the background task
            _ = StartVideoInfoLoopAsync(cancellationTokenSource.Token);
        }

        private async Task StartVideoInfoLoopAsync(CancellationToken cancellationToken)
        {
            TimeSpan initialDelay = TimeSpan.FromSeconds(15);
            TimeSpan normalDelay = TimeSpan.FromMinutes(5);
            TimeSpan errorDelay = TimeSpan.FromMinutes(1);

            await Task.Delay(initialDelay, cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    VideoInfo? videoInfo = await GetVideoInfoAsync(_channelReader, cancellationToken);

                    if (videoInfo != null)
                    {
                        _logger.LogInformation("Got video info for {key}", key);
                        OnVideoInfoUpdated(videoInfo);
                        await Task.Delay(normalDelay, cancellationToken);
                    }
                    else
                    {
                        await Task.Delay(errorDelay, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting video info for {key}", key);
                    await Task.Delay(errorDelay, cancellationToken);
                }
            }
        }

        private async Task<VideoInfo?> GetVideoInfoAsync(ChannelReader<byte[]> channelReader, CancellationToken cancellationToken)
        {
            try
            {
                Setting settings = _settingsMonitor.CurrentValue;
                string? ffProbeExec = FileUtil.GetExec(settings.FFProbeExecutable);

                if (string.IsNullOrEmpty(ffProbeExec))
                {
                    _logger.LogError("FFProbe executable \"{settings.FFProbeExecutable}\" not found.", settings.FFProbeExecutable);
                    return null;
                }

                const string options = "-loglevel error -print_format json -show_format -sexagesimal -show_streams - ";
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
                    _logger.LogError("Failed to start ffprobe process");
                    return null;
                }

                await using Timer timer = new(_ =>
                {

                    if (ffprobeProcess?.HasExited == false)
                    {
                        ffprobeProcess.Kill();
                    }
                }, null, 60000, Timeout.Infinite);

                try
                {
                    await using (Stream stdin = ffprobeProcess.StandardInput.BaseStream)
                    {
                        byte[] videoMemory = await ReadChannelDataAsync(channelReader, 1 * 1024 * 1024, cancellationToken);
                        await stdin.WriteAsync(videoMemory.AsMemory(0, videoMemory.Length), cancellationToken).ConfigureAwait(false);
                        await stdin.FlushAsync(cancellationToken).ConfigureAwait(false);
                    }

                    string output = await ffprobeProcess.StandardOutput.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
                    ffprobeProcess.WaitForExit();
                    timer.Dispose();

                    if (string.IsNullOrEmpty(output))
                    {
                        _logger.LogError("Failed to deserialize FFProbe output: {output}", output);
                        return null;
                    }

                    using JsonDocument document = JsonDocument.Parse(output);

                    // Serialize the document back to a JSON string with formatting
                    string formattedJsonString = JsonSerializer.Serialize(document.RootElement, jsonOptions);

                    return new VideoInfo { JsonOutput = formattedJsonString, StreamName = key };

                }
                finally
                {
                    ffprobeProcess?.Kill();
                    timer.Dispose();
                }
            }
            catch (Exception)
            {
                _logger.LogError("Exception occurred in GetVideoInfoAsync. Trying again in 1 minute");
                return null;
            }
        }

        private static async Task<byte[]> ReadChannelDataAsync(ChannelReader<byte[]> channelReader, int maxSize, CancellationToken cancellationToken)
        {
            try
            {
                byte[] buffer = new byte[maxSize];
                int totalBytesRead = 0;

                while (totalBytesRead < maxSize && await channelReader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    while (channelReader.TryRead(out byte[]? data))
                    {
                        int bytesToCopy = Math.Min(data.Length, maxSize - totalBytesRead);
                        Array.Copy(data, 0, buffer, totalBytesRead, bytesToCopy);
                        totalBytesRead += bytesToCopy;

                        if (totalBytesRead >= maxSize)
                        {
                            return buffer;
                        }
                    }
                }

                // If the buffer is not completely filled, return the filled portion
                if (totalBytesRead < maxSize)
                {
                    byte[] result = new byte[totalBytesRead];
                    Array.Copy(buffer, result, totalBytesRead);
                    return result;
                }

                return buffer;
            }
            catch (Exception ex)
            {
                // Log error and return an empty buffer
                Console.WriteLine($"Error reading channel data: {ex.Message}");
                return Array.Empty<byte>();
            }
        }

        protected virtual void OnVideoInfoUpdated(VideoInfo videoInfo)
        {
            VideoInfoUpdated?.Invoke(this, new VideoInfoEventArgs(videoInfo, key));
        }

        public void Stop()
        {
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
            }
        }
    }

}
