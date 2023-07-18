namespace StreamMasterDomain.Entities;

public class M3UFile : AutoUpdateEntity
{
    public M3UFile()
    {
        //DirectoryLocation = FileDefinitions.M3U.DirectoryLocation;
        FileExtension = FileDefinitions.M3U.FileExtension;
        SMFileType = FileDefinitions.M3U.SMFileType;
    }

    public int MaxStreamCount { get; set; }
    public int StartingChannelNumber { get; set; }
    public int StationCount { get; set; }

    public async Task<List<VideoStream>?> GetM3U()
    {
        string body = await FileUtil.GetFileData(Path.Combine(FileDefinitions.M3U.DirectoryLocation, Source)).ConfigureAwait(false);

        return IPTVExtensions.ConvertToVideoStream(body, Id, Name);
    }
}
