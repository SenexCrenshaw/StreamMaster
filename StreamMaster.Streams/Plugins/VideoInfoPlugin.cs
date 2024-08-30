using StreamMaster.Domain.Extensions;

using System.Diagnostics;
using System.Text.Json;
using System.Threading.Channels;

namespace StreamMaster.Streams.Plugins
{
    public class VideoInfoEventArgs(VideoInfo videoInfo, string id) : EventArgs
    {
        public VideoInfo VideoInfo { get; } = videoInfo;
        public string Id { get; } = id;
    }

    public class VideoInfoPlugin : IDisposable
    {
        private readonly ILogger<VideoInfoPlugin> _logger;
        private readonly IOptionsMonitor<Setting> _settingsMonitor;
        private readonly ChannelReader<byte[]> _channelReader;
        private readonly string name;
        private readonly string id;
        private readonly CancellationTokenSource cancellationTokenSource = new();
        private readonly Task? videoInfoTask;
        public event EventHandler<VideoInfoEventArgs>? VideoInfoUpdated;

        public VideoInfoPlugin(ILogger<VideoInfoPlugin> logger, IOptionsMonitor<Setting> settingsMonitor, ChannelReader<byte[]> channelReader, string id, string name)
        {
            this.id = id;
            this.name = name;
            _logger = logger;
            _settingsMonitor = settingsMonitor;
            _channelReader = channelReader;

            // Start the background task
            videoInfoTask = StartVideoInfoLoopAsync(cancellationTokenSource.Token);
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
                        videoInfo = await GetVideoInfoAsync(_channelReader, cancellationToken);

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
                        _logger.LogError(ex, "Error getting video info for {name}", name);
                        await Task.Delay(errorDelay, cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Handle the cancellation gracefully, no need to log TaskCanceledException
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in video info loop for {name}", name);
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

                byte[] videoMemory = await ReadChannelDataAsync(channelReader, 128 * 1024, cancellationToken);
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
                        _logger.LogError("FFprobe timeout");
                        ffprobeProcess.Kill();
                    }
                }, null, 60000, Timeout.Infinite);

                try
                {
                    await using (Stream stdin = ffprobeProcess.StandardInput.BaseStream)
                    {
                        await stdin.WriteAsync(videoMemory, cancellationToken).ConfigureAwait(false);
                        await stdin.FlushAsync(cancellationToken).ConfigureAwait(false);
                    }
                    //_logger.LogError("Written FFProbe data");
                    string output = await ffprobeProcess.StandardOutput.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
                    await ffprobeProcess.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

                    timer.Dispose();

                    if (string.IsNullOrEmpty(output))
                    {
                        _logger.LogError("Failed to get FFProbe output: {output}", output);
                        return null;
                    }

                    using JsonDocument document = JsonDocument.Parse(output);

                    string formattedJsonString = JsonSerializer.Serialize(document.RootElement, BuildInfo.JsonIndentOptions);

                    return new VideoInfo { JsonOutput = formattedJsonString, StreamId = id, StreamName = name, Created = SMDT.UtcNow };
                }
                finally
                {
                    if (ffprobeProcess != null)
                    {
                        if (!ffprobeProcess.HasExited)
                        {
                            ffprobeProcess?.Kill();
                        }
                        ffprobeProcess?.Dispose();
                    }
                    timer.Dispose();
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                //_logger.LogError(ex, "Exception occurred in GetVideoInfoAsync. Trying again in 1 minute");
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
                return [];
            }
        }

        protected virtual void OnVideoInfoUpdated(VideoInfo videoInfo)
        {
            VideoInfoUpdated?.Invoke(this, new VideoInfoEventArgs(videoInfo, id));
        }

        public void Stop()
        {
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
                _logger.LogInformation("Stop requested for video info plugin {name}", name);

                if (videoInfoTask != null)
                {
                    try
                    {
                        videoInfoTask.Wait();  // Wait for the task to complete
                    }
                    catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
                    {
                        // Handle the expected task cancellation without logging
                    }
                }
            }
        }

        public void Dispose()
        {
            Stop();
            cancellationTokenSource.Dispose();
        }
    }
}
