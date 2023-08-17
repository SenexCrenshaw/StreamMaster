namespace StreamMasterDomain.Repository;

public class M3UFile : AutoUpdateEntity
{
    public M3UFile()
    {
        FileExtension = FileDefinitions.M3U.FileExtension;
        SMFileType = FileDefinitions.M3U.SMFileType;
    }

    public int MaxStreamCount { get; set; }
    public int StartingChannelNumber { get; set; }
    public int StationCount { get; set; }

    public DateTime LastWrite()
    {
        string fileName = Path.Combine(FileDefinitions.M3U.DirectoryLocation, Source);
        DateTime lastWrite = File.GetLastWriteTime(fileName);

        return lastWrite;
    }

    public async Task<List<VideoStream>?> GetM3U()
    {
        using Stream dataStream = FileUtil.GetFileDataStream(Path.Combine(FileDefinitions.M3U.DirectoryLocation, Source));
        return IPTVExtensions.ConvertToVideoStream(dataStream, Id, Name);
    }
}
