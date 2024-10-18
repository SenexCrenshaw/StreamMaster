namespace StreamMaster.Application.Interfaces
{
    public interface IM3UToSMStreamsService
    {
        IAsyncEnumerable<SMStream?> GetSMStreamsFromM3U(M3UFile m3UFile);
    }
}