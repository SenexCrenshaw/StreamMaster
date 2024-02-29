﻿namespace StreamMaster.Domain.Repository
{
    public interface IRepositoryWrapper
    {
        ISMChannelRepository SMChannel { get; }
        ISMStreamRepository SMStream { get; }
        IEPGFileRepository EPGFile { get; }
        IChannelGroupRepository ChannelGroup { get; }
        IStreamGroupRepository StreamGroup { get; }
        IM3UFileRepository M3UFile { get; }
        IVideoStreamRepository VideoStream { get; }
        IVideoStreamLinkRepository VideoStreamLink { get; }
        IStreamGroupChannelGroupRepository StreamGroupChannelGroup { get; }
        IStreamGroupVideoStreamRepository StreamGroupVideoStream { get; }
        Task<int> SaveAsync();
    }
}