using FluentValidation;

using Microsoft.AspNetCore.Http;

using StreamMaster.SchedulesDirectAPI.Domain.EPG;

using StreamMasterDomain.Models;

using System.Web;

namespace StreamMasterApplication.EPGFiles.Commands;

public record CreateEPGFileRequest(string? Description, int EPGRank, IFormFile? FormFile, string Name, string? UrlSource) : IRequest<EPGFileDto?> { }
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

public class CreateEPGFileRequestHandler : BaseMediatorRequestHandler, IRequestHandler<CreateEPGFileRequest, EPGFileDto?>
{


    public CreateEPGFileRequestHandler(ILogger<CreateEPGFileRequest> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }


    public async Task<EPGFileDto?> Handle(CreateEPGFileRequest command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.UrlSource) && command.FormFile != null && command.FormFile.Length <= 0)
        {
            return null;
        }

        try
        {
            FileDefinition fd = FileDefinitions.EPG;

            string fullName = Path.Combine(fd.DirectoryLocation, command.Name + fd.FileExtension);

            EPGFile epgFile = new()
            {
                Description = command.Description ?? "",
                Name = command.Name,
                Source = command.Name + fd.FileExtension
            };

            if (command.FormFile != null)
            {
                epgFile.Source = command.Name + fd.FileExtension;

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
                    epgFile.LastDownloaded = File.GetLastWriteTime(fullName);
                    epgFile.FileExists = true;
                }
                else
                {
                    ++epgFile.DownloadErrors;
                    Logger.LogCritical("Exception EPG From URL {ex}", ex);
                }
            }

            epgFile.EPGRank = command.EPGRank;

            Tv? tv = await epgFile.GetTV().ConfigureAwait(false);
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

            epgFile.ChannelCount = tv.Channel != null ? tv.Channel.Count : 0;
            epgFile.ProgrammeCount = tv.Programme != null ? tv.Programme.Count : 0;

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
