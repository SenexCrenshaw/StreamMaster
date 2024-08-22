using System.Diagnostics;
using System.Threading.Channels;
namespace StreamMaster.Streams.Streams;
public class VideoCombiner : IVideoCombiner
{
    public async Task CombineVideosAsync(Stream stream1, Stream stream2, Stream stream3, Stream stream4, ChannelWriter<byte[]> writer, CancellationToken cancellationToken)
    {
        Process ffmpegProcess = StartFFmpegProcess() ?? throw new Exception("Failed to start ffmpeg process");
        Stream ffmpegInputStream = ffmpegProcess.StandardInput.BaseStream;
        Stream ffmpegOutputStream = ffmpegProcess.StandardOutput.BaseStream;

        Task[] readTasks =
        [
            ReadFromStreamAsync(stream1, ffmpegInputStream, cancellationToken),
            ReadFromStreamAsync(stream2, ffmpegInputStream, cancellationToken),
            ReadFromStreamAsync(stream3, ffmpegInputStream, cancellationToken),
            ReadFromStreamAsync(stream4, ffmpegInputStream, cancellationToken)
        ];

        Task writeTask = WriteToChannelAsync(ffmpegOutputStream, writer, cancellationToken);

        await Task.WhenAll(readTasks);
        ffmpegProcess.StandardInput.Close(); // Signal FFmpeg that no more input is coming
        await writeTask;
        ffmpegProcess.WaitForExit();
    }

    private static Process? StartFFmpegProcess()
    {
        ProcessStartInfo ffmpegStartInfo = new()
        {
            FileName = "ffmpeg",
            Arguments = "-f rawvideo -pixel_format yuv420p -video_size 1280x720 -i - -filter_complex \"[0:v][1:v][2:v][3:v]xstack=inputs=4:layout=0_0|w0_0|0_h0|w0_h0[v]\" -map \"[v]\" -f rawvideo -",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        return Process.Start(ffmpegStartInfo);
    }

    private static async Task ReadFromStreamAsync(Stream inputStream, Stream ffmpegInputStream, CancellationToken cancellationToken)
    {
        byte[] buffer = new byte[BuildInfo.BufferSize];
        int bytesRead;
        while ((bytesRead = await inputStream.ReadAsync(buffer, cancellationToken)) > 0)
        {
            await ffmpegInputStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
        }
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
