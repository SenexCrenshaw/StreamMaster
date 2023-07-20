using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Common.Extensions;

using StreamMasterDomain.Authentication;
using StreamMasterDomain.Dto;

using System.Linq.Expressions;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetStreamGroups() : IRequest<IEnumerable<StreamGroupDto>>;

internal class GetStreamGroupsHandler : IRequestHandler<GetStreamGroups, IEnumerable<StreamGroupDto>>
{
    protected Setting _setting = FileUtil.GetSetting();
    private readonly IAppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public GetStreamGroupsHandler(
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IAppDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _context = context;
    }

    public async Task<IEnumerable<StreamGroupDto>> Handle(GetStreamGroups request, CancellationToken cancellationToken = default)
    {
        
        var url = _httpContextAccessor.GetUrl();
        var ret = await _context.GetStreamGroupDtos(url,cancellationToken).ConfigureAwait(false);
     
        var encodedZero = 0.EncodeValue128(_setting.ServerKey);
        var zeroGroup = new StreamGroupDto
        {
            Id = 0,
            Name = "All",
            M3ULink = $"{url}/api/streamgroups/{encodedZero}/m3u.m3u",
            XMLLink = $"{url}/api/streamgroups/{encodedZero}/epg.xml",
            HDHRLink = $"{url}/api/streamgroups/{encodedZero}",
        };

        ret.Insert(0, zeroGroup);

        return ret;
    }
}
