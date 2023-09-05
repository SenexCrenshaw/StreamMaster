using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Filtering;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository.EPG;

using System.Text.Json;

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

        IQueryable<Programme> programmes = MemoryCache.Programmes().Where(a => !string.IsNullOrEmpty(a.Channel)).AsQueryable();// && a.StopDateTime > DateTime.Now.AddDays(-1)).ToList();
        if (!string.IsNullOrEmpty(request.Parameters.JSONFiltersString))
        {
            List<DataTableFilterMetaData>? filters = JsonSerializer.Deserialize<List<DataTableFilterMetaData>>(request.Parameters.JSONFiltersString);
            if (filters != null)
            {
                DataTableFilterMetaData? nameFilter = filters.FirstOrDefault(a => a.FieldName == "name");
                if (nameFilter != null)
                {
                    nameFilter.FieldName = "DisplayName";
                }
                programmes = FilterHelper<Programme>.ApplyFiltersAndSort(programmes, filters, "DisplayName asc");
            }
        }

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
