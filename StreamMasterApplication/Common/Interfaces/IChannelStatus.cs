namespace StreamMasterApplication.Common.Interfaces
{
    public interface IChannelStatus
    {
        void SetIsGlobal();
        void RemoveClientId(Guid clientId);
        void AddToClientIds(Guid clientId);
        List<Guid> GetChannelClientIds { get; }
        CancellationTokenSource ChannelWatcherToken { get; set; }
        bool FailoverInProgress { get; set; }
        bool IsGlobal { get; set; }
        int Rank { get; set; }
        IStreamHandler? StreamHandler { get; set; }
        string VideoStreamId { get; set; }
        string VideoStreamName { get; set; }
    }
}