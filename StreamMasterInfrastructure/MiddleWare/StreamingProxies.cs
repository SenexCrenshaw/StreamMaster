using StreamMasterDomain.Common;

using StreamMasterInfrastructure.Common;

using System.Diagnostics;
using System.IO.Pipelines;
using System.Runtime.InteropServices;

namespace StreamMasterInfrastructure.MiddleWare;

public static class StreamingProxies
{
    private static readonly HttpClient client = CreateHttpClient();

    /// <summary>
    /// Get FFMpeg Stream from url <strong>Supports failover</strong>
    /// </summary>
    /// <param name="streamUrl">URL to stream from</param>
    /// <param name="ffMPegExecutable">
    /// Path to FFMpeg executable. If it doesnt exist <strong>null</strong> is returned
    /// </param>
    /// <param name="user_agent">user_agent string</param>
    /// <returns><strong>A FFMpeg backed stream or null</strong></returns>
    public static async Task<(Stream? stream, ProxyStreamError? error)> GetFFMpegStream(string streamUrl)
    {
        Setting setting = FileUtil.GetSetting();

        if (!File.Exists(setting.FFMPegExecutable))
        {
            if (!IsFFmpegAvailable())
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = $"FFmpeg executable file not found: {setting.FFMPegExecutable}" };
                return (null, error);
            }
        }

        try
        {
            using Process process = new();
            process.StartInfo.FileName = setting.FFMPegExecutable;
            process.StartInfo.Arguments = $"-hide_banner -loglevel error -i \"{streamUrl}\" -c copy -f mpegts pipe:1 -user_agent \"streammaster\"";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;

            _ = process.Start();

            return (await Task.FromResult(process.StandardOutput.BaseStream).ConfigureAwait(false), null);
        }
        catch (IOException ex)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.IoError, Message = ex.Message };
            return (null, error);
        }
        catch (Exception ex)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.UnknownError, Message = ex.Message };
            return (null, error);
        }
    }

    public static async Task<(Stream? stream, ProxyStreamError? error)> GetProxyStream(string sourceUrl, CancellationToken cancellation)
    {
        try
        {
            var parser = new M3u8Parser();
            var m3u8PlayList = await parser.ParsePlaylistAsync(sourceUrl);

            if (m3u8PlayList == null || !m3u8PlayList.Streams.Any())
            {
                HttpResponseMessage? response = await client.GetWithRedirectAsync(sourceUrl, cancellationToken: cancellation).ConfigureAwait(false);

                if (response == null || !response.IsSuccessStatusCode)
                {
                    return (null, new ProxyStreamError { ErrorCode = ProxyStreamErrorCode.DownloadError, Message = "Could not retrieve stream utl" });
                }

                Stream stream = await response.Content.ReadAsStreamAsync();
                return (stream, null);
            }

            if (m3u8PlayList.IndependentSegments)
            {
                return (null, new ProxyStreamError
                {
                    ErrorCode = ProxyStreamErrorCode.MasterPlayListNotSupported,
                    Message = "M3U8 Master Playlis not supported"
                }); ;
            }

            var streams = m3u8PlayList.Streams.OrderByDescending(s => s.Bandwidth).First();

            var pipe = new Pipe();

            _ = Task.Run(async () =>
            {
                await DownloadSegments(streams.Segments, sourceUrl, pipe.Writer, cancellation);
                pipe.Writer.Complete();
            });

            return (pipe.Reader.AsStream(), null);
        }
        catch (Exception ex)
        {
            return (null, new ProxyStreamError
            {
                ErrorCode = ProxyStreamErrorCode.DownloadError,
                Message = ex.Message
            });
        }
    }

    private static HttpClient CreateHttpClient()
    {
        var client = new HttpClient(new HttpClientHandler()
        {
            AllowAutoRedirect = true,
        });
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; streammaster/1.0)");
        return client;
    }

    private static async Task DownloadSegments(List<M3u8Segment> segments, string sourceUrl, PipeWriter pipeWriter, CancellationToken cancellation)
    {
        while (!cancellation.IsCancellationRequested)
        {
            foreach (var segment in segments)
            {
                if (cancellation.IsCancellationRequested)
                {
                    break;
                }
                HttpResponseMessage? response;
                try
                {
                    //Debug.WriteLine($"{DateTime.Now} Streaming {segment.Uri} Delay {segment.Duration} byteRangeStart {byteRangeStart} byteRangeLength {byteRangeLength} ");

                    response = await client.GetWithRedirectAsync(
                        segment.Uri,
                        segment.ByterangeStart,
                        segment.ByterangeLength,
                        cancellationToken: cancellation
                        ).ConfigureAwait(false);

                    if (response == null)
                    {
                        continue;
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.Error.WriteLine($"Error downloading segment: {segment.Uri}. Status code: {response.StatusCode}");
                        continue;
                    }

                    using Stream stream = await response.Content.ReadAsStreamAsync(cancellation).ConfigureAwait(false);

                    await stream.CopyToAsync(pipeWriter, cancellationToken: cancellation).ConfigureAwait(false);
                    await Task.Delay((int)segment.Duration * 700, cancellation).ConfigureAwait(false);
                }
                catch
                {
                    Console.Error.WriteLine($"Error downloading segment: {segment.Uri}");
                    continue;
                }
            }

            var parser = new M3u8Parser();
            var m3u8PlayList = await parser.ParsePlaylistAsync(sourceUrl);

            if (m3u8PlayList == null)
            {
                //Debug.WriteLine($"{DateTime.Now} m3u8PlayList is null");
                break;
            }

            var streams = m3u8PlayList.Streams.OrderByDescending(s => s.Bandwidth).First();
            var existingMediaSequence = segments.Select(a => a.MediaSequence).ToList();
            segments = streams.Segments.Where(a => !existingMediaSequence.Contains(a.MediaSequence)).ToList();
        }
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
