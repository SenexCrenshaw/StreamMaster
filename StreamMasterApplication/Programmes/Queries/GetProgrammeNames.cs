using MediatR;

using Microsoft.Extensions.Caching.Memory;

namespace StreamMasterApplication.Programmes.Queries;

public class ProgrammeName
{
    public string Channel { get; set; }
    public string ChannelName { get; set; }
    public string DisplayName { get; set; }
}

public record GetProgrammeNames : IRequest<IEnumerable<ProgrammeName>>;

internal class GetProgrammeNamesHandler : IRequestHandler<GetProgrammeNames, IEnumerable<ProgrammeName>>
{
    private readonly IMemoryCache _memoryCache;

    public GetProgrammeNamesHandler(

        IMemoryCache memoryCache
    )
    {
        _memoryCache = memoryCache;
    }

    public async Task<IEnumerable<ProgrammeName>> Handle(GetProgrammeNames request, CancellationToken cancellationToken)
    {
        var programmes = _memoryCache.Programmes().Where(a => !string.IsNullOrEmpty(a.Channel) && a.StopDateTime > DateTime.Now.AddDays(-1)).ToList();
        if (programmes.Any())
        {
            var ret = programmes.GroupBy(a => a.Channel).Select(group => group.First()).Select(a => new ProgrammeName
            {
                Channel = a.Channel,
                ChannelName = a.ChannelName,
                DisplayName = a.DisplayName
            });

            return ret;
        }
        return new List<ProgrammeName>();
    }
}
