using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Common;

using System.Collections.Concurrent;
using System.Diagnostics;

namespace StreamMasterInfrastructure.MiddleWare;

public class StreamManager
{
    private readonly ILogger _logger;
    private readonly ConcurrentDictionary<string, StreamInformation> _streamInformations;

    public StreamManager(ILogger logger)
    {
        _logger = logger;
        _streamInformations = new ConcurrentDictionary<string, StreamInformation>();
    }

    public bool AddStreamInfo(StreamInformation streamStreamInfo)
    {
        return _streamInformations.TryAdd(streamStreamInfo.StreamUrl, streamStreamInfo);
    }

    public bool DecrementClientCounter(StreamerConfiguration config)
    {
        if (_streamInformations.TryGetValue(config.CurentVideoStream.User_Url, out var _streamInformation))
        {
            var setting = FileUtil.GetSetting();

            _streamInformation.RemoveStreamConfiguration(config);

            if (_streamInformation.ClientCount == 0)
            {
                _streamInformations.TryRemove(config.CurentVideoStream.User_Url, out _);
                _streamInformation.Stop();

                if (_streamInformation.ProcessId > 0)
                {
                    try
                    {
                        var procName = CheckProcessExists(_streamInformation.ProcessId);
                        if (procName != null)
                        {
                            Process process = Process.GetProcessById(_streamInformation.ProcessId);
                            process.Kill();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error killing process {ProcessId}", _streamInformation.ProcessId);
                    }
                }

                _logger.LogInformation("Buffer removed for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : config.CurentVideoStream.User_Url);
            }
            else
            {
                _logger.LogInformation("Client counter decremented for stream: {StreamUrl}. New count: {ClientCount}", setting.CleanURLs ? "url removed" : config.CurentVideoStream.User_Url, _streamInformation.ClientCount);
            }
            return true;
        }

        return false;
    }

    public void Dispose()
    {
        _streamInformations.Clear();
    }

    public int GetActiveStreamsCount()
    {
        return _streamInformations.Count;
    }

    public IEnumerable<string> GetActiveStreamUrls()
    {
        return _streamInformations.Values.Select(a => a.StreamUrl).Distinct();
    }

    public ICircularRingBuffer? GetBufferFromStreamUrl(string streamUrl)
    {
        return _streamInformations.TryGetValue(streamUrl, out var _streamInformation) ? _streamInformation.RingBuffer : null;
    }

    public StreamInformation? GetOrAdd(string streamUrl, Func<string, StreamInformation> valueFactory)
    {
        var setting = FileUtil.GetSetting();

        StreamInformation? si = _streamInformations.GetOrAdd(streamUrl, valueFactory);
        if (si != null)
        {
            AddStreamInfo(si);
        }

        return si;
    }

    public StreamerConfiguration? GetStreamerConfigurationFromID(string StreamUrl, Guid id)
    {
        var si = GetStreamInformationFromStreamUrl(StreamUrl);
        if (si is null)
        {
            return null;
        }

        return si.GetStreamConfigurations().FirstOrDefault(a => a.ClientId == id);
    }

    public StreamInformation? GetStreamInformationFromStreamUrl(string streamUrl)
    {
        if (_streamInformations.TryGetValue(streamUrl, out var _streamInformation))
        {
            return _streamInformation;
        }
        return null;
    }

    public ICollection<StreamInformation> GetStreamInformations()
    {
        return _streamInformations.Values;
    }

    public int GetStreamsCountForM3UFile(int m3uFileId)
    {
        return _streamInformations
            .Count(x => x.Value.M3UFileId == m3uFileId);
    }

    public void IncrementClientCounter(StreamerConfiguration config)
    {
        if (_streamInformations.TryGetValue(config.CurentVideoStream.User_Url, out var _streamInformation))
        {
            var setting = FileUtil.GetSetting();

            _streamInformation.AddStreamConfiguration(config);
            _logger.LogInformation("Client counter incremented for stream: {StreamUrl}. New count: {ClientCount}", setting.CleanURLs ? "url removed" : config.CurentVideoStream.User_Url, _streamInformation.ClientCount);
        }
    }

    public StreamInformation? RemoveStreamInfo(string streamUrl)
    {
        if (_streamInformations.TryRemove(streamUrl, out var _streamInformation))
        {
            return _streamInformation;
        }

        return null;
    }

    private static string? CheckProcessExists(int processId)
    {
        try
        {
            Process process = Process.GetProcessById(processId);
            Console.WriteLine($"Process with ID {processId} exists. Name: {process.ProcessName}");
            return process.ProcessName;
        }
        catch (ArgumentException)
        {
            Console.WriteLine($"Process with ID {processId} does not exist.");
            return null;
        }
    }
}
