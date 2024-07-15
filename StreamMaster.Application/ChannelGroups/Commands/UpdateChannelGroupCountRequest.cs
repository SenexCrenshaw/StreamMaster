using FluentValidation;

namespace StreamMaster.Application.ChannelGroups.Commands;
public record UpdateChannelGroupCountRequest(ChannelGroupDto ChannelGroup)
    : IRequest<DataResponse<ChannelGroupDto>>;

[LogExecutionTimeAspect]
public class UpdateChannelGroupCountRequestHandler(ILogger<UpdateChannelGroupCountRequest> logger, IRepositoryWrapper repository)
    : IRequestHandler<UpdateChannelGroupCountRequest, DataResponse<ChannelGroupDto>>
{
    public async Task<DataResponse<ChannelGroupDto>> Handle(UpdateChannelGroupCountRequest request, CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<SMStream> smStreams = repository.SMStream.GetQuery(true).Where(vs => vs.Group == request.ChannelGroup.Name);

            var counts = await smStreams.GroupBy(vs => vs.IsHidden).Select(g => new { IsHidden = g.Key, Count = g.Count() }).ToListAsync(cancellationToken);
            int totalCount = counts.Sum(c => c.Count);
            int hiddenCount = counts.Find(c => c.IsHidden)?.Count ?? 0;

            request.ChannelGroup.TotalCount = totalCount;
            request.ChannelGroup.ActiveCount = totalCount - hiddenCount;
            request.ChannelGroup.HiddenCount = hiddenCount;

            await repository.SaveAsync();

            return DataResponse<ChannelGroupDto>.Success(request.ChannelGroup);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while processing UpdateChannelGroupCountRequest.");
            throw;
        }
    }
}