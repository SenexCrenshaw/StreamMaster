using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Extensions;
using StreamMaster.Domain.Helpers;

using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;

namespace StreamMaster.Domain.Common;

public sealed class FileUtil
{
    /// <summary>
    /// Searches for the specified executable name in predefined directories.
    /// </summary>
    /// <param name="executableName">The name of the executable to locate.</param>
    /// <returns>The full path to the executable if found, otherwise null.</returns>
    public static string? GetExec(string executableName)
    {
        if (string.IsNullOrEmpty(executableName))
        {
            throw new ArgumentException("Executable name cannot be null or empty.", nameof(executableName));
        }

        // List of directories to search for the executable
        string[] directoriesToSearch =
        [
            string.Empty, // Current directory
            BuildInfo.AppDataFolder,
            "/usr/local/bin",
            "/usr/bin",
            "/bin"
        ];

        foreach (string? directory in directoriesToSearch)
        {
            string execPath = Path.Combine(directory, executableName);

            if (File.Exists(execPath))
            {
                return execPath;
            }

            if (File.Exists(execPath + ".exe"))
            {
                return execPath + ".exe";
            }
        }

        return null;
    }

    public static void WriteJSON(string fileName, string jsonText, string? directory = null)
    {
        string jsonPath = Path.Combine(directory ?? BuildInfo.AppDataFolder, fileName);
        File.WriteAllText(jsonPath, jsonText);
    }

    public static string EncodeToMD5(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentNullException(nameof(url), "URL cannot be null or empty.");
        }
        byte[] hashBytes = System.Security.Cryptography.MD5.HashData(Encoding.UTF8.GetBytes(url));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }

    public static async Task<bool> WaitForFileAsync(string filePath, int timeoutSeconds, int checkIntervalMilliseconds, CancellationToken cancellationToken)
    {
        try
        {
            if (File.Exists(filePath))
            {
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }

        bool didReport = false;
        using CancellationTokenSource timeoutTokenSource = new(TimeSpan.FromSeconds(timeoutSeconds));
        using CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutTokenSource.Token);

        try
        {
            string? directoryPath = Path.GetDirectoryName(filePath);

            if (directoryPath != null && !Directory.Exists(directoryPath))
            {
                // Wait for the directory to exist
                while (!Directory.Exists(directoryPath))
                {
                    if (!didReport)
                    {
                        didReport = true;
                        Debug.WriteLine("Waited on {directoryPath}", directoryPath);
                    }
                    await Task.Delay(checkIntervalMilliseconds, linkedTokenSource.Token).ConfigureAwait(false);
                }
            }

            // Wait for the file to exist
            while (!File.Exists(filePath))
            {
                await Task.Delay(checkIntervalMilliseconds, linkedTokenSource.Token).ConfigureAwait(false);
            }

            return true;
        }
        catch (OperationCanceledException) when (timeoutTokenSource.Token.IsCancellationRequested)
        {
            // Timeout has occurred
            return false;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // External cancellation request
            throw;
        }
    }

    private static readonly char[] separator = [' '];

    public static string CleanUpFileName(string fullName)
    {
        // Remove double spaces, trim, and replace spaces with underscores
        fullName = string.Join("_", fullName.Split(separator, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));

        // Ensure the file name doesn't start or end with an underscore
        if (fullName.StartsWith('_'))
        {
            fullName = fullName.TrimStart('_');
        }

        if (fullName.EndsWith('_'))
        {
            fullName = fullName.TrimEnd('_');
        }
        return fullName;
    }

    public static string BytesToString(long bytes)
    {
        string[] unit = ["", "K", "M", "G", "T"];
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

    //public static async Task<string> GetContentType(string Url)
    //{
    //    try
    //    {
    //        using HttpClient httpClient = new();
    //        HttpResponseMessage response = await httpClient.GetAsync(Url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
    //        if (!response.IsSuccessStatusCode)
    //        {
    //            return "";
    //        }

    //        string sContentType = response.Content.Headers.ContentType?.MediaType ?? "";
    //        return sContentType;
    //    }
    //    catch (Exception)
    //    {
    //        return "";
    //    }
    //}

    public static async Task Backup(int? versionsToKeep = null)
    {
        Setting? setting = SettingsHelper.GetSetting<Setting>(BuildInfo.SettingsFile);
        if (setting?.BackupEnabled != true)
        {
            return;
        }

        try
        {
            versionsToKeep ??= SettingsHelper.GetSetting<Setting>(BuildInfo.SettingsFile)?.BackupVersionsToKeep ?? 5;
            using Process process = new();
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"/usr/local/bin/backup.sh {versionsToKeep}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            _ = process.Start();

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
                Name = Path.GetFileNameWithoutExtension(file.Name),
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
}