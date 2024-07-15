using StreamMaster.PlayList.Models;

using System.Xml.Serialization;

namespace StreamMaster.PlayList;

public class NfoFileReader
{
    public async Task<MovieNfo?> ReadNfoFileAsync(string filePath, CancellationToken cancellationToken = default)
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
            using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read);
            XmlSerializer serializer = new(typeof(MovieNfo));
            MovieNfo? kodiMovie = (MovieNfo?)serializer.Deserialize(stream);
            return kodiMovie;
        }
        catch (Exception ex)
        {
            throw new IOException("An error occurred while reading the NFO file.", ex);
        }
    }
}
