namespace StreamMaster.Domain.Services
{
    public interface IImageDownloadService
    {
        void Start();
        Task StopAsync(CancellationToken cancellationToken);
        //Task StartAsync(CancellationToken cancellationToken);
        ImageDownloadServiceStatus ImageDownloadServiceStatus { get; }
    }
}