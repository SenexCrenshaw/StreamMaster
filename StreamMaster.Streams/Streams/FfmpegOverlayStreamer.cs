using StreamMaster.Domain.Configuration;

using System.Diagnostics;

public class FfmpegOverlayStreamer
{
    private readonly string ffmpegPath;
    private readonly string inputUrl;
    private readonly string outputUrl;
    private readonly OverlayStreamGenerator overlayGenerator;
    private Process ffmpegProcess;

    public FfmpegOverlayStreamer(IOptionsMonitor<Setting> intSettings, string outputUrl)
    {
        ffmpegPath = ffmpegPath ?? throw new ArgumentNullException(nameof(ffmpegPath));
        inputUrl = inputUrl ?? throw new ArgumentNullException(nameof(inputUrl));
        this.outputUrl = outputUrl ?? throw new ArgumentNullException(nameof(outputUrl));
        overlayGenerator = new OverlayStreamGenerator(intSettings);
    }

    public async Task StartStreamingAsync(string overlayText, string overlayImagePath, CancellationToken cancellationToken)
    {
        string overlayOutputPath = "overlay.flv";
        await overlayGenerator.StartOverlayStreamAsync(overlayText, overlayImagePath, overlayOutputPath, cancellationToken);

        ProcessStartInfo startInfo = new()
        {
            FileName = ffmpegPath,
            Arguments = $"-i {inputUrl} -i {overlayOutputPath} -filter_complex \"[0:v][1:v] overlay=10:10\" -c:v libx264 -f flv {outputUrl}",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        ffmpegProcess = new Process { StartInfo = startInfo };

        ffmpegProcess.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
        ffmpegProcess.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);

        ffmpegProcess.Start();
        ffmpegProcess.BeginOutputReadLine();
        ffmpegProcess.BeginErrorReadLine();

        try
        {
            await Task.Run(() => ffmpegProcess.WaitForExit(), cancellationToken);
        }
        catch (OperationCanceledException)
        {
            if (!ffmpegProcess.HasExited)
            {
                ffmpegProcess.Kill();
            }
        }
    }

    public void StopStreaming()
    {
        if (ffmpegProcess != null && !ffmpegProcess.HasExited)
        {
            ffmpegProcess.Kill();
        }
    }
}
