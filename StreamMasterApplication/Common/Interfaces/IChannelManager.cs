using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces
{
    public interface IChannelManager : IDisposable
    {
        Task ChangeVideoStreamChannel(string playingVideoStreamId, string newVideoStreamId);

        void FailClient(Guid clientId);

        Task<Stream?> GetStream(ClientStreamerConfiguration config);

        Task RemoveClient(ClientStreamerConfiguration config);

        void SimulateStreamFailure(string streamUrl);

        void SimulateStreamFailureForAll();
    }
}
