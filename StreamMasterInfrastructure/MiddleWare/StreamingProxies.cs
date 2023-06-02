using StreamMasterDomain.Common;

using System.Diagnostics;

namespace StreamMasterInfrastructure.MiddleWare;

public static class StreamingProxies
{
    /// <summary>
    /// Get FFMpeg Stream from url <strong>Supports failover</strong>
    /// </summary>
    /// <param name="streamUrl">URL to stream from</param>
    /// <param name="ffMPegExecutable">
    /// Path to FFMpeg executable. If it doesnt exist <strong>null</strong> is returned
    /// </param>
    /// <param name="user_agent">user_agent string</param>
    /// <returns><strong>A FFMpeg backed stream or null</strong></returns>
    public static async Task<(Stream? stream, ProxyStreamError? error)> GetFFMpegStream(string streamUrl, string ffMPegExecutable, string user_agent)
    {
        if (!File.Exists(ffMPegExecutable))
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = $"FFmpeg executable file not found: {ffMPegExecutable}" };
            return (null, error);
        }

        try
        {
            using Process process = new();
            process.StartInfo.FileName = ffMPegExecutable;
            process.StartInfo.Arguments = $"-hide_banner -loglevel error -i \"{streamUrl}\" -c copy -f mpegts pipe:1 -user_agent \"{user_agent}\"";
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

    /// <summary>
    /// This is a pass through proxy <strong>Supports failover</strong>
    /// </summary>
    /// <param name="streamUrl">URL to stream from</param>
    /// <param name="user_agent">user_agent string</param>
    /// <returns><strong>A FFMpeg backed stream or null</strong></returns>
    public static async Task<(Stream? stream, ProxyStreamError? error)> GetProxyStream(string streamUrl)
    {
        try
        {
            using HttpClientHandler handler = new() { AllowAutoRedirect = true };
            using HttpClient httpClient = new(handler);
            string userAgentString = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36 Edg/110.0.1587.57";
            httpClient.DefaultRequestHeaders.Add("User-Agent", userAgentString);

            int redirectCount = 0;

            HttpResponseMessage response = await httpClient.GetAsync(streamUrl, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            while (response.StatusCode == System.Net.HttpStatusCode.Redirect)
            {
                ++redirectCount;
                if (response.Headers.Location == null || redirectCount > 10)
                {
                    break;
                }

                string location = response.Headers.Location.ToString();
                response = await httpClient.GetAsync(location, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            }

            _ = response.EnsureSuccessStatusCode();

            return (await response.Content.ReadAsStreamAsync().ConfigureAwait(false), null);
        }
        catch (HttpRequestException ex)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.HttpRequestError, Message = ex.Message };
            return (null, error);
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
}
