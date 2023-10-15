using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces;

public interface IStreamManager
{
    void Dispose();

    Task<IStreamController?> GetOrCreateStreamController(ChildVideoStreamDto childVideoStreamDto, string videoStreamId, string videoStreamName, int rank);

    SingleStreamStatisticsResult GetSingleStreamStatisticsResult(string streamUrl);

    IStreamController? GetStreamInformationFromStreamUrl(string streamUrl);

    ICollection<IStreamController> GetStreamInformations();

    int GetStreamsCountForM3UFile(int m3uFileId);

    IStreamController? Stop(string streamUrl);
}
