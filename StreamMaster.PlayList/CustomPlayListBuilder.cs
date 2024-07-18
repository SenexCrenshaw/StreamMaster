using StreamMaster.Domain.Common;
using StreamMaster.Domain.Configuration;
using StreamMaster.PlayList.Models;

using System.Xml.Serialization;

namespace StreamMaster.PlayList;
public record CustomStreamNfo(string VideoFileName, Movie Movie);

public class CustomPlayList
{
    public string Name { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
    public Movie? FolderNfo { get; set; }
    public List<CustomStreamNfo> CustomStreamNfos { get; set; } = [];
}

public class CustomPlayListBuilder(INfoFileReader nfoFileReader) : ICustomPlayListBuilder
{
    public CustomPlayList? GetCustomPlayList(string Name)
    {

        CustomPlayList? ret = GetCustomPlayLists().FirstOrDefault(x => FileUtil.EncodeToBase64(x.Name) == Name);
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
            if (customPlayList.FolderNfo == null)
            {
                continue;
            }

            string[] videoFolders = Directory.GetDirectories(folder);

            foreach (string videoFolder in videoFolders)
            {
                var file = GetFirstVideoFileInDirectory(videoFolder);

                string fileName = Path.GetFileNameWithoutExtension(file);
                string fileNFOFileName = Path.ChangeExtension(fileName, ".nfo");
                string fileNFOFile = Path.Combine(videoFolder, fileNFOFileName);

                if (!File.Exists(fileNFOFile))
                {
                    continue;
                }

                var fileNfo = nfoFileReader.ReadNfoFile(fileNFOFile);
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
}
