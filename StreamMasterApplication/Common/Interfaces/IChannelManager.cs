using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces
{
    public interface IChannelManager
    {
        Task ChangeVideoStreamChannel(string playingVideoStreamId, string newVideoStreamId);

        void Dispose();

        Task FailClient(Guid clientId);

        Task<List<StreamStatisticsResult>> GetAllStatisticsForAllUrls();

        SingleStreamStatisticsResult GetSingleStreamStatisticsResult(string streamUrl);

        Task<Stream?> GetStream(ClientStreamerConfiguration config);

        void RemoveClient(ClientStreamerConfiguration config);

        void SimulateStreamFailure(string streamUrl);

        void SimulateStreamFailureForAll();
    }
}
