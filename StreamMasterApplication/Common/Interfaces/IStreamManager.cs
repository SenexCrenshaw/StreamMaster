namespace StreamMasterApplication.Common.Interfaces;

public interface IStreamManager
{
    void Dispose();
    Task<IStreamHandler?> GetOrCreateStreamHandler(ChildVideoStreamDto childVideoStreamDto, int rank);
    IStreamHandler? GetStreamInformationFromStreamUrl(string streamUrl);

    ICollection<IStreamHandler> GetStreamInformations();

    int GetStreamsCountForM3UFile(int m3uFileId);

    IStreamHandler? Stop(string streamUrl);
}
