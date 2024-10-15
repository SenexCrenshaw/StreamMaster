using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Enums;

using System.IO.Compression;

namespace StreamMaster.Infrastructure.Services
{
    public class FileUtilService(ILogger<FileUtilService> logger, IOptionsMonitor<Setting> _settings)
        : IFileUtilService
    {
        public Stream? GetFileDataStream(string source)
        {
            string? filePath = GetExistingFilePath(source);
            if (filePath == null)
            {
                return null;
            }

            FileStream fileStream = File.OpenRead(filePath);

            if (IsFileGzipped(fileStream))
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                return new GZipStream(fileStream, CompressionMode.Decompress);
            }
            else if (IsFileZipped(fileStream))
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                ZipArchive zipArchive = new(fileStream, ZipArchiveMode.Read, leaveOpen: true);
                ZipArchiveEntry zipEntry = zipArchive.Entries[0]; // Read the first entry
                return zipEntry.Open(); // Stream remains open even after ZipArchive is disposed
            }

            fileStream.Seek(0, SeekOrigin.Begin);
            return fileStream;
        }
        public string? GetExistingFilePath(string source)
        {
            if (File.Exists(source))
            {
                return source;
            }
            else if (File.Exists(source + ".gz"))
            {
                return source + ".gz";
            }
            else if (File.Exists(source + ".zip"))
            {
                return source + ".zip";
            }

            return null;
        }

        public IEnumerable<FileInfo> GetFilesFromDirectory(FileDefinition fileDefinition)
        {
            DirectoryInfo dirInfo = new(fileDefinition.DirectoryLocation);
            string[] extensions = fileDefinition.FileExtensions.Split('|');

            return dirInfo.GetFiles("*.*", SearchOption.AllDirectories)
                .Where(file =>
                    extensions.Any(ext =>
                        file.Name.EndsWith(ext, StringComparison.OrdinalIgnoreCase) ||
                        file.Name.EndsWith(ext + ".gz", StringComparison.OrdinalIgnoreCase) ||
                        file.Name.EndsWith(ext + ".zip", StringComparison.OrdinalIgnoreCase)
                    )
                );
        }

        public async Task<(bool success, Exception? ex)> DownloadUrlAsync(string url, string fileName, bool? ignoreCompression = false)
        {
            if (string.IsNullOrWhiteSpace(url) || !url.Contains("://"))
            {
                return (false, null); // Invalid URL
            }

            try
            {
                string compression = _settings.CurrentValue.DefaultCompression?.ToLower() ?? "none";

                HttpClientHandler handler = new() { AllowAutoRedirect = true, MaxAutomaticRedirections = 10 };
                using HttpClient httpClient = new(handler);

                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                Directory.CreateDirectory(Path.GetDirectoryName(fileName) ?? string.Empty); // Ensure directory exists

                using HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                response.EnsureSuccessStatusCode(); // Ensure success


                await using Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                // Check if the file is already compressed
                //if (ignoreCompression == true || string.Equals(_settings.CurrentValue.DefaultCompression, "none", StringComparison.OrdinalIgnoreCase) || IsFileGzipped(fullName) || IsFileZipped(fullName))
                if (IsFileGzipped(fileName) || IsFileZipped(fileName))
                {
                    await SaveStreamToFile(stream, fileName, "none"); // No additional compression
                }
                else
                {
                    await SaveStreamToFile(stream, fileName, compression);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                logger.LogError("Download failed for {Url} : {message}", url, ex.Message);
                return (false, ex);
            }
        }

        public async Task<(bool success, Exception? ex)> SaveFormFileAsync(dynamic data, string fileName)
        {
            try
            {
                string compression = _settings.CurrentValue.DefaultCompression?.ToLower() ?? "none";

                // Check if the uploaded file is already compressed
                if (IsFileGzipped(fileName) || IsFileZipped(fileName))
                {
                    await SaveStreamToFile(data.OpenReadStream(), fileName, "none"); // No additional compression
                }
                else
                {
                    await SaveStreamToFile(data.OpenReadStream(), fileName, compression);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving form file {FileName}", fileName);
                return (false, ex);
            }
        }

        private static async Task SaveStreamToFile(Stream inputStream, string fileName, string compression)
        {
            string compressedFileName = fileName;

            await using FileStream fileStream = File.Create(compressedFileName);

            if (compression == "gz")
            {
                await using GZipStream gzipStream = new(fileStream, CompressionMode.Compress);
                await inputStream.CopyToAsync(gzipStream).ConfigureAwait(false);
            }
            else if (compression == "zip")
            {
                using ZipArchive zipArchive = new(fileStream, ZipArchiveMode.Create);
                ZipArchiveEntry zipEntry = zipArchive.CreateEntry(Path.GetFileName(fileName));
                await using Stream zipStream = zipEntry.Open();
                await inputStream.CopyToAsync(zipStream).ConfigureAwait(false);
            }
            else // No compression
            {
                await inputStream.CopyToAsync(fileStream).ConfigureAwait(false);
            }
        }

        private static bool IsFileZipped(Stream fileStream)
        {
            try
            {
                byte[] signature = new byte[4];
                fileStream.Read(signature, 0, 4);
                return signature[0] == 0x50 && signature[1] == 0x4B && signature[2] == 0x03 && signature[3] == 0x04;
            }
            catch
            {
                // logger.LogError(ex, "Error checking if file stream is zipped.");
                return false;
            }
        }

        private static bool IsFileGzipped(Stream fileStream)
        {
            try
            {
                byte[] signature = new byte[3];
                fileStream.Read(signature, 0, 3);
                return signature[0] == 0x1F && signature[1] == 0x8B && signature[2] == 0x08;
            }
            catch
            {
                // logger.LogError(ex, "Error checking if file stream is gzipped.");
                return false;
            }
        }

        public bool IsFileZipped(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return false;
                }
                using FileStream fileStream = File.OpenRead(filePath);
                return IsFileZipped(fileStream);
            }
            catch
            {
                // logger.LogError(ex, "Er                ror checking if file { FilePath} is zipped.", filePath);
                return false;
            }
        }

        public bool IsFileGzipped(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return false;
                }
                using FileStream fileStream = File.OpenRead(filePath);
                return IsFileGzipped(fileStream);
            }
            catch
            {
                //logger.LogError(ex, "Error checking if file {FilePath} is gzipped.", filePath);
                return false;
            }
        }

        public string CheckNeedsCompression(string fullName)
        {
            return (_settings.CurrentValue.DefaultCompression?.ToLower()) switch
            {
                "gz" => fullName + ".gz",
                "zip" => fullName + ".zip",
                _ => fullName,
            };
        }
    }
}