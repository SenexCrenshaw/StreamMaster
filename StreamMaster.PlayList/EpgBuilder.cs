using StreamMaster.PlayList.Models;

namespace StreamMaster.PlayList;

public class EpgBuilder
{
    private readonly NfoFileReader _nfoFileReader;

    public EpgBuilder(NfoFileReader nfoFileReader)
    {
        _nfoFileReader = nfoFileReader;
    }

    public async Task<List<MovieNfo>> BuildEpgAsync(string directoryPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            throw new ArgumentException("Directory path must not be null or whitespace.", nameof(directoryPath));
        }

        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException("The specified directory does not exist.");
        }

        List<MovieNfo> epgEntries = [];

        List<string> nfoFiles = Directory.GetFiles(directoryPath, "*.nfo")
                                .OrderBy(file => file)
                                .ToList();

        foreach (string? nfoFile in nfoFiles)
        {
            MovieNfo? epgEntry = await _nfoFileReader.ReadNfoFileAsync(nfoFile, cancellationToken);
            epgEntries.Add(epgEntry!);
        }

        return epgEntries;
    }
}