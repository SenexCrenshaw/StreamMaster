using StreamMaster.PlayList.Models;

using System.Xml.Serialization;

namespace StreamMaster.PlayList;

public class NfoFileReader : INfoFileReader
{
    public Movie? ReadNfoFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path must not be null or whitespace.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The specified file does not exist.", filePath);
        }

        try
        {
            using StreamReader stream = new(filePath, System.Text.Encoding.UTF8);
            XmlSerializer serializer = new(typeof(Movie));
            Movie? nfo = (Movie?)serializer.Deserialize(stream);

            return nfo;
        }
        catch (Exception ex)
        {
            throw new IOException("An error occurred while reading the NFO file.", ex);
        }
    }

}