namespace StreamMaster.Domain.Services;
public interface IOverlayStreamGenerator
{
    Task StartOverlayStreamAsync(string text, string imagePath, string outputPath, CancellationToken cancellationToken);
}