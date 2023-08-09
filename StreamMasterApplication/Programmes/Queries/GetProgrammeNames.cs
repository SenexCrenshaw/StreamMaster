using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Programmes.Queries;

public record GetProgrammeNames : IRequest<IEnumerable<ProgrammeNameDto>>;

internal class GetProgrammeNamesHandler : IRequestHandler<GetProgrammeNames, IEnumerable<ProgrammeNameDto>>
{
    private readonly IMemoryCache _memoryCache;

    public GetProgrammeNamesHandler(

        IMemoryCache memoryCache
    )
    {
        _memoryCache = memoryCache;
    }

    public async Task<IEnumerable<ProgrammeNameDto>> Handle(GetProgrammeNames request, CancellationToken cancellationToken)
    {
        var programmes = _memoryCache.Programmes().Where(a => !string.IsNullOrEmpty(a.Channel) && a.StopDateTime > DateTime.Now.AddDays(-1)).ToList();
        if (programmes.Any())
        {
            var ret = programmes.GroupBy(a => a.Channel).Select(group => group.First()).Select(a => new ProgrammeNameDto
            {
                Channel = a.Channel,
                ChannelName = a.ChannelName,
                DisplayName = a.DisplayName
            });

            return ret.OrderBy(a => a.DisplayName);
        }
        return new List<ProgrammeNameDto>();
    }
}
