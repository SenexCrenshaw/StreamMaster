﻿namespace StreamMaster.Domain.Services
{
    public interface IImageDownloadService
    {
        Task StopAsync(CancellationToken cancellationToken);
        void Start();
        ImageDownloadServiceStatus GetStatus();
    }
}