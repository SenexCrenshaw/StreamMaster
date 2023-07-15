using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces
{
    public interface IChannelManager
    {
        void Dispose();

        List<StreamStatisticsResult> GetAllStatisticsForAllUrls();

        SingleStreamStatisticsResult GetSingleStreamStatisticsResult(string streamUrl);

        ICollection<IStreamInformation> GetStreamInformations();

        Task<Stream> RegisterClient(ClientStreamerConfiguration config);

        void SimulateStreamFailureForAll();

        void UnRegisterClient(ClientStreamerConfiguration config);
    }
}
