using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces
{
    public interface IChannelManager
    {
        Task ChangeVideoStreamChannel(string playingVideoStreamId, string newVideoStreamId);

        void Dispose();

        void FailClient(Guid clientId);

        Task<List<StreamStatisticsResult>> GetAllStatisticsForAllUrls();

        Task<Stream?> GetStream(ClientStreamerConfiguration config);

        void RemoveClient(ClientStreamerConfiguration config);

        void SimulateStreamFailure(string streamUrl);

        void SimulateStreamFailureForAll();
    }
}
