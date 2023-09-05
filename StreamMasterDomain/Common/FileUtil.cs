using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace StreamMasterDomain.Common;

public sealed class FileUtil
{
    private static bool setupDirectories = false;

    public static void CreateDirectory(string fileName)
    {
        string? directory = Path.EndsInDirectorySeparator(fileName) ? fileName : Path.GetDirectoryName(fileName);
        if (directory == null || string.IsNullOrEmpty(directory))
        {
            return;
        }

        if (
            !directory.ToLower().StartsWith(BuildInfo.AppDataFolder.ToLower()) &&
            !$"{directory.ToLower()}{Path.DirectorySeparatorChar}".StartsWith(BuildInfo.AppDataFolder.ToLower())
            )
        {
            throw new Exception($"Illegal directory outside of {BuildInfo.AppDataFolder} : {directory}");
        }

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            _ = Directory.CreateDirectory(directory);
        }
    }

    public static async Task<(bool success, Exception? ex)> DownloadUrlAsync(string url, string fullName, CancellationToken cancellationdefault)
    {
        if (url == null || !url.Contains("://"))
        {
            return (false, null);
        }

        try
        {
            using HttpClient httpClient = new();

            string userAgentString = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36 Edg/110.0.1587.57";

            httpClient.DefaultRequestHeaders.Add("User-Agent", userAgentString);

            try
            {
                string? path = Path.GetDirectoryName(fullName);
                if (string.IsNullOrEmpty(path))
                {
                    return (false, null);
                }
                using FileStream fileStream = new(fullName, FileMode.Create);
                using HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationdefault).ConfigureAwait(false);

                _ = response.EnsureSuccessStatusCode();

                Stream stream = await response.Content.ReadAsStreamAsync(cancellationdefault).ConfigureAwait(false);

                if (stream != null)
                {
                    await stream.CopyToAsync(fileStream, cancellationdefault).ConfigureAwait(false);
                    stream.Close();

                    string filePath = Path.Combine(path, Path.GetFileNameWithoutExtension(fullName) + ".url");
                    _ = WriteUrlToFile(filePath, url);
                }
                fileStream.Close();

                return (true, null);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.ToString());
                return (false, ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (false, ex);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return (false, ex);
        }
    }

    public static async Task<string> GetContentType(string Url)
    {
        try
        {
            using HttpClient httpClient = new();
            HttpResponseMessage response = await httpClient.GetAsync(Url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return "";
            }

            string sContentType = response.Content.Headers.ContentType?.MediaType ?? "";
            return sContentType;
        }
        catch (Exception)
        {
            return "";
        }
    }

    public static Stream GetFileDataStream(string source)
    {
        FileStream fs = File.Open(source, FileMode.Open);

        return IsFileGzipped(source) ? new GZipStream(fs, CompressionMode.Decompress) : fs;
    }


    public static async Task<string> GetFileData(string source)
    {
        try
        {
            if (!IsFileGzipped(source))
            {
                return await File.ReadAllTextAsync(source).ConfigureAwait(false);
            }

            using FileStream fs = File.Open(source, FileMode.Open);
            using GZipStream gzStream = new(fs, CompressionMode.Decompress);
            using StreamReader reader = new(gzStream, Encoding.Default);

            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return "";
        }
    }

    public static async Task<List<TvLogoFile>> GetIconFilesFromDirectory(DirectoryInfo dirInfo, string tvLogosLocation, int startingId, CancellationToken cancellationToken = default)
    {
        _ = FileUtil.GetSetting();
        List<TvLogoFile> ret = new();

        foreach (FileInfo file in dirInfo.GetFiles("*png"))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            string basePath = dirInfo.FullName.Replace(tvLogosLocation, "");
            if (basePath.StartsWith(Path.DirectorySeparatorChar))
            {
                basePath = basePath.Remove(0, 1);
            }

            string basename = basePath.Replace(Path.DirectorySeparatorChar, '-');
            string name = $"{basename}-{file.Name}";

            TvLogoFile tvLogo = new()
            {
                Id = startingId++,
                Name = Path.GetFileNameWithoutExtension(name),
                FileExists = true,
                ContentType = "image/png",
                LastDownloaded = DateTime.Now,
                Source = $"{basePath}{Path.DirectorySeparatorChar}{file.Name}"
            };

            tvLogo.SetFileDefinition(FileDefinitions.TVLogo);
            tvLogo.FileExtension = ".png";
            ret.Add(tvLogo);
        }

        foreach (DirectoryInfo newDir in dirInfo.GetDirectories())
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            List<TvLogoFile> files = await GetIconFilesFromDirectory(newDir, tvLogosLocation, startingId, cancellationToken).ConfigureAwait(false);
            ret = ret.Concat(files).ToList();
        }

        return ret;
    }

    public static Setting GetSetting()
    {
        string jsonString;
        Setting? ret;

        if (File.Exists(BuildInfo.SettingFile))
        {
            jsonString = File.ReadAllText(BuildInfo.SettingFile);
            ret = JsonSerializer.Deserialize<Setting>(jsonString);
            if (ret != null)
            {
                return ret;
            }
        }

        ret = new Setting();
        UpdateSetting(ret);

        return ret;
    }

    public static bool IsFileGzipped(string filePath)
    {
        try
        {
            using FileStream fileStream = File.OpenRead(filePath);
            byte[] signature = new byte[3];

            // Read the first two bytes from the file
            _ = fileStream.Read(signature, 0, 3);

            // Gzip files start with the signature bytes 0x1F 0x8B
            return signature[0] == 0x1F && signature[1] == 0x8B && signature[2] == 0x08;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    public static bool ReadUrlFromFile(string filePath, out string? url)
    {
        url = null;
        try
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length == 1)
                {
                    url = lines[0];
                    _ = WriteUrlToFile(filePath, url);
                    return true;
                }

                string? urlLine = lines.FirstOrDefault(line => line.StartsWith("URL="));

                if (urlLine != null)
                {
                    url = urlLine["URL=".Length..];
                    return true;
                }
                else
                {
                    Console.WriteLine("No URL found in file: " + filePath);
                    return false;
                }
            }
            else
            {
                Console.WriteLine("File not found: " + filePath);
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while reading the file: " + ex.Message);
            return false;
        }
    }

    public static void SetupDirectories(bool alwaysRun = false)
    {
        if (setupDirectories && !alwaysRun)
        {
            return;
        }
        setupDirectories = true;

        CreateDir(BuildInfo.AppDataFolder);
        CreateDir(BuildInfo.CacheFolder);
        CreateDir(BuildInfo.PlayListFolder);
        CreateDir(BuildInfo.IconDataFolder);
        CreateDir(BuildInfo.ChannelIconDataFolder);
        CreateDir(BuildInfo.ProgrammeIconDataFolder);
        CreateDir(BuildInfo.EPGFolder);
        CreateDir(BuildInfo.M3UFolder);
    }

    public static void UpdateSetting(Setting setting)
    {
        if (!Directory.Exists(BuildInfo.AppDataFolder))
        {
            _ = Directory.CreateDirectory(BuildInfo.AppDataFolder);
        }
        string jsonString = JsonSerializer.Serialize(setting, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(BuildInfo.SettingFile, jsonString);
    }



    private static bool WriteUrlToFile(string filePath, string url)
    {
        try
        {
            string content = "[InternetShortcut]\nURL=" + url;
            File.WriteAllText(filePath, content);
            //Console.WriteLine("URL successfully written to file.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while writing the URL to the file: " + ex.Message);
            return false;
        }
    }

    private static void CreateDir(string directory)
    {
        Console.WriteLine($"Creating directory for {directory}");
        CreateDirectory(directory);
    }
}
