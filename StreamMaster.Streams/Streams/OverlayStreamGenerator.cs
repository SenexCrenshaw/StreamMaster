using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using StreamMaster.Domain.Configuration;

using System.Diagnostics;
using System.Runtime.InteropServices;

public class OverlayStreamGenerator(IOptionsMonitor<Setting> intSettings) : IOverlayStreamGenerator
{
    public async Task StartOverlayStreamAsync(string text, string imagePath, string outputPath, CancellationToken cancellationToken)
    {
        Setting settings = intSettings.CurrentValue;
        string tempImagePath = Path.GetTempFileName() + ".png";
        GenerateOverlayImage(text, imagePath, tempImagePath);
        string args = $"-loop 1 -i {tempImagePath} -vf \"drawtext=fontfile=Fonts/Roboto-Regular.ttf:text='{text}':x=10:y=10:fontsize=24:fontcolor=white\" -f flv {outputPath}";
        string? ffmpegExec = GetFFPMpegExec();
        if (ffmpegExec == null)
        {
            return;
        }

        ProcessStartInfo startInfo = new()
        {
            FileName = ffmpegExec,
            Arguments = args,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process ffmpegProcess = new() { StartInfo = startInfo };

        ffmpegProcess.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
        ffmpegProcess.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);

        ffmpegProcess.Start();
        ffmpegProcess.BeginOutputReadLine();
        ffmpegProcess.BeginErrorReadLine();

        try
        {
            await Task.Run(ffmpegProcess.WaitForExit, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            if (!ffmpegProcess.HasExited)
            {
                ffmpegProcess.Kill();
            }
        }

        File.Delete(tempImagePath);
    }


    private string? GetFFPMpegExec()
    {
        Setting settings = intsettings.CurrentValue;

        string ffmpegExec = Path.Combine(BuildInfo.AppDataFolder, settings.FFMPegExecutable);

        if (!File.Exists(ffmpegExec) && !File.Exists(ffmpegExec + ".exe"))
        {
            if (!IsFFmpegAvailable())
            {

                return null;
            }
            ffmpegExec = "ffmpeg";
        }

        return ffmpegExec;
    }
    private static bool IsFFmpegAvailable()
    {
        string command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "where" : "which";
        ProcessStartInfo startInfo = new(command, "ffmpeg")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false
        };
        Process process = new()
        {
            StartInfo = startInfo
        };
        _ = process.Start();
        process.WaitForExit();
        return process.ExitCode == 0;
    }

    private void GenerateOverlayImage(string text, string imagePath, string outputPath)
    {
        using Image image = new Image<Rgba32>(1280, 720);
        if (File.Exists(imagePath))
        {
            using Image<Rgba32> overlayImage = Image.Load<Rgba32>(imagePath);
            image.Mutate(x => x.DrawImage(overlayImage, new Point(0, 0), 1f));
        }

        FontCollection fontCollection = new();
        FontFamily font = fontCollection.Add("Fonts/Roboto-Regular");
        TextOptions textOptions = new(new Font(font, 24))
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Origin = new PointF(10, 10)
        };

        image.Mutate(x => x.DrawText(text, new Font(font, 24), Color.White, new PointF(10, 10)));
        image.Save(outputPath, new PngEncoder());
    }
}
