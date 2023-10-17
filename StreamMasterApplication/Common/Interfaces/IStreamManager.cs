namespace StreamMasterApplication.Common.Interfaces;

public interface IStreamManager
{
    void Dispose();
    Task<IStreamHandler?> GetOrCreateStreamHandler(ChildVideoStreamDto childVideoStreamDto, int rank, CancellationToken cancellation = default);
    IStreamHandler? GetStreamInformationFromStreamUrl(string streamUrl);
    IStreamHandler? GetStreamHandler(string videoStreamId);

    ICollection<IStreamHandler> GetStreamInformations();
    List<IStreamHandler> GetStreamHandlers();
    int GetStreamsCountForM3UFile(int m3uFileId);

    IStreamHandler? Stop(string videoStreamId);
}
