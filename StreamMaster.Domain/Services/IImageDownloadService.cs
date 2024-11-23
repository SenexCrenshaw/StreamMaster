using Microsoft.Extensions.Hosting;

namespace StreamMaster.Domain.Services
{
    public interface IImageDownloadService : IHostedService, IDisposable
    {
        ImageDownloadServiceStatus ImageDownloadServiceStatus { get; }
        Task<bool> DownloadImageAsync(NameLogo nameLogo, CancellationToken cancellationToken);
    }
}