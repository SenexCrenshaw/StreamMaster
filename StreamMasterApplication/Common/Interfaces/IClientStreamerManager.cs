namespace StreamMasterApplication.Common.Interfaces
{
    public interface IClientStreamerManager
    {
        void MoveClientStreamers(IStreamHandler oldStreamHandler, IStreamHandler newStreamHandler);

        List<IClientStreamerConfiguration> GetClientStreamerConfigurationsByChannelVideoStreamId(string ChannelVideoStreamId);

        void SetClientBufferDelegate(Guid ClientId, Func<ICircularRingBuffer> func);

        int ClientCount(string ChannelVideoStreamId);

        bool CancelClient(Guid ClientId);

        void Dispose();

        void FailClient(Guid clientId);

        IClientStreamerConfiguration? GetClientStreamerConfiguration(string ChannelVideoStreamId, Guid ClientId);

        IClientStreamerConfiguration? GetClientStreamerConfiguration(Guid ClientId);

        List<IClientStreamerConfiguration> GetClientStreamerConfigurations { get; }

        List<IClientStreamerConfiguration> GetClientStreamerConfigurationFromIds(List<Guid> clientIds);

        bool HasClient(string VideoStreamId, Guid ClientId);

        void RegisterClient(IClientStreamerConfiguration clientStreamerConfiguration);

        void UnRegisterClient(Guid clientId);
    }
}