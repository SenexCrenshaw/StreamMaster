﻿using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces;

public interface IStreamController
{
    int ClientCount { get; }
    bool FailoverInProgress { get; set; }
    int M3UFileId { get; set; }
    bool M3UStream { get; set; }
    int MaxStreams { get; set; }
    int ProcessId { get; set; }
    ICircularRingBuffer RingBuffer { get; }
    Task StreamingTask { get; set; }
    string StreamUrl { get; set; }
    CancellationTokenSource VideoStreamingCancellationToken { get; set; }

    void Dispose();

    ClientStreamerConfiguration? GetClientStreamerConfiguration(Guid ClientId);

    List<ClientStreamerConfiguration> GetClientStreamerConfigurations();

    void RegisterClientStreamer(ClientStreamerConfiguration streamerConfiguration);

    void Stop();

    bool UnRegisterClientStreamer(ClientStreamerConfiguration streamerConfiguration);
}