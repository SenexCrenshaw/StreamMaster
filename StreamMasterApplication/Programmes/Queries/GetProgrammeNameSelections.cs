using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository.EPG;

namespace StreamMasterApplication.Programmes.Queries;

public record GetProgrammeNameSelections(ProgrammeParameters Parameters) : IRequest<PagedResponse<ProgrammeNameDto>>;

internal class GetProgrammeNameSelectionsHandler : BaseMemoryRequestHandler, IRequestHandler<GetProgrammeNameSelections, PagedResponse<ProgrammeNameDto>>
{

    public GetProgrammeNameSelectionsHandler(ILogger<GetProgrammeNameSelectionsHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public async Task<PagedResponse<ProgrammeNameDto>> Handle(GetProgrammeNameSelections request, CancellationToken cancellationToken)
    {
        if (request.Parameters.PageSize == 0)
        {
            PagedResponse<ProgrammeNameDto> emptyResponse = new();
            emptyResponse.TotalItemCount = MemoryCache.Programmes().Count;
            return emptyResponse;
        }

        List<ProgrammeNameDto> ret = new();

        List<Programme> programmes = MemoryCache.Programmes().Where(a => !string.IsNullOrEmpty(a.Channel)).ToList();// && a.StopDateTime > DateTime.Now.AddDays(-1)).ToList();

        IEnumerable<string> names = programmes.Select(a => a.Channel).Distinct();
        foreach (string? name in names)
        {
            Programme? programme = programmes.FirstOrDefault(a => a.Channel == name);
            if (programme != null)
            {
                ProgrammeNameDto programmeDto = Mapper.Map<ProgrammeNameDto>(programme);
                ret.Add(programmeDto);
            }
        }

        IPagedList<ProgrammeNameDto> test = await ret.OrderBy(a => a.DisplayName).ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize).ConfigureAwait(false);

        PagedResponse<ProgrammeNameDto> pagedResponse = test.ToPagedResponse(test.TotalItemCount);
        return pagedResponse;


    }
}
