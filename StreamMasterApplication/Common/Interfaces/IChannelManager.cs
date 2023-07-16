using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces
{
    public interface IChannelManager
    {
        Task ChangeVideoStreamChannel(int playingVideoStreamId, int newVideoStreamId);
        Task FailClient(Guid clientId);
        void Dispose();

        List<StreamStatisticsResult> GetAllStatisticsForAllUrls();

        SingleStreamStatisticsResult GetSingleStreamStatisticsResult(string streamUrl);

        Task<Stream?> GetStream(ClientStreamerConfiguration config);

        void RemoveClient(ClientStreamerConfiguration config);

        void SimulateStreamFailure(string streamUrl);

        void SimulateStreamFailureForAll();
    }
}
