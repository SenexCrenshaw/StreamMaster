namespace StreamMasterApplication.Common.Interfaces;

public interface IStreamManager
{
    void Dispose();
    Task<IStreamHandler?> GetOrCreateStreamController(ChildVideoStreamDto childVideoStreamDto, string videoStreamId, string videoStreamName, int rank);

    IStreamHandler? GetStreamInformationFromStreamUrl(string streamUrl);

    ICollection<IStreamHandler> GetStreamInformations();

    int GetStreamsCountForM3UFile(int m3uFileId);

    IStreamHandler? Stop(string streamUrl);
}
