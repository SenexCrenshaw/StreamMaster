using StreamMaster.Domain.API;

namespace StreamMaster.Domain.Services
{
    /// <summary>
    /// Provides methods for managing and caching logo files.
    /// </summary>
    public interface ILogoService
    {
        /// <summary>
        /// Adds a new logo based on the specified artwork URI and title.
        /// </summary>
        /// <param name="artworkUri">The URI of the artwork.</param>
        /// <param name="title">The title associated with the logo.</param>
        void AddLogo(string artworkUri, string title);

        /// <summary>
        /// Adds a new logo using the specified <see cref="LogoFileDto"/>.
        /// </summary>
        /// <param name="logoFile">The logo file DTO containing logo details.</param>
        void AddLogo(LogoFileDto logoFile);

        /// <summary>
        /// Builds the logo cache using the current streams asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The result contains a <see cref="DataResponse{Boolean}"/> indicating the success of the operation.</returns>
        Task<DataResponse<bool>> BuildLogosCacheFromSMStreamsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Caches the logos for SM Channels.
        /// </summary>
        void CacheSMChannelLogos();

        /// <summary>
        /// Clears all logos from the cache.
        /// </summary>
        void ClearLogos();

        /// <summary>
        /// Clears all TV logos from the cache.
        /// </summary>
        void ClearTvLogos();

        /// <summary>
        /// Retrieves the logo corresponding to the specified source.
        /// </summary>
        /// <param name="source">The source URL of the logo.</param>
        /// <returns>The <see cref="LogoFileDto"/> if found; otherwise, null.</returns>
        LogoFileDto? GetLogoBySource(string source);

        /// <summary>
        /// Retrieves a list of logos of the specified file type.
        /// </summary>
        /// <param name="SMFileType">The type of the logos to retrieve.</param>
        /// <returns>A list of <see cref="LogoFileDto"/> objects.</returns>
        List<LogoFileDto> GetLogos(SMFileTypes? SMFileType = null);

        /// <summary>
        /// Generates a URL for a logo based on the specified icon source and base URL.
        /// </summary>
        /// <param name="iconSource">The source of the icon.</param>
        /// <param name="baseUrl">The base URL to prepend to the icon source.</param>
        /// <returns>The full URL of the logo.</returns>
        string GetLogoUrl(string iconSource, string baseUrl);

        /// <summary>
        /// Retrieves a list of TV logos.
        /// </summary>
        /// <returns>A list of <see cref="TvLogoFile"/> objects.</returns>
        List<TvLogoFile> GetTvLogos();

        /// <summary>
        /// Retrieves a valid image path for the specified URL and file type.
        /// </summary>
        /// <param name="URL">The URL of the image.</param>
        /// <param name="fileType">The type of the file. If null, the file type is determined automatically.</param>
        /// <returns>The valid <see cref="ImagePath"/>, or null if not found.</returns>
        ImagePath? GetValidImagePath(string URL, SMFileTypes? fileType = null);

        /// <summary>
        /// Reads the directory containing TV logos asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The result contains a boolean indicating the success of the operation.</returns>
        Task<bool> ReadDirectoryTVLogos(CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes logos associated with the specified M3U file ID.
        /// </summary>
        /// <param name="id">The ID of the M3U file.</param>
        void RemoveLogosByM3UFileId(int id);
    }
}
