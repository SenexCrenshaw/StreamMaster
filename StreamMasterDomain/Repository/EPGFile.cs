using StreamMasterDomain.Repository.EPG;

using System.Xml.Serialization;

namespace StreamMasterDomain.Repository;

public class EPGFile : AutoUpdateEntity
{
    public EPGFile()
    {
        //DirectoryLocation = FileDefinitions.EPG.DirectoryLocation;
        FileExtension = FileDefinitions.EPG.FileExtension;
        SMFileType = FileDefinitions.EPG.SMFileType;
    }
    public DateTime LastWrite()
    {
        string fileName = Path.Combine(FileDefinitions.EPG.DirectoryLocation, Source);
        DateTime lastWrite = File.GetLastWriteTime(fileName);

        return lastWrite;
    }
    public int ChannelCount { get; set; }
    public int EPGRank { get; set; }
    public int ProgrammeCount { get; set; }
    public float TimeShift { get; set; } = 0;

    public static Tv? GetTVFromBody(string body)
    {
        try
        {
            using StringReader reader = new(body);
            XmlSerializer serializer = new(typeof(Tv));
            object? result = serializer.Deserialize(reader);

            return (Tv?)result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return null;
    }

    public async Task<Tv?> GetTV()
    {
        try
        {
            string body = await FileUtil.GetFileData(Path.Combine(FileDefinitions.EPG.DirectoryLocation, Source)).ConfigureAwait(false);

            return GetTVFromBody(body);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return null;
    }
}
