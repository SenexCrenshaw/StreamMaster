using StreamMaster.Domain.Configuration;
using StreamMaster.PlayList.Models;

using System.Xml.Serialization;

namespace StreamMaster.PlayList;

public record CustomStreamNfo(string VideoFileName, MovieNfo MovieNfo);


public class CustomPlayList
{
    public string Name { get; set; } = string.Empty;
    public MovieNfo? FolderNfo { get; set; }
    public List<CustomStreamNfo> CustomStreamNfos { get; set; } = [];
}
public class CustomPlayListBuilder(INfoFileReader nfoFileReader) : ICustomPlayListBuilder
{
    public List<CustomPlayList> GetNFOs()
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

        string[] subDirs = Directory.GetDirectories(BuildInfo.CustomPlayListFolder);

        foreach (string subDir in subDirs)
        {
            CustomPlayList customPlayList = new();
            string[] mp4Files = Directory.GetFiles(subDir, "*.mp4");
            string[] tsFiles = Directory.GetFiles(subDir, "*.ts");
            string? dirName = Path.GetFileName(subDir);
            customPlayList.Name = dirName ?? "Unknown";

            List<string> allFiles = [.. mp4Files.Concat(tsFiles).OrderBy(file => file)];

            string folderNfoFile = Path.Combine(subDir, "folder.nfo");

            if (File.Exists(folderNfoFile))
            {
                MovieNfo? nfo = nfoFileReader.ReadNfoFile(folderNfoFile);
                customPlayList.FolderNfo = nfo;
            }

            foreach (string? file in allFiles)
            {
                string nfoFile = Path.ChangeExtension(file, ".nfo");
                if (File.Exists(nfoFile))
                {
                    MovieNfo? nfo = nfoFileReader.ReadNfoFile(nfoFile);
                    if (!string.IsNullOrEmpty(dirName) && nfo != null)
                    {
                        CustomStreamNfo customStreamNfo = new(file, nfo);
                        customPlayList.CustomStreamNfos.Add(customStreamNfo);
                    }
                }
            }
            ret.Add(customPlayList);
        }
        return ret;
    }

    public static void WriteNfoFile(string filePath, MovieNfo movieNfo)
    {
        try
        {
            using StreamWriter stream = new(filePath, false, System.Text.Encoding.UTF8);
            XmlSerializer serializer = new(typeof(MovieNfo));
            serializer.Serialize(stream, movieNfo);
        }
        catch (Exception ex)
        {
            throw new IOException("An error occurred while writing the NFO file.", ex);
        }
    }
}
