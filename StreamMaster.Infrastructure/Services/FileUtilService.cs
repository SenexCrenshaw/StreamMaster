using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Enums;

using System.IO.Compression;

namespace StreamMaster.Infrastructure.Services
{
    public class FileUtilService(ILogger<FileUtilService> logger, IOptionsMonitor<Setting> _settings)
        : IFileUtilService
    {
        public Stream GetFileDataStream(string source)
        {
            string extension = Path.GetExtension(source).ToLowerInvariant();
            FileStream fileStream = File.OpenRead(source);

            if (extension == ".gz" || IsFileGzipped(source))
            {
                return new GZipStream(fileStream, CompressionMode.Decompress);
            }
            else if (extension == ".zip" || IsFileZipped(source))
            {
                ZipArchive zipArchive = new(fileStream, ZipArchiveMode.Read);
                ZipArchiveEntry zipEntry = zipArchive.Entries[0]; // Read the first entry
                return zipEntry.Open(); // Stream remains open even after ZipArchive is disposed
            }
            return fileStream;
        }

        public IEnumerable<FileInfo> GetFilesFromDirectory(FileDefinition fileDefinition)
        {
            DirectoryInfo dirInfo = new(fileDefinition.DirectoryLocation);
            string[] extensions = fileDefinition.FileExtension.Split('|');

            return dirInfo.GetFiles("*.*", SearchOption.AllDirectories)
                .Where(file => extensions.Contains(file.Extension.ToLower()) ||
                               extensions.Contains(file.Extension.ToLower() + ".gz") ||
                               extensions.Contains(file.Extension.ToLower() + ".zip"));
        }

        public async Task<(bool success, Exception? ex)> DownloadUrlAsync(string url, string fullName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(url) || !url.Contains("://"))
            {
                return (false, null); // Invalid URL
            }

            try
            {
                HttpClientHandler handler = new() { AllowAutoRedirect = true, MaxAutomaticRedirections = 10 };
                using HttpClient httpClient = new(handler);

                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                Directory.CreateDirectory(Path.GetDirectoryName(fullName) ?? string.Empty); // Ensure directory exists

                using HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode(); // Ensure success

                await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

                // Check if the file is already compressed
                if (string.Equals(_settings.CurrentValue.DefaultCompression, "none", StringComparison.OrdinalIgnoreCase) || IsFileGzipped(fullName) || IsFileZipped(fullName))
                {
                    await SaveStreamToFile(stream, fullName, "none", cancellationToken);
                }
                else
                {
                    await SaveStreamToFile(stream, fullName, _settings.CurrentValue.DefaultCompression, cancellationToken);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Download failed for {Url}", url);
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
                    await SaveStreamToFile(data.OpenReadStream(), fileName, "none", CancellationToken.None); // No additional compression
                }
                else
                {
                    await SaveStreamToFile(data.OpenReadStream(), fileName, compression, CancellationToken.None);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving form file {FileName}", fileName);
                return (false, ex);
            }
        }

        private static async Task SaveStreamToFile(Stream inputStream, string fileName, string compression, CancellationToken cancellationToken)
        {
            //string compressedFileName = compression switch
            //{
            //    "gz" => fileName + ".gz",
            //    "zip" => Path.ChangeExtension(fileName, ".zip"),
            //    _ => fileName // No compression
            //};

            string compressedFileName = fileName;

            await using FileStream fileStream = File.Create(compressedFileName);

            if (compression == "gz")
            {
                await using GZipStream gzipStream = new(fileStream, CompressionMode.Compress);
                await inputStream.CopyToAsync(gzipStream, cancellationToken).ConfigureAwait(false);
            }
            else if (compression == "zip")
            {
                using ZipArchive zipArchive = new(fileStream, ZipArchiveMode.Create);
                ZipArchiveEntry zipEntry = zipArchive.CreateEntry(Path.GetFileName(fileName));
                await using Stream zipStream = zipEntry.Open();
                await inputStream.CopyToAsync(zipStream, cancellationToken).ConfigureAwait(false);
            }
            else // No compression
            {
                await inputStream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
            }
        }

        public bool IsFileZipped(string filePath)
        {
            try
            {
                using FileStream fileStream = File.OpenRead(filePath);
                byte[] signature = new byte[4];
                fileStream.Read(signature, 0, 4);

                return signature[0] == 0x50 && signature[1] == 0x4B && signature[2] == 0x03 && signature[3] == 0x04;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking if file {FilePath} is zipped.", filePath);
                return false;
            }
        }

        public bool IsFileGzipped(string filePath)
        {
            try
            {
                using FileStream fileStream = File.OpenRead(filePath);
                byte[] signature = new byte[3];
                fileStream.Read(signature, 0, 3);

                return signature[0] == 0x1F && signature[1] == 0x8B && signature[2] == 0x08;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking if file {FilePath} is gzipped.", filePath);
                return false;
            }
        }
    }
}