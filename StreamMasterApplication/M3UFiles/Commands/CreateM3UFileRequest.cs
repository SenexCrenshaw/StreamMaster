using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;

using System.ComponentModel.DataAnnotations;
using System.Web;

namespace StreamMasterApplication.M3UFiles.Commands;

public class CreateM3UFileRequest : IRequest<bool>
{
    public string? Description { get; set; }
    public IFormFile? FormFile { get; set; }

    public int MaxStreamCount { get; set; }

    public string? MetaData { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public int? StartingChannelNumber { get; set; }
    public string? UrlSource { get; set; }
}

public class CreateM3UFileRequestValidator : AbstractValidator<CreateM3UFileRequest>
{
    public CreateM3UFileRequestValidator()
    {
        _ = RuleFor(v => v.Name)
            .MaximumLength(32)
            .NotEmpty();

        _ = RuleFor(v => v.UrlSource).NotEmpty().When(v => v.FormFile == null);
        _ = RuleFor(v => v.FormFile).NotNull().When(v => string.IsNullOrEmpty(v.UrlSource));
    }
}


public class CreateM3UFileRequestHandler : BaseMediatorRequestHandler, IRequestHandler<CreateM3UFileRequest, bool>
{

    public CreateM3UFileRequestHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<bool> Handle(CreateM3UFileRequest command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.UrlSource) && command.FormFile != null && command.FormFile.Length <= 0)
        {
            return false;
        }

        //Setting setting = FileUtil.GetSetting();
        try
        {
            FileDefinition fd = FileDefinitions.M3U;
            string fullName = Path.Combine(fd.DirectoryLocation, command.Name + fd.FileExtension);

            M3UFile m3UFile = new()
            {
                Description = command.Description ?? "",
                Name = command.Name,
                Source = command.Name + fd.FileExtension,
                StartingChannelNumber = command.StartingChannelNumber == null ? 1 : (int)command.StartingChannelNumber,
            };

            if (command.FormFile != null)
            {
                Logger.LogInformation("Adding M3U From Form: {fullName}", fullName);
                (bool success, Exception? ex) = await FormHelper.SaveFormFileAsync(command.FormFile!, fullName).ConfigureAwait(false);
                if (success)
                {
                    m3UFile.LastDownloaded = File.GetLastWriteTime(fullName);
                    m3UFile.FileExists = true;
                }
                else
                {
                    Logger.LogCritical("Exception M3U From Form {ex}", ex);
                    return false;
                }
            }
            else if (!string.IsNullOrEmpty(command.UrlSource))
            {
                string source = HttpUtility.UrlDecode(command.UrlSource);
                m3UFile.Url = source;
                m3UFile.LastDownloadAttempt = DateTime.Now;

                Logger.LogInformation("Add M3U From URL {command.UrlSource}", command.UrlSource);
                (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(source, fullName, cancellationToken).ConfigureAwait(false);
                if (success)
                {
                    m3UFile.LastDownloaded = File.GetLastWriteTime(fullName);
                    m3UFile.FileExists = true;
                }
                else
                {
                    ++m3UFile.DownloadErrors;
                    Logger.LogCritical("Exception M3U From URL {ex}", ex);
                }
            }

            m3UFile.MaxStreamCount = command.MaxStreamCount;

            var streams = await m3UFile.GetM3U().ConfigureAwait(false);
            if (streams == null || streams.Count == 0)
            {
                Logger.LogCritical("Exception M3U {fullName} format is not supported", fullName);
                //Bad M3U
                if (File.Exists(fullName))
                {
                    File.Delete(fullName);
                }
                return false;
            }

            if (m3UFile.StationCount != streams.Count)
            {
                m3UFile.StationCount = streams.Count;
            }


            Repository.M3UFile.CreateM3UFile(m3UFile);
            await Repository.SaveAsync().ConfigureAwait(false);

            M3UFileDto ret = Mapper.Map<M3UFileDto>(m3UFile);
            await Publisher.Publish(new M3UFileAddedEvent(ret), cancellationToken).ConfigureAwait(false);

            return true;
        }
        catch (Exception exception)
        {
            Logger.LogCritical("Exception M3U From Form {exception}", exception);
        }
        return false;
    }
}
