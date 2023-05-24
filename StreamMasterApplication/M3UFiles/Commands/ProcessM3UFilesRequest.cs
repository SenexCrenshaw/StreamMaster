using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

namespace StreamMasterApplication.M3UFiles.Commands;

public record ProcessM3UFilesRequest : IRequest
{
}

public class ProcessM3UFilesRequestValidator : AbstractValidator<ProcessM3UFilesRequest>
{
    public ProcessM3UFilesRequestValidator()
    {
    }
}

public class ProcessM3UFilesRequestHandler : IRequestHandler<ProcessM3UFilesRequest>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<ProcessM3UFilesRequestHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;
    private readonly ISender _sender;

    public ProcessM3UFilesRequestHandler(
        ILogger<ProcessM3UFilesRequestHandler> logger,
        ISender sender,
        IMapper mapper,
          IPublisher publisher,
        IAppDbContext context
    )
    {
        _sender = sender;
        _publisher = publisher;
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Handle(ProcessM3UFilesRequest command, CancellationToken cancellationToken)
    {
        try
        {
            foreach (M3UFile m3uFile in _context.M3UFiles)
            {
                _ = await _sender.Send(new ProcessM3UFileRequest { M3UFileId = m3uFile.Id }, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error while processing M3U file");
        }
    }
}
