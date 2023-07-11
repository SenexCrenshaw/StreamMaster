using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

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
        if (!Directory.Exists(Constants.TVLogoDirectory))
        {
            return;
        }

        Setting setting = FileUtil.GetSetting();
        DirectoryInfo dirInfo = new(Constants.TVLogoDirectory);

        List<TvLogoFile> tvLogos = new()
        {
            new TvLogoFile
            {
                Id=0,
                Url = "/" + Constants.IconDefault,
                OriginalSource = Constants.IconDefault,
                Source = Constants.IconDefault,
                FileExists = true,
                Name = "Default Icon"
            },

            new TvLogoFile
            {
                Id=1,
                Url =  "/" + setting.StreamMasterIcon,
                OriginalSource = setting.StreamMasterIcon,
                Source = setting.StreamMasterIcon,
                FileExists = true,
                Name = "Stream Master"
            }
        };

        tvLogos.AddRange(await FileUtil.GetIconFilesFromDirectory(dirInfo, Constants.TVLogoDirectory, tvLogos.Count, cancellationToken).ConfigureAwait(false));

        _memoryCache.ClearIcons();
        _memoryCache.Set(tvLogos);
    }
}
