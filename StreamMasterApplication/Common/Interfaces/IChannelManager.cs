using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces
{
    public interface IChannelManager
    {
        void Dispose();

        List<StreamStatisticsResult> GetAllStatisticsForAllUrls();

        SingleStreamStatisticsResult GetSingleStreamStatisticsResult(string streamUrl);

        Task<Stream?> GetStream(ClientStreamerConfiguration config);

        ICollection<IStreamInformation> GetStreamInformations();

        Task<Stream> RegisterClient(ClientStreamerConfiguration config);

        void RemoveClient(ClientStreamerConfiguration config);

        void SimulateStreamFailure(string streamUrl);

        void SimulateStreamFailureForAll();

        void UnRegisterClient(ClientStreamerConfiguration config);
    }
}
