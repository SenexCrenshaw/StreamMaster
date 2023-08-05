using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Repository;
using StreamMasterDomain.Repository.EPG;

using System.ComponentModel.DataAnnotations;
using System.Web;

namespace StreamMasterApplication.EPGFiles.Commands;

public class AddEPGFileRequest : IRequest<EPGFilesDto?>
{
    public string? Description { get; set; }

    public int EPGRank { get; set; }
    public IFormFile? FormFile { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? UrlSource { get; set; }
}

public class AddEPGFileRequestValidator : AbstractValidator<AddEPGFileRequest>
{
    public AddEPGFileRequestValidator()
    {
        _ = RuleFor(v => v.Name)
            .MaximumLength(32)
            .NotEmpty();

        _ = RuleFor(v => v.UrlSource).NotEmpty().When(v => v.FormFile == null);
        _ = RuleFor(v => v.FormFile).NotNull().When(v => string.IsNullOrEmpty(v.UrlSource));
    }
}

public class AddEPGFileRequestHandler : IRequestHandler<AddEPGFileRequest, EPGFilesDto?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<AddEPGFileRequestHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;

    public AddEPGFileRequestHandler(
        ILogger<AddEPGFileRequestHandler> logger,
         IMapper mapper,
         IPublisher publisher,
        IAppDbContext context)
    {
        _logger = logger;
        _publisher = publisher;
        _mapper = mapper;
        _context = context;
    }

    public async Task<EPGFilesDto?> Handle(AddEPGFileRequest command, CancellationToken cancellationToken)
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

                _logger.LogInformation("Adding EPG From Form: {fullName}", fullName);
                (bool success, Exception? ex) = await FormHelper.SaveFormFileAsync(command.FormFile!, fullName).ConfigureAwait(false);
                if (success)
                {
                    epgFile.LastDownloaded = File.GetLastWriteTime(fullName);
                    epgFile.FileExists = true;
                }
                else
                {
                    _logger.LogCritical("Exception EPG From Form {ex}", ex);
                    return null;
                }
            }
            else if (!string.IsNullOrEmpty(command.UrlSource))
            {
                string source = HttpUtility.UrlDecode(command.UrlSource);
                epgFile.Url = source;
                epgFile.LastDownloadAttempt = DateTime.Now;

                _logger.LogInformation("Add EPG From URL {command.UrlSource}", command.UrlSource);
                (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(source, fullName, cancellationToken).ConfigureAwait(false);
                if (success)
                {
                    epgFile.LastDownloaded = File.GetLastWriteTime(fullName);
                    epgFile.FileExists = true;
                }
                else
                {
                    ++epgFile.DownloadErrors;
                    _logger.LogCritical("Exception EPG From URL {ex}", ex);
                }
            }

            epgFile.EPGRank = command.EPGRank;

            Tv? tv = await epgFile.GetTV().ConfigureAwait(false);
            if (tv == null)
            {
                _logger.LogCritical("Exception EPG {fullName} format is not supported", fullName);
                //Bad EPG
                if (File.Exists(fullName))
                {
                    File.Delete(fullName);
                }
                return null;
            }

            epgFile.ChannelCount = tv.Channel != null ? tv.Channel.Count : 0;
            epgFile.ProgrammeCount = tv.Programme != null ? tv.Programme.Count : 0;

            _ = _context.EPGFiles.Add(epgFile);
            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            EPGFilesDto ret = _mapper.Map<EPGFilesDto>(epgFile);
            await _publisher.Publish(new EPGFileAddedEvent(ret), cancellationToken).ConfigureAwait(false);

            return ret;
        }
        catch (Exception exception)
        {
            _logger.LogCritical("Exception EPG From Form {exception}", exception);
        }

        return null;
    }
}
