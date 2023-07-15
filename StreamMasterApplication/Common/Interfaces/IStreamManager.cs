using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Interfaces;

public interface IStreamManager
{
    void Dispose();

    Task<StreamInformation?> GetOrCreateBuffer(ChildVideoStreamDto childVideoStreamDto);

    SingleStreamStatisticsResult GetSingleStreamStatisticsResult(string streamUrl);

    ICollection<StreamInformation> GetStreamInformations();

    StreamInformation? Stop(string streamUrl);
}
