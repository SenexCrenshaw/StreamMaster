using StreamMaster.Domain.API;

namespace StreamMaster.Domain.Services
{
    public interface ILogoService
    {
        string GetLogoUrl(string iconSource, string baseUrl);
        //void DownloadAndAdd(NameLogo nameLogo);
        void CacheSMChannelLogos();
        void AddLogo(string artworkUri, string title);
        ImagePath? GetValidImagePath(string URL, SMFileTypes? fileType = null);
        List<TvLogoFile> GetTvLogos();
        void AddLogo(LogoFileDto iconFile);
        //void AddIcons(List<LogoFileDto> newIconFiles);
        void ClearLogos();
        void ClearTvLogos();
        LogoFileDto? GetLogoBySource(string source);
        List<LogoFileDto> GetLogos(SMFileTypes? SMFileType = null);
        Task<bool> ReadDirectoryTVLogos(CancellationToken cancellationToken = default);
        void RemoveLogosByM3UFileId(int id);
        Task<DataResponse<bool>> BuildLogosCacheFromSMStreamsAsync(CancellationToken cancellationToken);
    }
}