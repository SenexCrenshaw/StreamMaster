namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IM3U8Generator
    {
        string CreateM3U8Content(List<string> tsFiles, bool insertIntros = false);
    }
}