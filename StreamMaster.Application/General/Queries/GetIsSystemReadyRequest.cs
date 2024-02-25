using StreamMaster.Domain.Configuration;

namespace StreamMaster.Application.General.Queries;

public record GetIsSystemReadyRequest : IRequest<bool>;

internal class GetIsSystemReadyRequestHandler : IRequestHandler<GetIsSystemReadyRequest, bool>
{


    public Task<bool> Handle(GetIsSystemReadyRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(BuildInfo.SetIsSystemReady);
    }
}
