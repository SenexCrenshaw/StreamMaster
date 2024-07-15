using StreamMaster.PlayList.Models;

namespace StreamMaster.PlayList
{
    public interface INfoFileReader
    {
        MovieNfo? ReadNfoFile(string filePath);
    }
}