using System.Diagnostics;
using System.Threading.Channels;

namespace StreamMaster.Streams.Services
{
    public class Dubcer : IDubcer
    {
        private readonly Channel<byte[]> inputChannel = Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });

        public void DubcerChannels(ChannelReader<byte[]> channelReader, ChannelWriter<byte[]> channelWriter, CancellationToken cancellationToken)
        {
            _ = Task.Run(async () =>
            {
                Process ffmpegProcess = StartFFmpegProcess();

                try
                {
                    // Writing to ffmpeg
                    Task writeTask = Task.Run(async () =>
                    {
                        await foreach (byte[] data in channelReader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                break;
                            }

                            await ffmpegProcess.StandardInput.BaseStream.WriteAsync(data, cancellationToken).ConfigureAwait(false);
                            await inputChannel.Writer.WriteAsync(data, cancellationToken).ConfigureAwait(false);
                        }
                        ffmpegProcess.StandardInput.Close();
                    }, cancellationToken);

                    // Reading from ffmpeg
                    Task readTask = Task.Run(async () =>
                    {
                        byte[] buffer = new byte[8192];
                        int bytesRead;
                        while ((bytesRead = await ffmpegProcess.StandardOutput.BaseStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                break;
                            }

                            byte[] data = new byte[bytesRead];
                            Array.Copy(buffer, data, bytesRead);
                            await channelWriter.WriteAsync(data, cancellationToken).ConfigureAwait(false);
                        }
                        channelWriter.Complete();
                    }, cancellationToken);

                    await Task.WhenAll(writeTask, readTask).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // Log error
                    Debug.WriteLine($"Error in DubcerChannels: {ex.Message}");
                }
                finally
                {
                    ffmpegProcess.WaitForExit();
                    ffmpegProcess.Dispose();
                }
            }, cancellationToken);
        }

        private Process StartFFmpegProcess()
        {
            string? exec = FileUtil.GetExec("ffmpeg");
            ProcessStartInfo startInfo = new()
            {
                FileName = exec,
                Arguments = "-hide_banner -loglevel error -i pipe:0 -c:v libx264 -c:a ac3 -vsync 1 -f mpegts pipe:1",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            return Process.Start(startInfo);
        }
    }
}