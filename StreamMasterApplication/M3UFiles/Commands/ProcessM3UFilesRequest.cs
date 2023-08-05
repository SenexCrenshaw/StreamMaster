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

public class ProcessM3UFilesRequestHandler : BaseMediatorRequestHandler, IRequestHandler<ProcessM3UFilesRequest>
{
    public ProcessM3UFilesRequestHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task Handle(ProcessM3UFilesRequest command, CancellationToken cancellationToken)
    {
        try
        {
            foreach (M3UFile m3uFile in await Repository.M3UFile.GetAllM3UFilesAsync().ConfigureAwait(false))
            {
                _ = await Sender.Send(new ProcessM3UFileRequest { Id = m3uFile.Id }, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "Error while processing M3U file");
        }
    }
}
