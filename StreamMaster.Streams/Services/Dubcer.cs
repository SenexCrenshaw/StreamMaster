using StreamMaster.Streams.Domain.Helpers;

using System.Diagnostics;
using System.Threading.Channels;

namespace StreamMaster.Streams.Services
{
    public class Dubcer
    {//
        public string SourceName { get; set; } = string.Empty;
        private readonly ILogger<IBroadcasterBase> logger;
        private readonly Channel<byte[]> inputChannel = ChannelHelper.GetChannel();
        private Process? ffmpegProcess;

        private readonly CancellationTokenSource cancellationTokenSource = new();

        public Channel<byte[]> DubcerChannel { get; } = ChannelHelper.GetChannel();

        public Dubcer(ILogger<IBroadcasterBase> logger)
        {
            this.logger = logger;
            StartFFmpegProcess();
            StartFeedingFFmpeg(inputChannel);
        }

        private void StartFeedingFFmpeg(ChannelReader<byte[]> channelReader)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    while (!cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        if (DubcerChannel != null)
                        {
                            await foreach (byte[] data in DubcerChannel.Reader.ReadAllAsync(cancellationTokenSource.Token))
                            {
                                await inputChannel.Writer.WriteAsync(data, cancellationTokenSource.Token);
                            }
                        }
                        else
                        {
                            // Wait briefly before checking again to avoid a tight loop
                            await Task.Delay(100, cancellationTokenSource.Token);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        logger.LogError(ex, "Error feeding data to ffmpeg");
                    }
                }
                finally
                {
                    ffmpegProcess?.StandardInput.Close();
                }
            });
        }

        private void StartFFmpegProcess()
        {
            string? exec = FileUtil.GetExec("ffmpeg");
            ProcessStartInfo startInfo = new()
            {
                FileName = exec,
                Arguments = "-hide_banner -loglevel error -hwaccel cuda -i pipe:0 -c:v h264_nvenc -preset fast -cq 23 -c:a aac -b:a 128k -f mpegts pipe:1",// " - hide_banner -loglevel error -i pipe:0 -c copy -vsync 1 -f mpegts pipe:1",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            ffmpegProcess = Process.Start(startInfo);

            // Start reading from ffmpeg and writing to DubcerChannel
            _ = Task.Run(async () =>
            {
                try
                {
                    byte[] buffer = new byte[BuildInfo.BufferSize];
                    int bytesRead;
                    while ((bytesRead = await ffmpegProcess!.StandardOutput.BaseStream.ReadAsync(buffer, cancellationTokenSource.Token).ConfigureAwait(false)) > 0)
                    {
                        byte[] data = new byte[bytesRead];
                        Array.Copy(buffer, data, bytesRead);
                        await DubcerChannel.Writer.WriteAsync(data, cancellationTokenSource.Token).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    if (!cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        logger.LogError(ex, "Error reading from ffmpeg");
                    }
                }
                finally
                {
                    DubcerChannel.Writer.TryComplete();
                }
            });
        }

        public void Stop()
        {
            try
            {
                // Check if cancellation has already been requested
                if (!cancellationTokenSource.IsCancellationRequested)
                {
                    // Signal cancellation to any ongoing tasks
                    cancellationTokenSource.Cancel();
                }

                // Complete the input channel
                inputChannel.Writer.TryComplete();

                // Wait for ffmpeg process to exit quietly
                if (ffmpegProcess != null && !ffmpegProcess.HasExited)
                {
                    try
                    {
                        ffmpegProcess.StandardInput.Close(); // Close input to ffmpeg
                        ffmpegProcess.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Error while waiting for ffmpeg process to exit, but continuing with shutdown.");
                    }
                }

                DubcerChannel.Writer.TryComplete();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during shutdown of Dubcer");
            }
            finally
            {
                try
                {
                    ffmpegProcess?.Dispose();
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error disposing ffmpeg process, but continuing with shutdown.");
                }

                try
                {
                    cancellationTokenSource.Dispose();
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error disposing CancellationTokenSource, but continuing with shutdown.");
                }
            }
        }
    }
}