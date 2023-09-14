using FluentValidation;
using StreamMasterDomain.Models;

namespace StreamMasterApplication.Icons.Commands;

public record ReadDirectoryLogosRequest : IRequest
{
}

public class ReadDirectoryLogosRequestValidator : AbstractValidator<ReadDirectoryLogosRequest>
{
    public ReadDirectoryLogosRequestValidator()
    {
    }
}

public class ReadDirectoryLogosRequestHandler : IRequestHandler<ReadDirectoryLogosRequest>
{
    private readonly IMemoryCache _memoryCache;

    public ReadDirectoryLogosRequestHandler(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task Handle(ReadDirectoryLogosRequest command, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(BuildInfo.TVLogoDataFolder))
        {
            return;
        }


        DirectoryInfo dirInfo = new(BuildInfo.TVLogoDataFolder);

        List<TvLogoFile> tvLogos = new()
        {
            new TvLogoFile
            {
                Id=0,
                Source = BuildInfo.IconDefault,
                FileExists = true,
                Name = "Default Icon"
            },

            new TvLogoFile
            {
                Id=1,
                Source = "images/StreamMaster.png",
                FileExists = true,
                Name = "Stream Master"
            }
        };

        tvLogos.AddRange(await FileUtil.GetIconFilesFromDirectory(dirInfo, BuildInfo.TVLogoDataFolder, tvLogos.Count, cancellationToken).ConfigureAwait(false));

        _memoryCache.ClearIcons();
        _memoryCache.Set(tvLogos);
    }
}
