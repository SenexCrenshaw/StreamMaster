namespace StreamMaster.Application.StreamGroups.Commands;

public record CreateSTRMFilesRequest() : IRequest;

[LogExecutionTimeAspect]
public class CreateSTRMFilesRequestHandler(IStreamGroupService streamGroupService)
    : IRequestHandler<CreateSTRMFilesRequest>
{
    public async Task Handle(CreateSTRMFilesRequest request, CancellationToken cancellationToken)
    {
        await streamGroupService.SyncSTRMFilesAsync(cancellationToken);
    }
}