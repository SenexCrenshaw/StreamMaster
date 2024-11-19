using System.IO.Compression;
using System.Net;
using System.Xml;
using System.Xml.Serialization;

using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Extensions;
using StreamMaster.Domain.XmltvXml;
using StreamMaster.Streams.Domain.Interfaces;

namespace StreamMaster.Infrastructure.Services
{
    public class FileUtilService : IFileUtilService
    {
        public HttpClient _httpClient = null!;
        private readonly ILogger<FileUtilService> logger;
        private readonly ICacheManager cacheManager;
        private readonly IOptionsMonitor<Setting> settings;

        public FileUtilService(ILogger<FileUtilService> logger, ICacheManager cacheManager, IOptionsMonitor<Setting> _settings)
        {
            this.logger = logger;
            settings = _settings;
            this.cacheManager = cacheManager;
            CreateHttpClient();
        }

        public async Task<List<StationChannelName>?> ProcessStationChannelNamesAsync(EPGFile epgFile)
        {
            string? epgPath = GetEPGFilePath(epgFile);
            return string.IsNullOrEmpty(epgPath) ? null : await ProcessStationChannelNamesAsync(epgPath, epgFile.EPGNumber);
        }

        public async Task<List<StationChannelName>?> ProcessStationChannelNamesAsync(string epgPath, int epgNumber)
        {
            if (cacheManager.StationChannelNames.TryGetValue(epgNumber, out List<StationChannelName>? value))
            {
                return value;
            }

            List<XmltvChannel> channels = await GetChannelsFromXmlAsync(epgPath).ConfigureAwait(false);
            if (channels.Count == 0)
            {
                return null;
            }

            value = [];

            foreach (XmltvChannel xmlChannel in channels)
            {
                //string channelId = GetID(xmlChannel, epgNumber);

                List<XmltvText> displayNames = xmlChannel.DisplayNames;
                string callSign = displayNames.Count > 0 ? displayNames[0]?.Text ?? xmlChannel.Id : xmlChannel.Id;
                string name = displayNames.Count > 1 ? displayNames[1]?.Text ?? callSign : callSign;

                string displayName = $"[{callSign}] {name}";
                string channel = xmlChannel.Id;// $"{epgNumber}-{xmlChannel.Id}";
                StationChannelName stationChannelName = new(channel, displayName, name, epgNumber);
                //{
                //    Channel = channel,
                //    DisplayName = displayName,
                //    ChannelName = name,
                //    EPGNumber = epgNumber
                //};

                value.Add(stationChannelName);
            }
            cacheManager.StationChannelNames[epgNumber] = value;
            return value;
        }

        public async Task<List<XmltvChannel>> GetChannelsFromXmlAsync(string epgPath)
        {
            List<XmltvChannel> channels = [];

            try
            {
                string? filePath = GetFilePath(epgPath);
                if (filePath == null)
                {
                    return channels;
                }

                XmlReaderSettings settings = new()
                {
                    Async = true,
                    DtdProcessing = DtdProcessing.Ignore,
                    MaxCharactersFromEntities = 1024,
                    ConformanceLevel = ConformanceLevel.Document,
                };

                await using Stream? fileStream = await GetFileDataStream(filePath).ConfigureAwait(false);
                if (fileStream == null)
                {
                    return channels;
                }

                using XmlReader reader = XmlReader.Create(fileStream, settings);

                XmltvChannel? currentChannel = null;
                List<XmltvText>? displayNames = null;

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "channel")
                    {
                        currentChannel = new XmltvChannel
                        {
                            Id = reader.GetAttribute("id") ?? string.Empty
                        };
                        displayNames = [];
                    }
                    else if (reader.NodeType == XmlNodeType.Element && reader.Name == "display-name" && currentChannel != null)
                    {
                        string? displayNameText = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                        displayNames?.Add(new XmltvText { Text = displayNameText });
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "channel" && currentChannel != null)
                    {
                        currentChannel.DisplayNames = displayNames ?? [];
                        channels.Add(currentChannel);
                        currentChannel = null;
                        displayNames = null;
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "programme")
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error reading channels from file {EpgPath}", epgPath);
            }

            return channels;
        }

        private string? GetEPGFilePath(EPGFile epgFile)
        {
            string path = Path.Combine(FileDefinitions.EPG.DirectoryLocation, epgFile.Source);
            string? epgPath = GetFilePath(path);
            return epgPath;
        }

        public async Task<(int channelCount, int programCount)> ReadXmlCountsFromFileAsync(EPGFile epgFile)
        {
            string? epgPath = GetEPGFilePath(epgFile);
            return string.IsNullOrEmpty(epgPath) || !File.Exists(epgPath)
                ? ((int channelCount, int programCount))(-1, -1)
                : await ReadXmlCountsFromFileAsync(epgPath, epgFile.EPGNumber);
        }
        public async Task<(int channelCount, int programCount)> ReadXmlCountsFromFileAsync(string filepath, int epgNumber)
        {
            string? newFilePath = GetFilePath(filepath);
            if (newFilePath == null)
            {
                return (-1, -1);
            }

            if (cacheManager.StationChannelCounts.TryGetValue(epgNumber, out (int channelCount, int programmeCount) value))
            {
                return value;
            }
            cacheManager.StationChannelCounts[epgNumber] = new();

            int channelCount = 0;
            int programCount = 0;

            try
            {
                XmlReaderSettings settings = new()
                {
                    Async = true,
                    DtdProcessing = DtdProcessing.Ignore,
                    MaxCharactersFromEntities = 1024,
                    ConformanceLevel = ConformanceLevel.Document,
                };

                await using Stream? fileStream = await GetFileDataStream(newFilePath).ConfigureAwait(false);
                if (fileStream == null)
                {
                    return (-1, -1);
                }

                using XmlReader reader = XmlReader.Create(fileStream, settings);
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name.EqualsIgnoreCase("channel"))
                        {
                            channelCount++;
                        }
                        else if (reader.Name.EqualsIgnoreCase("programme"))
                        {
                            programCount++;
                        }
                    }
                }
                cacheManager.StationChannelCounts[epgNumber] = (channelCount, programCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to count elements in file {FilePath}", filepath);
                return (-1, -1);
            }

            return (channelCount, programCount);
        }

        public async Task<XMLTV?> ReadXmlFileAsync(EPGFile epgFile)
        {
            string? epgPath = GetEPGFilePath(epgFile);
            return string.IsNullOrEmpty(epgPath) || !File.Exists(epgPath) ? null : await ReadXmlFileAsync(epgPath);
        }

        public async Task<XMLTV?> ReadXmlFileAsync(string filepath)
        {
            string? newFilePath = GetFilePath(filepath);

            if (newFilePath == null)
            {
                return default;
            }

            try
            {
                XmlReaderSettings settings = new()
                {
                    Async = true, // Allow async operations
                    DtdProcessing = DtdProcessing.Ignore, // Ignore DTD processing                    
                    MaxCharactersFromEntities = 1024 // Limit the number of characters parsed from entities
                };

                XmlSerializer serializer = new(typeof(XMLTV));
                await using Stream? fileStream = await GetFileDataStream(filepath);
                if (fileStream == null)
                {
                    return default; // Return null if no valid stream is retrieved
                }

                // Now create the async XML reader and deserialize
                using XmlReader reader = XmlReader.Create(fileStream, settings);
                object? result = serializer.Deserialize(reader);

                if (result is null)
                {
                    return null;
                }
                XMLTV ret = (XMLTV)result;

                //List<XmltvChannel>? test = ret!.Channels.Where(a => a.Id.Contains("ABCWABC.us")).ToList();

                //List<XmltvProgramme> test2 = ret!.Programs.Where(a => a.Channel.Contains("ABCWABC.us")).ToList();

                // Return the deserialized object, cast to the expected type
                return (XMLTV)result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read file \"{filepath}\". Exception: {FileUtil.ReportExceptionMessages(ex)}");
                return default; // Return null if an error occurs
            }
        }

        public async Task<bool> IsXmlFileValid(string filepath)
        {
            string? newFilePath = GetFilePath(filepath);

            if (newFilePath == null)
            {
                return false; // Invalid file path
            }

            try
            {
                XmlReaderSettings settings = new()
                {
                    Async = false, // Synchronous check to validate XML structure
                    DtdProcessing = DtdProcessing.Ignore, // Ignore DTD processing
                    MaxCharactersFromEntities = 1024, // Limit to prevent entity expansion attacks
                    ConformanceLevel = ConformanceLevel.Document // Expecting a full XML document
                };

                await using Stream? fileStream = await GetFileDataStream(filepath);
                if (fileStream == null)
                {
                    return false; // No valid stream
                }

                using XmlReader reader = XmlReader.Create(fileStream, settings);
                while (reader.Read())
                {
                    // Simply iterate through the XML document to check for well-formedness
                }

                // If no exceptions were thrown, it means the XML is valid
                return true;
            }
            catch (XmlException ex)
            {
                Console.WriteLine($"XML validation error in file \"{filepath}\". Exception: {FileUtil.ReportExceptionMessages(ex)}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error during XML validation for file \"{filepath}\". Exception: {FileUtil.ReportExceptionMessages(ex)}");
                return false;
            }
        }
        public string? GetFilePath(string filepath)
        {
            // Check if the file exists at the original filepath
            if (File.Exists(filepath))
            {
                return filepath;
            }

            // Check if a .gz compressed file exists
            string gzFilePath = filepath + ".gz";
            if (File.Exists(gzFilePath))
            {
                return gzFilePath;
            }

            // Check if a .zip compressed file exists
            string zipFilePath = filepath + ".zip";
            if (File.Exists(zipFilePath))
            {
                return zipFilePath;
            }

            // Log the information if none of the files exist
            // Logger.WriteInformation($"File \"{filepath}\" does not exist in any checked format.");
            return null;
        }

        public async Task<Stream?> GetFileDataStream(string source)
        {
            string? filePath = GetExistingFilePath(source);
            if (filePath == null)
            {
                return null;
            }

            FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);

            if (await IsFileGzippedAsync(fileStream).ConfigureAwait(false))
            {
                return new GZipStream(fileStream, CompressionMode.Decompress);
            }
            else if (await IsFileZippedAsync(fileStream).ConfigureAwait(false))
            {
                ZipArchive zipArchive = new(fileStream, ZipArchiveMode.Read, leaveOpen: true);
                ZipArchiveEntry zipEntry = zipArchive.Entries[0]; // Read the first entry
                return zipEntry.Open(); // Stream remains open even after ZipArchive is disposed
            }

            return fileStream;
        }

        private static async Task<bool> IsFileGzippedAsync(Stream fileStream)
        {
            try
            {
                byte[] signature = new byte[3];
                await fileStream.ReadAsync(signature.AsMemory(0, 3)).ConfigureAwait(false);
                fileStream.Seek(0, SeekOrigin.Begin);
                return signature[0] == 0x1F && signature[1] == 0x8B && signature[2] == 0x08;
            }
            catch
            {
                return false;
            }
        }

        private static async Task<bool> IsFileZippedAsync(Stream fileStream)
        {
            try
            {
                byte[] signature = new byte[4];
                await fileStream.ReadAsync(signature.AsMemory(0, 4)).ConfigureAwait(false);
                fileStream.Seek(0, SeekOrigin.Begin);
                return signature[0] == 0x50 && signature[1] == 0x4B && signature[2] == 0x03 && signature[3] == 0x04;
            }
            catch
            {
                return false;
            }
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
                        file.Name.EndsWithIgnoreCase(ext) ||
                        file.Name.EndsWithIgnoreCase(ext + ".gz") ||
                        file.Name.EndsWithIgnoreCase(ext + ".zip")
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
                await using Stream? dataStream = await GetFileDataStream(fileName + "_temp");
                if (dataStream == null)
                {
                    return (false, null);
                }

                if (IsFileGzipped(fileName + "_temp"))
                {
                    if (!fileName.EndsWithIgnoreCase(".gz"))
                    {
                        fileName += ".gz";
                    }

                    await SaveStreamToFileAsync(dataStream, fileName, "none"); // No additional compression
                }
                else if (IsFileZipped(fileName + "_temp"))
                {
                    if (!fileName.EndsWithIgnoreCase(".zip"))
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

        private async Task SaveStreamToFileAsync(Stream inputStream, string fileName, string compression)
        {
            string compressedFileName = !compression.EqualsIgnoreCase("none") ? CheckNeedsCompression(fileName) : fileName;

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
            ArgumentNullException.ThrowIfNull(fileStream);

            try
            {
                // Ensure the stream supports reading and seeking
                if (!fileStream.CanRead || !fileStream.CanSeek)
                {
                    throw new NotSupportedException("Stream must support reading and seeking.");
                }

                byte[] signature = new byte[4];
                fileStream.ReadExactly(signature, 0, 4);
                fileStream.Seek(0, SeekOrigin.Begin);

                return signature[0] == 0x50 && signature[1] == 0x4B && signature[2] == 0x03 && signature[3] == 0x04;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsFileGzipped(Stream fileStream)
        {
            try
            {
                byte[] signature = new byte[3];
                fileStream.ReadExactly(signature, 0, 3);
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
                "gz" => fullName.EndsWithIgnoreCase(".gz") ? fullName : fullName + ".gz",
                "zip" => fullName.EndsWithIgnoreCase(".zip") ? fullName : fullName + ".zip",
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

            if (fullName.EndsWithIgnoreCase(".gz") || fullName.EndsWithIgnoreCase(".zip"))
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
                Timeout = TimeSpan.FromSeconds(240)
            };
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(settings.CurrentValue.ClientUserAgent);
            _httpClient.DefaultRequestHeaders.ExpectContinue = true;
        }
    }
}
