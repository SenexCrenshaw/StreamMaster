using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Extensions;
using StreamMaster.Domain.Repository;
using StreamMaster.PlayList.Models;

namespace StreamMaster.PlayList
{
    public class IntroPlayListBuilder : IIntroPlayListBuilder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMemoryCache _memoryCache;
        private readonly FileSystemWatcher _fileSystemWatcher;
        private const string IntroPlayListCacheKey = "IntroPlayLists";

        public const string IntroIDPrefix = "|intro|";

        public IntroPlayListBuilder(IServiceProvider serviceProvider, IMemoryCache memoryCache)
        {
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;

            _fileSystemWatcher = new FileSystemWatcher(BuildInfo.IntrosFolder, "*.mp4")
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
            };
            _fileSystemWatcher.Changed += OnIntrosFolderChanged;
            _fileSystemWatcher.Created += OnIntrosFolderChanged;
            _fileSystemWatcher.Deleted += OnIntrosFolderChanged;
            _fileSystemWatcher.Renamed += OnIntrosFolderChanged;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void OnIntrosFolderChanged(object sender, FileSystemEventArgs e)
        {
            _memoryCache.Remove(IntroPlayListCacheKey);
        }

        public CustomPlayList? GetIntroPlayList(string name)
        {
            List<CustomPlayList> playlists = GetIntroPlayLists();

            if (name.StartsWithIgnoreCase(IntroIDPrefix))
            {
                name = name[IntroIDPrefix.Length..];
            }

            return playlists.Find(x => x.Name == name);
        }

        public static CustomStreamNfo? GetIntro()
        {
            string introMovie = Path.Combine(BuildInfo.IntrosFolder, "Intro.mp4");

            if (!File.Exists(introMovie))
            {
                return null;
            }

            Movie movie = new() { Title = "Intro" };
            return new CustomStreamNfo(introMovie, movie) { Movie = { Runtime = 0 } };
        }

        public List<CustomPlayList> GetIntroPlayLists()
        {
            return LoadIntroPlayLists();
        }

        private List<CustomPlayList> LoadIntroPlayLists()
        {
            List<CustomPlayList> playlists = [];

            if (string.IsNullOrWhiteSpace(BuildInfo.IntrosFolder) || !Directory.Exists(BuildInfo.IntrosFolder))
            {
                return playlists;
            }

            string[] intros = Directory.GetFiles(BuildInfo.IntrosFolder, "*.mp4");

            foreach (string intro in intros)
            {
                CustomPlayList customPlayList = new()
                {
                    Name = Path.GetFileNameWithoutExtension(intro),
                    Logo = GetIntroLogo(intro)
                };

                Movie movie = new() { Title = customPlayList.Name };
                customPlayList.CustomStreamNfos.Add(new CustomStreamNfo(intro, movie));
                playlists.Add(customPlayList);
            }

            return playlists;
        }

        public int IntroCount => Directory.GetFiles(BuildInfo.IntrosFolder, "*.mp4").Length;

        public string GetIntroLogo(string introFileName)
        {
            string logoName = Path.GetFileNameWithoutExtension(introFileName);
            string? dir = Path.GetDirectoryName(introFileName);
            if (dir == null)
            {
                return "";
            }

            string pngPath = Path.Combine(dir, logoName + ".png");
            if (File.Exists(pngPath))
            {
                return pngPath;
            }

            string jpgPath = Path.Combine(dir, logoName + ".jpg");
            return File.Exists(jpgPath) ? jpgPath : string.Empty;
        }

        public CustomStreamNfo? GetRandomIntro(int? avoidIndex = null)
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

        public string GetRandomSMStreamIntro()
        {
            List<CustomPlayList> intros = GetIntroPlayLists();
            CustomStreamNfo intro = intros[new Random().Next(intros.Count)].CustomStreamNfos[0];
            string streamId = $"{IntroIDPrefix}{intro.Movie.Title}";

            using IServiceScope scope = _serviceProvider.CreateScope();
            IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
            Domain.Dto.SMStreamDto? smStream = repositoryWrapper.SMStream.GetSMStream(streamId);

            return smStream != null ? smStream.Url : string.Empty;
        }
    }
}