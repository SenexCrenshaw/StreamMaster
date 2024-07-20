using Microsoft.Extensions.Logging;

using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.Configuration;
using StreamMaster.PlayList.Models;
using StreamMaster.SchedulesDirect.Domain.XmltvXml;

using System.Xml.Serialization;

namespace StreamMaster.PlayList;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CustomStreamNfo(string VideoFileName, Movie Movie);

public class CustomPlayListBuilder(ILogger<CustomPlayListBuilder> logger, INfoFileReader nfoFileReader) : ICustomPlayListBuilder
{
    private static readonly DateTime SequenceStartTime = new(2024, 7, 18, 0, 0, 0);

    public CustomPlayList? GetCustomPlayList(string Name)
    {
        CustomPlayList? ret = GetCustomPlayLists().Find(x => x.Name == Name);
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
    public int IntroCount => Directory.GetFiles(BuildInfo.IntrosFolder, "*.mp4").Count();
    public CustomStreamNfo? GetIntro(int introIndex)
    {
        string[] introMovies = Directory.GetFiles(BuildInfo.IntrosFolder, "*.mp4");

        if (introMovies.Length == 0)
        {
            return null;
        }

        if (introIndex >= introMovies.Length)
        {
            introIndex = 0;
        }

        string introMovie = Path.Combine(BuildInfo.IntrosFolder, introMovies[introIndex]);
        if (File.Exists(introMovie))
        {
            string introMovieName = Path.GetFileNameWithoutExtension(introMovie);
            _ = Path.Combine(introMovieName, $"{introMovieName}.jpg");

            Movie movie = new()
            {
                Title = introMovieName,
            };
            CustomStreamNfo customStreamNfo = new(introMovie, movie);
            customStreamNfo.Movie.Runtime = 0;
            return customStreamNfo;
        }
        return null;
    }
    public CustomStreamNfo? GetIntro()
    {
        string introMovie = Path.Combine(BuildInfo.IntrosFolder, "Intro.mp4");
        if (File.Exists(introMovie))
        {
            _ = Path.Combine(BuildInfo.CustomPlayListFolder, "Intro.jpg");
            Movie movie = new()
            {
                Title = "Intro",
            };
            CustomStreamNfo customStreamNfo = new(introMovie, movie);
            customStreamNfo.Movie.Runtime = 0;
            return customStreamNfo;
        }
        return null;
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

    public List<(Movie Movie, DateTime StartTime, DateTime EndTime)> GetMoviesForPeriod(string customPlayListName, DateTime startDate, int days)
    {
        CustomPlayList? customPlayList = GetCustomPlayList(customPlayListName);
        if (customPlayList == null)
        {
            throw new ArgumentException($"Custom playlist with name {customPlayListName} not found.");
        }

        // Calculate the start and end times for the period
        DateTime periodStartTime = new DateTime(startDate.Year, startDate.Month, startDate.Day, 23, 0, 0).AddDays(-1);
        DateTime periodEndTime = periodStartTime.AddDays(days + 1);

        // Total length is in seconds, converting runtime from minutes to seconds
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
        CustomPlayList? customPlayList = GetCustomPlayList(customPlayListName) ?? throw new ArgumentException($"Custom playlist with name {customPlayListName} not found.");

        CustomStreamNfo? intro = GetIntro();
        // Calculate the total length of the playlist
        int totalLength = customPlayList.CustomStreamNfos.Sum(nfo => nfo.Movie.Runtime) * 60;

        logger.LogInformation("Total playlist length: {totalLength} seconds", totalLength);

        // Calculate elapsed seconds since the sequence start time
        double elapsedSeconds = (DateTime.Now - SequenceStartTime).TotalSeconds % totalLength;
        logger.LogInformation("Elapsed seconds since sequence start time: {elapsedSeconds}", elapsedSeconds);

        int accumulatedTime = 0;

        // Find the current video and the seconds into it
        foreach (CustomStreamNfo customStreamNfo in customPlayList.CustomStreamNfos)
        {
            int videoLength = (customStreamNfo.Movie.Runtime * 60) + (intro == null ? 0 : 6);
            logger.LogInformation("Checking video: {customStreamNfo.VideoFileName}, length: {videoLength} seconds", customStreamNfo.VideoFileName, videoLength);

            if (elapsedSeconds < accumulatedTime + videoLength)
            {
                int secondsIn = (int)(elapsedSeconds - accumulatedTime);
                logger.LogInformation("Current video: {customStreamNfo.VideoFileName}, seconds in: {secondsIn}", customStreamNfo.VideoFileName, secondsIn);
                return (customStreamNfo, secondsIn);
            }

            accumulatedTime += videoLength;
            logger.LogInformation("Accumulated time: {accumulatedTime} seconds", accumulatedTime);
        }

        // If for some reason no video was found (though this should not happen), return the first video
        CustomStreamNfo firstVideo = customPlayList.CustomStreamNfos[0];
        logger.LogInformation("Fallback to the first video: {firstVideo.VideoFileName}, elapsed seconds: {elapsedSeconds}", firstVideo.VideoFileName, elapsedSeconds);
        return (firstVideo, (int)elapsedSeconds);
    }

    public List<XmltvProgramme> GetXmltvProgrammeForPeriod(string customPlayListName, DateTime startDate, int days)
    {
        return [];
        //var movies = GetMoviesForPeriod(customPlayListName, startDate, days);
        //return movies.ConvertAll(XmltvProgrammeConverter.ConvertMovieToXmltvProgramme);
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
}

