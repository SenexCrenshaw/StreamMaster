using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Extensions;
using StreamMaster.Domain.Helpers;
using StreamMaster.SchedulesDirect.Domain.XmltvXml;

using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace StreamMaster.Domain.Common;

public sealed class FileUtil
{
    public static async Task<bool> WaitForFileAsync(string filePath, int timeoutSeconds, int checkIntervalMilliseconds, CancellationToken cancellationToken)
    {
        CancellationTokenSource timeoutTokenSource = new(TimeSpan.FromSeconds(timeoutSeconds));
        CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutTokenSource.Token);

        try
        {
            string? dir = Path.GetDirectoryName(filePath);
            if (dir != null && !Directory.Exists(dir))
            {
                while (!Directory.Exists(dir))
                {
                    await Task.Delay(checkIntervalMilliseconds, linkedTokenSource.Token).ConfigureAwait(false);
                }
            }

            while (!File.Exists(filePath))
            {
                await Task.Delay(checkIntervalMilliseconds, linkedTokenSource.Token).ConfigureAwait(false);
            }

            return true;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // Timeout has occurred
            return false;
        }
        catch (OperationCanceledException)
        {
            // External cancellation request
            throw;
        }
    }

    public static string CleanUpFileName(string fullName)
    {
        // Remove double spaces, trim, and replace spaces with underscores
        fullName = string.Join("_", fullName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));

        // Ensure the file name doesn't start or end with an underscore
        if (fullName.StartsWith("_"))
        {
            fullName = fullName.TrimStart('_');
        }

        if (fullName.EndsWith("_"))
        {
            fullName = fullName.TrimEnd('_');
        }
        return fullName;
    }

    public static XMLTV? ReadXmlFile(string filepath)
    {
        if (!File.Exists(filepath))
        {
            //Logger.WriteInformation($"File \"{filepath}\" does not exist.");
            return null;
        }

        try
        {
            XmlReaderSettings settings = new()
            {
                DtdProcessing = DtdProcessing.Ignore,
                ValidationType = ValidationType.DTD,
                MaxCharactersFromEntities = 1024
            };

            XmlSerializer serializer = new(typeof(XMLTV));
            using Stream fileStream = GetFileDataStream(filepath);
            using XmlReader reader = XmlReader.Create(fileStream, settings);
            object? result = serializer.Deserialize(reader);
            return (XMLTV?)result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to read file \"{filepath}\". Exception:{ReportExceptionMessages(ex)}");
        }
        return null;
    }
    public static string BytesToString(long bytes)
    {
        string[] unit = { "", "K", "M", "G", "T" };
        for (int i = 0; i < unit.Length; ++i)
        {
            double calc;
            if ((calc = bytes / Math.Pow(1024, i)) < 1024)
            {
                return $"{calc:N3} {unit[i]}B";
            }
        }
        return "0 bytes";
    }

    public static bool WriteXmlFile(object obj, string filepath)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            XmlSerializer serializer = new(obj.GetType());
            XmlSerializerNamespaces ns = new();
            ns.Add("", "");
            using StreamWriter writer = new(filepath, false, Encoding.UTF8);
            serializer.Serialize(writer, obj, ns);
            //if (compress)
            //{
            //    GZipCompressFile(filepath);
            //    DeflateCompressFile(filepath);
            //}
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to write file \"{filepath}\". Exception:{ReportExceptionMessages(ex)}");
        }
        return false;
    }

    public static string ReportExceptionMessages(Exception ex)
    {
        string ret = string.Empty;
        Exception? innerException = ex;
        do
        {
            ret += $" {innerException.Message} ";
            innerException = innerException.InnerException;
        } while (innerException != null);
        return ret;
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
                if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.NotFound)
                {
                    return (false, null);
                }

                _ = response.EnsureSuccessStatusCode();

                Stream stream = await response.Content.ReadAsStreamAsync(cancellationdefault).ConfigureAwait(false);

                if (stream != null)
                {
                    await stream.CopyToAsync(fileStream, cancellationdefault).ConfigureAwait(false);
                    stream.Close();

                    //string filePath = Path.Combine(path, Path.GetFileNameWithoutExtension(fullName) + ".url");
                    //_ = WriteUrlToFile(filePath, url);
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

        if (!IsFileGzipped(source))
        {
            return File.OpenRead(source);
        }
        FileStream fs = File.OpenRead(source);
        return new GZipStream(fs, CompressionMode.Decompress);
    }

    public static async Task Backup(int? versionsToKeep = null)
    {
        Setting? setting = SettingsHelper.GetSetting<Setting>(BuildInfo.SettingFileName);
        if (setting == null || setting.BackupEnabled == false)
        {
            return;
        }

        try
        {
            versionsToKeep ??= SettingsHelper.GetSetting<Setting>(BuildInfo.SettingFileName)?.BackupVersionsToKeep ?? 5;
            using Process process = new();
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"/usr/local/bin/backup.sh {versionsToKeep}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (!string.IsNullOrEmpty(output))
            {
                Console.WriteLine($"Backup Output: {output}");
            }

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine($"Backup Error: {error}");
            }

            if (process.ExitCode == 0)
            {
                Console.WriteLine("Backup executed successfully.");
            }
            else
            {
                Console.WriteLine($"Backup execution failed with exit code: {process.ExitCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Backup Exception occurred: {ex.Message}");
        }

    }

    public static async Task<string> GetFileData(string source)
    {
        try
        {
            if (!IsFileGzipped(source))
            {
                return await File.ReadAllTextAsync(source).ConfigureAwait(false);
            }

            using FileStream fs = File.OpenRead(source);
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

    public static async Task<List<TvLogoFile>> GetTVLogosFromDirectory(DirectoryInfo dirInfo, string tvLogosLocation, int startingId, CancellationToken cancellationToken = default)
    {
        List<TvLogoFile> ret = [];

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
                LastDownloaded = SMDT.UtcNow,
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
            List<TvLogoFile> files = await GetTVLogosFromDirectory(newDir, tvLogosLocation, startingId, cancellationToken).ConfigureAwait(false);
            ret = [.. ret, .. files];
        }

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


}