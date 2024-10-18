using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Enums;

using System.IO.Compression;
using System.Net;

namespace StreamMaster.Infrastructure.Services
{
    public class FileUtilService : IFileUtilService
    {
        public HttpClient _httpClient = null!;
        private readonly ILogger<FileUtilService> logger;
        private readonly IOptionsMonitor<Setting> settings;

        public FileUtilService(ILogger<FileUtilService> logger, IOptionsMonitor<Setting> _settings)
        {
            this.logger = logger;
            settings = _settings;
            CreateHttpClient();
        }

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
                return new GZipStream(fileStream, CompressionMode.Decompress);
            }
            else if (IsFileZipped(fileStream))
            {
                ZipArchive zipArchive = new(fileStream, ZipArchiveMode.Read, leaveOpen: true);
                ZipArchiveEntry zipEntry = zipArchive.Entries[0]; // Read the first entry
                return zipEntry.Open(); // Stream remains open even after ZipArchive is disposed
            }

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
                string compression = settings.CurrentValue.DefaultCompression?.ToLower() ?? "none";

                //using HttpClientHandler handler = new() { AllowAutoRedirect = true, MaxAutomaticRedirections = 10 };
                //using HttpClient httpClient = new(handler);
                //_httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                Directory.CreateDirectory(Path.GetDirectoryName(fileName) ?? string.Empty); // Ensure directory exists

                using HttpResponseMessage response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                response.EnsureSuccessStatusCode(); // Ensure success

                await using Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                await SaveStreamToFileAsync(stream, fileName + "_temp", "none");
                await using Stream? dataStream = GetFileDataStream(fileName + "_temp");
                if (dataStream == null)
                {
                    return (false, null);
                }

                if (IsFileGzipped(fileName + "_temp"))
                {
                    if (!fileName.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
                    {
                        fileName += ".gz";
                    }

                    await SaveStreamToFileAsync(dataStream, fileName, "none"); // No additional compression
                }
                else if (IsFileZipped(fileName + "_temp"))
                {
                    if (!fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    {
                        fileName += ".zip";
                    }
                    await SaveStreamToFileAsync(dataStream, fileName, "none"); // No additional compression
                }
                else
                {
                    await SaveStreamToFileAsync(dataStream, fileName, compression);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                logger.LogError("Download failed for {Url} : {message}", url, ex.Message);
                return (false, ex);
            }
            finally
            {
                if (File.Exists(fileName + "_temp"))
                {
                    File.Delete(fileName + "_temp");
                }
            }
        }

        public async Task<(bool success, Exception? ex)> SaveFormFileAsync(dynamic data, string fileName)
        {
            try
            {
                string compression = settings.CurrentValue.DefaultCompression?.ToLower() ?? "none";
                using Stream stream = data.OpenReadStream();

                // Check if the uploaded file is already compressed
                if (IsFileGzipped(stream) || IsFileZipped(stream))
                {
                    await SaveStreamToFileAsync(stream, fileName, "none"); // No additional compression
                }
                else
                {
                    await SaveStreamToFileAsync(stream, fileName, compression);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving form file {FileName}", fileName);
                return (false, ex);
            }
        }

        private static async Task SaveStreamToFileAsync(Stream inputStream, string fileName, string compression)
        {
            string compressedFileName = fileName;

            await using FileStream fileStream = File.Create(compressedFileName);

            if (compression == "gz")
            {
                await using GZipStream gzipStream = new(fileStream, CompressionMode.Compress);
                await inputStream.CopyToAsync(gzipStream, 81920).ConfigureAwait(false); // Using a buffer size of 80 KB
            }
            else if (compression == "zip")
            {
                using ZipArchive zipArchive = new(fileStream, ZipArchiveMode.Create);
                ZipArchiveEntry zipEntry = zipArchive.CreateEntry(Path.GetFileName(fileName));
                await using Stream zipStream = zipEntry.Open();
                await inputStream.CopyToAsync(zipStream, 81920).ConfigureAwait(false); // Using a buffer size of 80 KB
            }
            else // No compression
            {
                await inputStream.CopyToAsync(fileStream, 81920).ConfigureAwait(false); // Using a buffer size of 80 KB
            }
        }

        private static bool IsFileZipped(Stream fileStream)
        {
            try
            {
                byte[] signature = new byte[4];
                fileStream.Read(signature, 0, 4);
                fileStream.Seek(0, SeekOrigin.Begin);
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
                fileStream.Seek(0, SeekOrigin.Begin);
                return signature[0] == 0x1F && signature[1] == 0x8B && signature[2] == 0x08;
            }
            catch
            {
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
            return (settings.CurrentValue.DefaultCompression?.ToLower()) switch
            {
                "gz" => fullName + ".gz",
                "zip" => fullName + ".zip",
                _ => fullName,
            };
        }

        public void CleanUpFile(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("File name cannot be null or empty", nameof(fullName));
            }

            // Delete the original file if it exists
            if (File.Exists(fullName))
            {
                File.Delete(fullName);
            }

            // Get the directory of the original file
            string directoryPath = Path.GetDirectoryName(fullName) ?? string.Empty;

            // Determine the base file name without any compression extension (like .gz or .zip)
            string fileNameWithoutCompression;
            string originalExtension;

            if (fullName.EndsWith(".gz", StringComparison.OrdinalIgnoreCase) || fullName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                // Remove both the compression extension and the main file extension
                fileNameWithoutCompression = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fullName));
                originalExtension = Path.GetExtension(Path.GetFileNameWithoutExtension(fullName)); // Retain the base file's extension (e.g., .m3u)
            }
            else
            {
                // Just remove the main file extension
                fileNameWithoutCompression = Path.GetFileNameWithoutExtension(fullName);
                originalExtension = Path.GetExtension(fullName); // Keep the original file's extension (e.g., .m3u)
            }

            // Combine the directory with the base name + original extension to form the .url and .json paths
            string urlPath = Path.Combine(directoryPath, fileNameWithoutCompression + originalExtension + ".url");
            string jsonPath = Path.Combine(directoryPath, fileNameWithoutCompression + originalExtension + ".json");

            // Delete the .url file if it exists
            if (File.Exists(urlPath))
            {
                File.Delete(urlPath);
            }

            // Delete the .json file if it exists
            if (File.Exists(jsonPath))
            {
                File.Delete(jsonPath);
            }
        }

        private void CreateHttpClient()
        {
            _httpClient = new(new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
            })
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(settings.CurrentValue.ClientUserAgent);
            _httpClient.DefaultRequestHeaders.ExpectContinue = true;
        }
    }
}