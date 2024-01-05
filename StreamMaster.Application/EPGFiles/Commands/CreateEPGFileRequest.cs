using FluentValidation;

using Microsoft.AspNetCore.Http;

using StreamMaster.Domain.Color;

using System.Web;
namespace StreamMaster.Application.EPGFiles.Commands;

public record CreateEPGFileRequest(IFormFile? FormFile, string Name, int EPGNumber, string? UrlSource, string? Color) : IRequest<EPGFileDto?> { }
public class CreateEPGFileRequestValidator : AbstractValidator<CreateEPGFileRequest>
{
    public CreateEPGFileRequestValidator()
    {
        _ = RuleFor(v => v.Name)
            .MaximumLength(32)
            .NotEmpty();

        _ = RuleFor(v => v.UrlSource).NotEmpty().When(v => v.FormFile == null);
        _ = RuleFor(v => v.FormFile).NotNull().When(v => string.IsNullOrEmpty(v.UrlSource));
    }
}

public class CreateEPGFileRequestHandler(ILogger<CreateEPGFileRequest> logger, IXmltv2Mxf xmltv2Mxf, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<CreateEPGFileRequest, EPGFileDto?>
{
    public async Task<EPGFileDto?> Handle(CreateEPGFileRequest command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.UrlSource) && command.FormFile != null && command.FormFile.Length <= 0)
        {
            return null;
        }

        try
        {
            FileDefinition fd = FileDefinitions.EPG;

            string fullName = Path.Combine(fd.DirectoryLocation, command.Name + ".xmltv");

            int num = command.EPGNumber;

            if (await Repository.EPGFile.GetEPGFileByNumber(command.EPGNumber).ConfigureAwait(false) != null)
            {
                num = await Repository.EPGFile.GetNextAvailableEPGNumberAsync(cancellationToken).ConfigureAwait(false);
            }

            EPGFile epgFile = new()
            {

                Name = command.Name,
                Source = command.Name + ".xmltv",
                Color = command.Color ?? ColorHelper.GetColor(command.Name),
                EPGNumber = num,
            };

            if (command.FormFile != null)
            {
                epgFile.Source = command.Name + ".xmltv";

                Logger.LogInformation("Adding EPG From Form: {fullName}", fullName);
                (bool success, Exception? ex) = await FormHelper.SaveFormFileAsync(command.FormFile!, fullName).ConfigureAwait(false);
                if (success)
                {
                    epgFile.LastDownloaded = File.GetLastWriteTime(fullName);
                    epgFile.FileExists = true;
                }
                else
                {
                    Logger.LogCritical("Exception EPG From Form {ex}", ex);
                    return null;
                }
            }
            else if (!string.IsNullOrEmpty(command.UrlSource))
            {
                string source = HttpUtility.UrlDecode(command.UrlSource);
                epgFile.Url = source;
                epgFile.LastDownloadAttempt = DateTime.Now;

                Logger.LogInformation("Add EPG From URL {command.UrlSource}", command.UrlSource);
                (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(source, fullName, cancellationToken).ConfigureAwait(false);
                if (success)
                {
                    //If EPG file gzipped decompress it
                    if (FileUtil.IsFileGzipped(fullName))
                    {
                        var fi = new FileInfo(fullName);
                        FileUtil.Decompress(fi);
                        File.Delete(fullName);
                        fullName = fullName.Remove(fullName.Length - fi.Extension.Length);
                    }
                    epgFile.LastDownloaded = File.GetLastWriteTime(fullName);
                    epgFile.FileExists = true;
                }
                else
                {
                    ++epgFile.DownloadErrors;
                    Logger.LogCritical("Exception EPG From URL {ex}", ex);
                }
            }

            XMLTV? tv = xmltv2Mxf.ConvertToMxf(Path.Combine(FileDefinitions.EPG.DirectoryLocation, epgFile.Source), epgFile.EPGNumber);
            if (tv == null)
            {
                Logger.LogCritical("Exception EPG {fullName} format is not supported", fullName);
                //Bad EPG
                if (File.Exists(fullName))
                {
                    File.Delete(fullName);
                }
                return null;
            }

            epgFile.ChannelCount = tv.Channels != null ? tv.Channels.Count : 0;
            epgFile.ProgrammeCount = tv.Programs != null ? tv.Programs.Count : 0;


            Repository.EPGFile.CreateEPGFile(epgFile);
            _ = await Repository.SaveAsync().ConfigureAwait(false);
            epgFile.WriteJSON();

            EPGFileDto ret = Mapper.Map<EPGFileDto>(epgFile);
            await Publisher.Publish(new EPGFileAddedEvent(ret), cancellationToken).ConfigureAwait(false);

            return ret;
        }
        catch (Exception exception)
        {
            Logger.LogCritical("Exception EPG From Form {exception}", exception);
        }

        return null;
    }
}
