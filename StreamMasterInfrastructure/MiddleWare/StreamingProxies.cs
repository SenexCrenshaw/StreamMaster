using StreamMasterDomain.Common;

using StreamMasterInfrastructure.Common;

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace StreamMasterInfrastructure.MiddleWare;

public static class StreamingProxies
{
    private static readonly HttpClient client = CreateHttpClient();

    public static async Task<(Stream? stream, int processId, ProxyStreamError? error)> GetFFMpegStream(string streamUrl)
    {
        Setting setting = FileUtil.GetSetting();

        var ffmpegExec = Path.Combine(Constants.ConfigFolder, setting.FFMPegExecutable);

        if (!File.Exists(ffmpegExec) && !File.Exists(ffmpegExec+".exe"))
        {       
            if (!IsFFmpegAvailable())
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = $"FFmpeg executable file not found: {setting.FFMPegExecutable}" };
                return (null, -1, error);
            }
            ffmpegExec = "ffmpeg";        
        }

        try
        {            
            using Process process = new();
            process.StartInfo.FileName = ffmpegExec;
            process.StartInfo.Arguments = $"-hide_banner -loglevel error -i \"{streamUrl}\" -c copy -f mpegts pipe:1 -user_agent \"{setting.ClientUserAgent}\"";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;

            _ = process.Start();

            return (await Task.FromResult(process.StandardOutput.BaseStream).ConfigureAwait(false), process.Id, null);
        }
        catch (IOException ex)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.IoError, Message = ex.Message };
            return (null, -1, error);
        }
        catch (Exception ex)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.UnknownError, Message = ex.Message };
            return (null, -1, error);
        }
    }

    public static async Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxyStream(string sourceUrl, CancellationToken cancellation)
    {
        try
        {
            HttpResponseMessage? response = await client.GetWithRedirectAsync(sourceUrl, cancellationToken: cancellation).ConfigureAwait(false);

            if (response == null || !response.IsSuccessStatusCode)
            {
                return (null, -1, new ProxyStreamError { ErrorCode = ProxyStreamErrorCode.DownloadError, Message = "Could not retrieve stream utl" });
            }

            string contentType = response.Content.Headers.ContentType.MediaType;

            if (contentType.Equals("application/vnd.apple.mpegurl", StringComparison.OrdinalIgnoreCase) ||
                        contentType.Equals("audio/mpegurl", StringComparison.OrdinalIgnoreCase) ||
                       contentType.Equals("application/x-mpegURL", StringComparison.OrdinalIgnoreCase))
            {
                return await GetFFMpegStream(sourceUrl).ConfigureAwait(false);
            }

            Stream stream = await response.Content.ReadAsStreamAsync(cancellation);
            return (stream, -1, null);
        }
        catch (Exception ex)
        {
            return (null, -1, new ProxyStreamError
            {
                ErrorCode = ProxyStreamErrorCode.DownloadError,
                Message = ex.Message
            });
        }
    }

    private static HttpClient CreateHttpClient()
    {
        var setting = FileUtil.GetSetting();
        var client = new HttpClient(new HttpClientHandler()
        {
            AllowAutoRedirect = true,
        });
        client.DefaultRequestHeaders.UserAgent.ParseAdd(setting.ClientUserAgent);
        return client;
    }

    private static bool IsFFmpegAvailable()
    {
        string command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "where" : "which";
        ProcessStartInfo startInfo = new ProcessStartInfo(command, "ffmpeg");
        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;
        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();
        process.WaitForExit();
        return process.ExitCode == 0;
    }
}
