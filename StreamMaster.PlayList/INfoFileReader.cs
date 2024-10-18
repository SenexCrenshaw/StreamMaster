using StreamMaster.PlayList.Models;

namespace StreamMaster.PlayList
{
    public interface INfoFileReader
    {
        Movie? ReadNfoFile(string filePath);
    }
}