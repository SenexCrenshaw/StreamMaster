using StreamMaster.Streams.Domain.Helpers;

using System.Diagnostics;
using System.Threading.Channels;

namespace StreamMaster.Streams.Services
{
    public class Dubcer
    {
        public string SourceName { get; set; } = string.Empty;
        private readonly ILogger<IBroadcasterBase> logger;
        private readonly Channel<byte[]> inputChannel = Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });
        private Process? ffmpegProcess;
        private readonly CancellationTokenSource cancellationTokenSource = new();

        public Channel<byte[]> DubcerChannel { get; } = ChannelHelper.GetChannel();

        public Dubcer(ILogger<IBroadcasterBase> logger)
        {
            this.logger = logger;
            StartFFmpegProcess();
            StartFeedingFFmpeg(inputChannel.Reader);
        }

        public void SetSourceChannel(ChannelReader<byte[]> sourceChannelReader)
        {
            _ = Task.Run(async () =>
            {
                await foreach (byte[] data in sourceChannelReader.ReadAllAsync(cancellationTokenSource.Token))
                {
                    await inputChannel.Writer.WriteAsync(data, cancellationTokenSource.Token);
                }
            });
        }

        public void Stop()
        {
            try
            {
                // Signal cancellation to any ongoing tasks
                cancellationTokenSource.Cancel();

                // Complete the input channel
                inputChannel.Writer.TryComplete();

                // Wait for ffmpeg process to exit
                if (ffmpegProcess != null && !ffmpegProcess.HasExited)
                {
                    ffmpegProcess.StandardInput.Close(); // Close input to ffmpeg
                    ffmpegProcess.WaitForExit();
                }

                DubcerChannel.Writer.TryComplete();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during shutdown of Dubcer");
            }
            finally
            {
                ffmpegProcess?.Dispose();
                cancellationTokenSource.Dispose();
            }
        }

        private void StartFeedingFFmpeg(ChannelReader<byte[]> channelReader)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    while (await channelReader.WaitToReadAsync(cancellationTokenSource.Token).ConfigureAwait(false))
                    {
                        while (channelReader.TryRead(out byte[]? data))
                        {
                            await ffmpegProcess!.StandardInput.BaseStream.WriteAsync(data, cancellationTokenSource.Token).ConfigureAwait(false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!cancellationTokenSource.IsCancellationRequested)
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
                Arguments = "-hide_banner -loglevel error -i pipe:0 -c copy -vsync 1 -f mpegts pipe:1",
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
                    byte[] buffer = new byte[8192];
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
                    if (!cancellationTokenSource.IsCancellationRequested)
                    {
                        logger.LogError(ex, "Error reading from ffmpeg");
                    }
                }
                finally
                {
                    DubcerChannel.Writer.Complete();
                }
            });
        }
    }
}
