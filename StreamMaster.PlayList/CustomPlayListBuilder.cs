using System.Xml.Serialization;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.Configuration;
using StreamMaster.PlayList.Models;
namespace StreamMaster.PlayList;
public class CustomPlayListBuilder : ICustomPlayListBuilder
{
    private readonly bool _generateMissingNfoFiles = false;
    private readonly bool _alwaysCreateFirstFolderNfo = false; // New setting to always create the first folder .nfo

    private static readonly DateTime SequenceStartTime = new(2024, 7, 18, 0, 0, 0);
    private readonly ILogger<CustomPlayListBuilder> _logger;
    private readonly INfoFileReader _nfoFileReader;
    private readonly IMemoryCache _memoryCache;

    private readonly FileSystemWatcher _fileSystemWatcher;
    private const string CustomPlayListCacheKey = "CustomPlayLists";

    public CustomPlayListBuilder(
        ILogger<CustomPlayListBuilder> logger,
        INfoFileReader nfoFileReader,
        IMemoryCache memoryCache)
    {
        _logger = logger;
        _nfoFileReader = nfoFileReader;
        _memoryCache = memoryCache;

        _fileSystemWatcher = new FileSystemWatcher(BuildInfo.CustomPlayListFolder, "*.nfo")
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
        };
        _fileSystemWatcher.Changed += OnCustomPlayListFolderChanged;
        _fileSystemWatcher.Created += OnCustomPlayListFolderChanged;
        _fileSystemWatcher.Deleted += OnCustomPlayListFolderChanged;
        _fileSystemWatcher.Renamed += OnCustomPlayListFolderChanged;
        _fileSystemWatcher.EnableRaisingEvents = true;
    }

    private void OnCustomPlayListFolderChanged(object sender, FileSystemEventArgs e)
    {
        _memoryCache.Remove(CustomPlayListCacheKey);
    }

    public CustomPlayList? GetCustomPlayList(string name)
    {
        List<CustomPlayList> customPlayLists = GetCustomPlayLists();
        return customPlayLists.Find(x => x.Name == name) ?? customPlayLists.Find(x => FileUtil.EncodeToMD5(x.Name) == name);
    }

    public string? GetCustomPlayListLogoFromFileName(string FileName)
    {
        string? dir = Path.GetDirectoryName(FileName);
        if (string.IsNullOrEmpty(dir))
        {
            dir = Path.Combine(BuildInfo.CustomPlayListFolder, FileName);
        }
        if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
        {
            return null;
        }
        string[] files = Directory.GetFiles(dir);
        string? logo = files.FirstOrDefault(file => file.EndsWith("poster.png") || file.EndsWith("poster.jpg"));
        return logo;
    }

    public CustomPlayList? GetCustomForFilePlayList(string name)
    {
        if (name.Contains('|'))
        {
            name = name.Split('|')[1];
        }
        return GetCustomPlayLists().Find(a => a.CustomStreamNfos.Any(b => b.Movie.Title == name));
    }

    public List<CustomPlayList> GetCustomPlayLists()
    {
        return LoadCustomPlayLists();
    }

    private List<CustomPlayList> LoadCustomPlayLists()
    {
        List<CustomPlayList> ret = [];
        if (string.IsNullOrWhiteSpace(BuildInfo.CustomPlayListFolder) || !Directory.Exists(BuildInfo.CustomPlayListFolder))
        {
            return ret;
        }

        string[] folders = Directory.GetDirectories(BuildInfo.CustomPlayListFolder);
        bool isFirstFolder = true; // Track whether we are processing the first folder

        foreach (string folder in folders)
        {
            CustomPlayList customPlayList = new();
            string folderName = Path.GetFileNameWithoutExtension(folder);
            string folderNFOFileName = Path.ChangeExtension(folderName, ".nfo");
            string folderNFOFile = Path.Combine(folder, folderNFOFileName);

            if (!File.Exists(folderNFOFile))
            {
                // Always create the .nfo for the first folder
                if (isFirstFolder && _alwaysCreateFirstFolderNfo)
                {
                    customPlayList.FolderNfo = CreateDefaultNfoFile(folderNFOFile, folderName);
                    isFirstFolder = false; // First folder processed, update the flag
                }
                else if (_generateMissingNfoFiles)
                {
                    // For subsequent folders, create .nfo if the setting is enabled
                    customPlayList.FolderNfo = CreateDefaultNfoFile(folderNFOFile, folderName);
                }
                else
                {
                    continue; // Skip if no folder .nfo file and no need to create it
                }
            }
            else
            {
                customPlayList.FolderNfo = _nfoFileReader.ReadNfoFile(folderNFOFile);
                isFirstFolder = false; // Mark as first folder processed if the nfo existed
            }

            customPlayList.Name = folderName;
            customPlayList.Logo = GetFolderLogoInDirectory(folder);

            if (customPlayList.FolderNfo == null)
            {
                continue;
            }

            string[] videoFolders = Directory.GetDirectories(folder);
            List<Actor> allActors = []; // To collect unique actors
            List<string> allGenres = []; // To collect unique genres
            List<string> allDirectors = []; // To collect unique directors
            List<string> allCredits = []; // To collect unique credits (cast/crew)
            List<string> allArtwork = []; // To collect artwork paths (posters, banners)
            List<string> allTrailers = []; // To collect trailer paths
            int totalDuration = 0; // To sum the duration

            foreach (string videoFolder in videoFolders)
            {
                string file = GetFirstVideoFileInDirectory(videoFolder);

                if (string.IsNullOrEmpty(file))
                {
                    continue;
                }

                string fileName = Path.GetFileNameWithoutExtension(file);
                string fileNFOFileName = Path.ChangeExtension(fileName, ".nfo");
                string fileNFOFile = Path.Combine(videoFolder, fileNFOFileName);

                if (File.Exists(fileNFOFile))
                {
                    Movie? fileNfo = _nfoFileReader.ReadNfoFile(fileNFOFile);

                    if (fileNfo != null)
                    {
                        // Add actors to the list, avoiding any duplicates
                        if (fileNfo.Actors != null)
                        {
                            foreach (Actor actor in fileNfo.Actors)
                            {
                                if (!allActors.Contains(actor))
                                {
                                    allActors.Add(actor);
                                }
                            }
                        }

                        // Add genres to the list, avoiding duplicates
                        if (fileNfo.Genres != null)
                        {
                            foreach (string genre in fileNfo.Genres)
                            {
                                if (!allGenres.Contains(genre))
                                {
                                    allGenres.Add(genre);
                                }
                            }
                        }

                        // Add directors to the list, avoiding duplicates
                        if (fileNfo.Directors != null)
                        {
                            foreach (string director in fileNfo.Directors)
                            {
                                if (!allDirectors.Contains(director))
                                {
                                    allDirectors.Add(director);
                                }
                            }
                        }

                        // Add credits (cast/crew) to the list, avoiding duplicates
                        if (fileNfo.Credits != null)
                        {
                            foreach (string credit in fileNfo.Credits)
                            {
                                if (!allCredits.Contains(credit))
                                {
                                    allCredits.Add(credit);
                                }
                            }
                        }

                        // Add artwork (posters, banners) to the list, avoiding duplicates
                        string poster = GetFirstArtworkInDirectory(videoFolder);
                        if (!string.IsNullOrEmpty(poster) && !allArtwork.Contains(poster))
                        {
                            allArtwork.Add(poster);
                            fileNfo.Thumb ??= new();
                            fileNfo.Thumb.Preview = poster;
                            fileNfo.Thumb.Text = poster;
                        }

                        // Add trailers to the list, avoiding duplicates
                        if (fileNfo.Trailers != null)
                        {
                            foreach (string trailer in fileNfo.Trailers)
                            {
                                if (!allTrailers.Contains(trailer))
                                {
                                    allTrailers.Add(trailer);
                                }
                            }
                        }

                        // Add the duration to the total duration
                        if (fileNfo.Runtime > 0)
                        {
                            totalDuration += fileNfo.Runtime;
                        }

                        customPlayList.CustomStreamNfos.Add(new CustomStreamNfo(file, fileNfo));
                    }
                }
            }

            // Update folder NFO file with collected actors, genres, directors, credits, artwork, trailers, and total duration
            if (_generateMissingNfoFiles)
            {
                UpdateFolderNfoFile(folderNFOFile, customPlayList.FolderNfo, allActors, allGenres, allDirectors, allCredits, allArtwork, allTrailers, totalDuration);
            }

            ret.Add(customPlayList);
        }

        return ret;
    }

    private void UpdateFolderNfoFile(
        string folderNfoFile,
        Movie folderNfo,
        List<Actor> actors,
        List<string> genres,
        List<string> directors,
        List<string> credits,
        List<string> artworkPaths,
        List<string> trailerPaths,
        int totalDuration)
    {
        // Update the folder NFO with the collected data
        folderNfo.Actors ??= []; // Ensure actors list is not null
        folderNfo.Genres ??= []; // Ensure genres list is not null
        folderNfo.Directors ??= []; // Ensure directors list is not null
        folderNfo.Trailers ??= []; // Ensure trailers list is not null

        foreach (Actor actor in actors)
        {
            if (!folderNfo.Actors.Contains(actor)) // Avoid duplicates
            {
                folderNfo.Actors.Add(actor);
            }
        }

        foreach (string genre in genres)
        {
            if (!folderNfo.Genres.Contains(genre)) // Avoid duplicates
            {
                folderNfo.Genres.Add(genre);
            }
        }

        foreach (string director in directors)
        {
            if (!folderNfo.Directors.Contains(director)) // Avoid duplicates
            {
                folderNfo.Directors.Add(director);
            }
        }

        folderNfo.Credits = [.. credits.Distinct()]; // Add unique credits

        folderNfo.Artworks = [.. artworkPaths.Distinct()]; // Add artwork paths without duplicates

        folderNfo.Trailers = [.. trailerPaths.Distinct()]; // Add trailer paths without duplicates

        folderNfo.Runtime = totalDuration; // Set the total duration

        // Write back the updated NFO file
        WriteNfoFileAsync(folderNfoFile, folderNfo).ConfigureAwait(false);
        _logger.LogInformation("Updated folder NFO file at {folderNfoFile} with actors, genres, directors, credits, artwork, trailers, and total duration", folderNfoFile);
    }

    private static string GetFirstArtworkInDirectory(string dir)
    {
        string[] files = Directory.GetFiles(dir);
        return files.FirstOrDefault(file => file.EndsWith("poster.png") || file.EndsWith("poster.jpg") || file.EndsWith("banner.jpg")) ?? string.Empty;
    }

    private Movie CreateDefaultNfoFile(string nfoFilePath, string folderName)
    {
        Movie defaultNfo = new()
        {
            Title = folderName,
            Id = Guid.NewGuid().ToString(),
            Runtime = 120, // Set a default runtime (e.g., 2 hours)
            Year = DateTime.Now.Year.ToString(), // Use current year
            Genres = ["Unknown"]
        };

        WriteNfoFileAsync(nfoFilePath, defaultNfo).ConfigureAwait(false);
        _logger.LogInformation("Created default NFO file at {nfoFilePath}", nfoFilePath);

        return defaultNfo;
    }

    public static string GetFirstVideoFileInDirectory(string dir)
    {
        string[] files = Directory.GetFiles(dir);
        return files.FirstOrDefault(file => file.EndsWith(".mp4") || file.EndsWith(".mkv") || file.EndsWith(".avi")) ?? string.Empty;
    }

    public static string GetFolderLogoInDirectory(string dir)
    {
        string logoName = Path.GetFileNameWithoutExtension(dir);
        string[] files = Directory.GetFiles(dir);
        return files.FirstOrDefault(file => file.EndsWith($"{logoName}.png") || file.EndsWith($"{logoName}.jpg")) ?? string.Empty;
    }

    public static async Task WriteNfoFileAsync(string filePath, Movie movieNfo)
    {
        try
        {
            await using StreamWriter stream = new(filePath, false, System.Text.Encoding.UTF8);
            XmlSerializer serializer = new(typeof(Movie));
            serializer.Serialize(stream, movieNfo);
        }
        catch (Exception ex)
        {
            throw new IOException("An error occurred while writing the NFO file.", ex);
        }
    }

    public List<(Movie Movie, DateTime StartTime, DateTime EndTime)> GetMoviesForPeriod(string customPlayListName, DateTime startDate, int days)
    {
        CustomPlayList customPlayList = GetCustomPlayList(customPlayListName) ?? throw new ArgumentException($"Custom playlist with name {customPlayListName} not found.");

        DateTime periodStartTime = new DateTime(startDate.Year, startDate.Month, startDate.Day, 23, 0, 0).AddDays(-1);
        DateTime periodEndTime = periodStartTime.AddDays(days + 1);

        int totalLength = customPlayList.CustomStreamNfos.Where(nfo => nfo.Movie.Runtime >= 1).Sum(nfo => nfo.Movie.Runtime * 60);
        double totalPeriodSeconds = (periodEndTime - periodStartTime).TotalSeconds;

        List<(Movie Movie, DateTime StartTime, DateTime EndTime)> moviesForPeriod = [];

        double currentSecond = (periodStartTime - SequenceStartTime).TotalSeconds % totalLength;
        if (currentSecond < 0)
        {
            currentSecond += totalLength; // Correct for negative modulo results
        }

        DateTime currentPeriodTime = periodStartTime.AddSeconds(-currentSecond);

        while (totalPeriodSeconds > 0)
        {
            foreach (CustomStreamNfo customStreamNfo in customPlayList.CustomStreamNfos)
            {
                if (customStreamNfo.Movie.Runtime < 1)
                {
                    continue; // Skip movies less than 1 minute
                }

                int videoLengthInSeconds = customStreamNfo.Movie.Runtime * 60;

                if (currentSecond < videoLengthInSeconds)
                {
                    DateTime movieStartTime = currentPeriodTime.AddSeconds(currentSecond);
                    DateTime movieEndTime = movieStartTime.AddSeconds(videoLengthInSeconds - currentSecond);

                    if (movieEndTime > periodEndTime)
                    {
                        movieEndTime = periodEndTime;
                    }

                    moviesForPeriod.Add((customStreamNfo.Movie, movieStartTime, movieEndTime));

                    totalPeriodSeconds -= videoLengthInSeconds - currentSecond;
                    if (totalPeriodSeconds <= 0)
                    {
                        break;
                    }

                    currentPeriodTime = movieEndTime;
                    currentSecond = 0; // After the first video, we start from 0 for subsequent videos
                }
                else
                {
                    currentSecond -= videoLengthInSeconds;
                }
            }
            currentSecond = 0; // Loop back to the beginning of the playlist
        }

        return moviesForPeriod;
    }

    public (CustomStreamNfo StreamNfo, int SecondsIn) GetCurrentVideoAndElapsedSeconds(string customPlayListName)
    {
        CustomPlayList customPlayList = GetCustomPlayList(customPlayListName) ?? throw new ArgumentException($"Custom playlist with name {customPlayListName} not found.");

        int totalLength = customPlayList.CustomStreamNfos.Sum(nfo => nfo.Movie.Runtime) * 60;

        _logger.LogInformation("Total playlist length: {totalLength} seconds", totalLength);

        double elapsedSeconds = (DateTime.Now - SequenceStartTime).TotalSeconds % totalLength;
        _logger.LogInformation("Elapsed seconds since sequence start time: {elapsedSeconds}", elapsedSeconds);

        int accumulatedTime = 0;

        foreach (CustomStreamNfo customStreamNfo in customPlayList.CustomStreamNfos)
        {
            int videoLength = customStreamNfo.Movie.Runtime * 60;
            //_logger.LogInformation("Checking video: {customStreamNfo.VideoFileName}, length: {videoLength} seconds", customStreamNfo.VideoFileName, videoLength);

            if (elapsedSeconds < accumulatedTime + videoLength)
            {
                int secondsIn = (int)(elapsedSeconds - accumulatedTime);
                _logger.LogInformation("Current video: {customStreamNfo.VideoFileName}, seconds in: {secondsIn}", customStreamNfo.VideoFileName, secondsIn);
                return (customStreamNfo, secondsIn);
            }

            accumulatedTime += videoLength;
            _logger.LogInformation("Accumulated time: {accumulatedTime} seconds", accumulatedTime);
        }

        CustomStreamNfo firstVideo = customPlayList.CustomStreamNfos[0];
        _logger.LogInformation("Fallback to the first video: {firstVideo.VideoFileName}, elapsed seconds: {elapsedSeconds}", firstVideo.VideoFileName, elapsedSeconds);
        return (firstVideo, (int)elapsedSeconds);
    }

    //public List<XmltvProgramme> GetXmltvProgrammeForPeriod(StationChannelName stationChannelName, DateTime startDate, int days)
    //{
    //    List<(Movie Movie, DateTime StartTime, DateTime EndTime)> movies = GetMoviesForPeriod(stationChannelName.ChannelName, startDate, days);
    //    List<XmltvProgramme> ret = movies.Select(x => XmltvProgrammeConverter.ConvertMovieToXmltvProgramme(x.Movie, stationChannelName.Id)).ToList();
    //    return ret;
    //}

    public (CustomPlayList? customPlayList, CustomStreamNfo? customStreamNfo) GetCustomPlayListByMovieId(string movieId)
    {
        foreach (CustomPlayList customPlayList in GetCustomPlayLists())
        {
            foreach (CustomStreamNfo customStreamNfo in customPlayList.CustomStreamNfos)
            {
                if (customStreamNfo.Movie.Id == movieId)
                {
                    return (customPlayList, customStreamNfo);
                }
            }
        }

        return (null, null);
    }

    public static CustomStreamNfo? GetRandomIntro(int? avoidIndex = null)
    {
        string[] introMovies = Directory.GetFiles(BuildInfo.IntrosFolder, "*.mp4");

        if (introMovies.Length == 0)
        {
            return null;
        }

        List<int> availableIndices = [.. Enumerable.Range(0, introMovies.Length)];

        if (avoidIndex >= 0 && avoidIndex.Value < introMovies.Length)
        {
            availableIndices.Remove(avoidIndex.Value);
        }

        if (availableIndices.Count == 0)
        {
            return null; // In case all indices are avoided, though practically this should not happen
        }

        Random random = new();
        int selectedIndex = availableIndices[random.Next(availableIndices.Count)];

        string introMovie = introMovies[selectedIndex];
        string introMovieName = Path.GetFileNameWithoutExtension(introMovie);

        Movie movie = new() { Title = introMovieName };
        return new CustomStreamNfo(introMovie, movie) { Movie = { Runtime = 0 } };
    }
}