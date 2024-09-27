using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.Configuration;
using StreamMaster.PlayList.Models;
using StreamMaster.SchedulesDirect.Domain.XmltvXml;

using System.Xml.Serialization;

namespace StreamMaster.PlayList
{
    [TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
    public class CustomStreamNfo
    {
        public CustomStreamNfo() { }
        public CustomStreamNfo(string VideoFileName, Movie Movie)
        {
            this.VideoFileName = VideoFileName; this.Movie = Movie;

        }
        public string VideoFileName { get; set; }
        public Movie Movie { get; set; }
    }


    public class CustomPlayListBuilder : ICustomPlayListBuilder
    {
        private static readonly DateTime SequenceStartTime = new(2024, 7, 18, 0, 0, 0);
        private readonly ILogger<CustomPlayListBuilder> _logger;
        private readonly INfoFileReader _nfoFileReader;
        private readonly IMemoryCache _memoryCache;
        private readonly FileSystemWatcher _fileSystemWatcher;
        private const string CustomPlayListCacheKey = "CustomPlayLists";

        public CustomPlayListBuilder(ILogger<CustomPlayListBuilder> logger, INfoFileReader nfoFileReader, IMemoryCache memoryCache)
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

        public string GetCustomPlayListLogoFromFileName(string FileName)
        {
            string dir = Path.GetDirectoryName(FileName);
            //string logoName = Path.GetFileNameWithoutExtension(FileName);
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
            {
                return string.Empty;
            }
            string[] files = Directory.GetFiles(dir);
            string logo = files.FirstOrDefault(file => file.EndsWith("poster.png") || file.EndsWith($"poster.jpg")) ?? string.Empty;
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
            if (!_memoryCache.TryGetValue(CustomPlayListCacheKey, out List<CustomPlayList> cachedPlaylists))
            {
                cachedPlaylists = LoadCustomPlayLists();
                MemoryCacheEntryOptions cacheEntryOptions = new()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(10),
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                };
                _memoryCache.Set(CustomPlayListCacheKey, cachedPlaylists, cacheEntryOptions);
            }

            return cachedPlaylists;
        }

        private List<CustomPlayList> LoadCustomPlayLists()
        {
            List<CustomPlayList> ret = [];

            if (string.IsNullOrWhiteSpace(BuildInfo.CustomPlayListFolder) || !Directory.Exists(BuildInfo.CustomPlayListFolder))
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

                customPlayList.FolderNfo = _nfoFileReader.ReadNfoFile(folderNFOFile);
                customPlayList.Name = folderName;
                customPlayList.Logo = GetFolderLogoInDirectory(folder);

                if (customPlayList.FolderNfo == null)
                {
                    continue;
                }

                string[] videoFolders = Directory.GetDirectories(folder);

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

                    if (!File.Exists(fileNFOFile))
                    {
                        continue;
                    }

                    Movie? fileNfo = _nfoFileReader.ReadNfoFile(fileNFOFile);

                    if (fileNfo == null)
                    {
                        continue;
                    }

                    customPlayList.CustomStreamNfos.Add(new CustomStreamNfo(file, fileNfo));
                }

                ret.Add(customPlayList);
            }

            return ret;
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

        public List<XmltvProgramme> GetXmltvProgrammeForPeriod(string customPlayListName, DateTime startDate, int days)
        {
            return [];
            // var movies = GetMoviesForPeriod(customPlayListName, startDate, days);
            // return movies.ConvertAll(XmltvProgrammeConverter.ConvertMovieToXmltvProgramme);
        }

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

        public CustomStreamNfo? GetRandomIntro(int? avoidIndex = null)
        {
            string[] introMovies = Directory.GetFiles(BuildInfo.IntrosFolder, "*.mp4");

            if (introMovies.Length == 0)
            {
                return null;
            }

            List<int> availableIndices = Enumerable.Range(0, introMovies.Length).ToList();

            if (avoidIndex.HasValue && avoidIndex.Value >= 0 && avoidIndex.Value < introMovies.Length)
            {
                availableIndices.Remove(avoidIndex.Value);
            }

            if (!availableIndices.Any())
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
}
