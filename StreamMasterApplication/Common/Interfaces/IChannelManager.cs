using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces
{
    public interface IChannelManager
    {
        void Dispose();
        List<StreamStatisticsResult> GetAllStatisticsForAllUrls();
        SingleStreamStatisticsResult GetSingleStreamStatisticsResult(string streamUrl);
        ICollection<StreamInformation> GetStreamInformations();
        Task<Stream> RegisterClient(ClientStreamerConfiguration config);
        void UnRegisterClient(ClientStreamerConfiguration config);
    }
}