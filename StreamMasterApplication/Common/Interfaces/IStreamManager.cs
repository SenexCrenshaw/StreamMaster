using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Interfaces;

public interface IStreamManager
{
    void Dispose();

    Task<IStreamInformation?> GetOrCreateBuffer(ChildVideoStreamDto childVideoStreamDto, string videoStreamId, string videoStreamName, int rank);

    SingleStreamStatisticsResult GetSingleStreamStatisticsResult(string streamUrl);

    IStreamInformation? GetStreamInformationFromStreamUrl(string streamUrl);

    ICollection<IStreamInformation> GetStreamInformations();

    int GetStreamsCountForM3UFile(int m3uFileId);

    IStreamInformation? Stop(string streamUrl);
}
