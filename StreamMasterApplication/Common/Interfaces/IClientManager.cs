using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces
{
    public interface IClientManager
    {
        List<ClientStreamerConfiguration> GetClientStreamerConfigurationByVideoStreamId(string VideoStreamId);
        void SetClientBufferDelegate(Guid ClientId, Func<ICircularRingBuffer> func);
        int ClientCount { get; }
        bool CancelClient(Guid clientID);
        void Dispose();
        ClientStreamerConfiguration? GetClientStreamerConfiguration(Guid clientID);
        List<ClientStreamerConfiguration>? GetClientStreamerConfigurations { get; }
        List<ClientStreamerConfiguration> GetClientStreamerConfigurationFromIds(List<Guid> clientIds);
        bool HasClient(Guid clientId);
        void RegisterClient(ClientStreamerConfiguration clientStreamerConfiguration);
        void UnRegisterClient(Guid clientId);
    }
}