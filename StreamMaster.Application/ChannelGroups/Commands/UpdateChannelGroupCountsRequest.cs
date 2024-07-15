using FluentValidation;

namespace StreamMaster.Application.ChannelGroups.Commands;

public record UpdateChannelGroupCountsRequest(List<ChannelGroup>? ChannelGroups = null)
    : IRequest;

[LogExecutionTimeAspect]
public class UpdateChannelGroupCountsRequestHandler(ILogger<UpdateChannelGroupCountsRequest> Logger, IRepositoryWrapper Repository)
    : IRequestHandler<UpdateChannelGroupCountsRequest>
{
    public async Task Handle(UpdateChannelGroupCountsRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            List<ChannelGroup> cgs = request.ChannelGroups == null || request.ChannelGroups.Count == 0
                ? [.. Repository.ChannelGroup.GetQuery(true)]
                : request.ChannelGroups;

            if (cgs.Count != 0)
            {
                List<string> cgNames = cgs.ConvertAll(a => a.Name);

                List<SMStream> smStreams = [.. Repository.SMStream.GetQuery().Where(a => cgNames.Contains(a.Group))];

                foreach (ChannelGroup cg in cgs)
                {
                    if (cg == null)
                    {
                        continue;
                    }

                    List<SMStream> relevantStreams = smStreams.Where(vs => vs.Group == cg.Name).ToList();

                    var counts = relevantStreams.GroupBy(vs => vs.IsHidden).Select(g => new { IsHidden = g.Key, Count = g.Count() }).ToList();
                    int totalCount = counts.Sum(c => c.Count);
                    int hiddenCount = counts.Find(c => c.IsHidden)?.Count ?? 0;
                    cg.TotalCount = totalCount;
                    cg.ActiveCount = totalCount - hiddenCount;
                    cg.HiddenCount = hiddenCount;
                }

                await Repository.SaveAsync();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error while handling UpdateChannelGroupCountsRequest.");

            throw;
        }
    }
}