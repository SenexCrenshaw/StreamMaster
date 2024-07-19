using StreamMaster.Domain.Common;
using StreamMaster.Domain.Configuration;
using StreamMaster.PlayList.Models;

using System.Xml.Serialization;

namespace StreamMaster.PlayList;

public record CustomStreamNfo(string VideoFileName, Movie Movie);

public class CustomPlayListBuilder(INfoFileReader nfoFileReader) : ICustomPlayListBuilder
{
    private static readonly DateTime SequenceStartTime = new(2024, 7, 19, 0, 0, 0);

    public CustomPlayList? GetCustomPlayList(string Name)
    {
        CustomPlayList? ret = GetCustomPlayLists().FirstOrDefault(x => x.Name == Name);
        ret ??= GetCustomPlayLists().Find(x => FileUtil.EncodeToBase64(x.Name) == Name);
        return ret;
    }

    public List<CustomPlayList> GetCustomPlayLists()
    {
        List<CustomPlayList> ret = [];

        if (string.IsNullOrWhiteSpace(BuildInfo.CustomPlayListFolder))
        {
            return ret;
        }

        if (!Directory.Exists(BuildInfo.CustomPlayListFolder))
        {
            return ret;
        }

        string[] folders = Directory.GetDirectories(BuildInfo.CustomPlayListFolder);

        foreach (string folder in folders)
        {
            CustomPlayList customPlayList = new();
            string folderName = Path.GetFileNameWithoutExtension(folder);
            string folderNFOFileName = Path.ChangeExtension(folderName, ".nfo");
            string folderNFOFile = Path.Combine(folder, folderNFOFileName);
            if (!File.Exists(folderNFOFile))
            {
                continue;
            }

            customPlayList.FolderNfo = nfoFileReader.ReadNfoFile(folderNFOFile);
            customPlayList.Name = folderName;

            string folderLogoFile = GetFolderLogoInDirectory(folder);
            customPlayList.Logo = folderLogoFile;

            if (customPlayList.FolderNfo == null)
            {
                continue;
            }

            string[] videoFolders = Directory.GetDirectories(folder);

            foreach (string videoFolder in videoFolders)
            {
                string file = GetFirstVideoFileInDirectory(videoFolder);

                string fileName = Path.GetFileNameWithoutExtension(file);
                string fileNFOFileName = Path.ChangeExtension(fileName, ".nfo");
                string fileNFOFile = Path.Combine(videoFolder, fileNFOFileName);

                if (!File.Exists(fileNFOFile))
                {
                    continue;
                }

                Movie? fileNfo = nfoFileReader.ReadNfoFile(fileNFOFile);
                if (fileNfo == null)
                {
                    continue;
                }

                CustomStreamNfo customStreamNfo = new(file, fileNfo);
                customPlayList.CustomStreamNfos.Add(customStreamNfo);
            }
            ret.Add(customPlayList);
        }
        return ret;
    }

    public static string GetFirstVideoFileInDirectory(string dir)
    {
        string[] files = Directory.GetFiles(dir);
        foreach (string file in files)
        {
            if (file.EndsWith(".mp4") || file.EndsWith(".mkv") || file.EndsWith(".avi"))
            {
                return file;
            }
        }
        return string.Empty;
    }

    public static string GetFolderLogoInDirectory(string dir)
    {
        string logoName = Path.GetFileNameWithoutExtension(dir);
        string[] files = Directory.GetFiles(dir);
        foreach (string fullFileName in files)
        {
            string file = Path.GetFileName(fullFileName);

            if (file.Equals(logoName + ".png") || file.Equals(logoName + ".jpg"))
            {
                return file;
            }
        }
        return string.Empty;
    }

    public static void WriteNfoFile(string filePath, Movie movieNfo)
    {
        try
        {
            using StreamWriter stream = new(filePath, false, System.Text.Encoding.UTF8);
            XmlSerializer serializer = new(typeof(Movie));
            serializer.Serialize(stream, movieNfo);
        }
        catch (Exception ex)
        {
            throw new IOException("An error occurred while writing the NFO file.", ex);
        }
    }

    public (string VideoFileName, int SecondsIn) GetCurrentVideoAndElapsedSeconds(string customPlayListName)
    {
        CustomPlayList? customPlayList = GetCustomPlayList(customPlayListName);
        if (customPlayList == null)
        {
            throw new ArgumentException($"Custom playlist with name {customPlayListName} not found.");
        }

        // Calculate the total length of the playlist
        int totalLength = customPlayList.CustomStreamNfos.Sum(nfo => nfo.Movie.Runtime);

        // Calculate elapsed seconds since the sequence start time
        double elapsedSeconds = (DateTime.Now - SequenceStartTime).TotalSeconds % totalLength;

        int accumulatedTime = 0;

        // Find the current video and the seconds into it
        foreach (CustomStreamNfo customStreamNfo in customPlayList.CustomStreamNfos)
        {
            int videoLength = customStreamNfo.Movie.Runtime;

            if (elapsedSeconds < accumulatedTime + videoLength)
            {
                int secondsIn = (int)(elapsedSeconds - accumulatedTime);
                return (customStreamNfo.VideoFileName, secondsIn);
            }

            accumulatedTime += videoLength;
        }

        // If for some reason no video was found (though this should not happen), return the first video
        CustomStreamNfo firstVideo = customPlayList.CustomStreamNfos.First();
        return (firstVideo.VideoFileName, (int)elapsedSeconds);
    }


}
