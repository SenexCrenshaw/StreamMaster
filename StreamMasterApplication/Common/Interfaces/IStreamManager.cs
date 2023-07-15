using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Interfaces;

public interface IStreamManager
{
    void Dispose();

    int GetActiveStreamsCount();

    IEnumerable<string> GetActiveStreamUrls();
    SingleStreamStatisticsResult GetSingleStreamStatisticsResult(string streamUrl);
    Task<StreamInformation?> GetOrCreateBuffer(ChildVideoStreamDto childVideoStreamDto);

    ClientStreamerConfiguration? GetStreamerConfigurationFromID(string StreamUrl, Guid id);

    StreamInformation? GetStreamInformationFromStreamUrl(string streamUrl);

    ICollection<StreamInformation> GetStreamInformations();

    int GetStreamsCountForM3UFile(int m3uFileId);

    StreamInformation? RemoveStreamInfo(string streamUrl);
}

public interface IStreamManagers
{
    bool AddStreamInfo(StreamInformation streamStreamInfo);

    bool DecrementClientCounter(ClientStreamerConfiguration config);

    void Dispose();

    int GetActiveStreamsCount();

    IEnumerable<string> GetActiveStreamUrls();

    ICircularRingBuffer? GetBufferFromStreamUrl(string streamUrl);

    StreamInformation? GetOrAdd(string streamUrl, Func<string, StreamInformation> valueFactory);

    ClientStreamerConfiguration? GetStreamerConfigurationFromID(string StreamUrl, Guid id);

    StreamInformation? GetStreamInformationFromStreamUrl(string streamUrl);

    ICollection<StreamInformation> GetStreamInformations();

    int GetStreamsCountForM3UFile(int m3uFileId);

    void IncrementClientCounter(ClientStreamerConfiguration config);

    StreamInformation? RemoveStreamInfo(string streamUrl);
}
