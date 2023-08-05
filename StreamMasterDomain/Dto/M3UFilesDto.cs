using StreamMasterDomain.Attributes;
using StreamMasterDomain.Mappings;

namespace StreamMasterDomain.Dto;

public class PagedDto
{
    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
}

[RequireAll]
public class M3UFileDto : BaseFileDto, IMapFrom<M3UFile>
{
    public int StartingChannelNumber { get; set; }
    public int MaxStreamCount { get; set; }

    public int StationCount { get; set; }

}
