using StreamMaster.Domain.Pagination;

namespace StreamMaster.Application.SMStreams.Queries;


public record GetPagedSMStreams(SMStreamParameters Parameters) : IRequest<PagedResponse<SMStreamDto>>;

internal class GetPagedSMStreamsHandler(ILogger<GetPagedSMStreamsHandler> logger, IRepositoryWrapper Repository) : IRequestHandler<GetPagedSMStreams, PagedResponse<SMStreamDto>>
{
    public async Task<PagedResponse<SMStreamDto>> Handle(GetPagedSMStreams request, CancellationToken cancellationToken)
    {
        if (request.Parameters.PageSize == 0)
        {
            return Repository.SMStream.CreateEmptyPagedResponse();
        }

        PagedResponse<SMStreamDto> res = await Repository.SMStream.GetPagedSMStreams(request.Parameters, cancellationToken).ConfigureAwait(false);

        return res;
    }
}