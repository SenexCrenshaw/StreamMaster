namespace StreamMasterApplication.Icons.Commands;

public record ReadDirectoryLogosRequest : IRequest
{
}

public class ReadDirectoryLogosRequestHandler(IMemoryCache memoryCache) : IRequestHandler<ReadDirectoryLogosRequest>
{
    public async Task Handle(ReadDirectoryLogosRequest command, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(BuildInfo.TVLogoDataFolder))
        {
            return;
        }

        DirectoryInfo dirInfo = new(BuildInfo.TVLogoDataFolder);

        List<TvLogoFile> tvLogos =
        [
            new TvLogoFile
            {
                Id = 0,
                Source = BuildInfo.IconDefault,
                FileExists = true,
                Name = "Default Icon"
            },

            new TvLogoFile
            {
                Id = 1,
                Source = "images/StreamMaster.png",
                FileExists = true,
                Name = "Stream Master"
            }
        ];

        tvLogos.AddRange(await FileUtil.GetIconFilesFromDirectory(dirInfo, dirInfo.FullName, tvLogos.Count, cancellationToken).ConfigureAwait(false));

        memoryCache.ClearIcons();
        memoryCache.SetCache(tvLogos);
    }
}
