using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Authentication;
using StreamMasterDomain.Configuration;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities;

using StreamMasterInfrastructure.Extensions;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetStreamGroups() : IRequest<IEnumerable<StreamGroupDto>>;

internal class GetStreamGroupsHandler : IRequestHandler<GetStreamGroups, IEnumerable<StreamGroupDto>>
{
    private readonly IConfigFileProvider _configFileProvider;
    private readonly IAppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public GetStreamGroupsHandler(
        IMapper mapper, 
        IConfigFileProvider configFileProvider,
        IHttpContextAccessor httpContextAccessor,
        IAppDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _context = context;
        _configFileProvider = configFileProvider;
    }

    public async Task<IEnumerable<StreamGroupDto>> Handle(GetStreamGroups request, CancellationToken cancellationToken = default)
    {
        var ret = await _context.StreamGroups
           .Include(a => a.VideoStreams)
           .Include(a => a.ChannelGroups)
           .AsNoTracking()
           .ProjectTo<StreamGroupDto>(_mapper.ConfigurationProvider)
           .OrderBy(x => x.Name)
           .ToListAsync(cancellationToken).ConfigureAwait(false);

        var url = _httpContextAccessor.GetUrl();

        foreach (var streamGroup in ret)
        {
            var existingIds = streamGroup.VideoStreams.Select(a => a.Id).ToList();

            foreach (var channegroup in streamGroup.ChannelGroups)
            {
                var streams = _context.VideoStreams
                    .Where(a => !existingIds.Contains(a.Id) && a.User_Tvg_group == channegroup.Name)
                    .AsNoTracking()
                    .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider)
                    .ToList();
                foreach (var stream in streams)
                {
                    stream.IsReadOnly = true;
                }
                streamGroup.VideoStreams.AddRange(streams);
            }
            var encodedStreamGroupNumber = streamGroup.StreamGroupNumber.EncodeValue128(_configFileProvider.Setting.ServerKey);
            streamGroup.M3ULink = $"{url}/api/streamgroups/{encodedStreamGroupNumber}/m3u.m3u";
            streamGroup.XMLLink = $"{url}/api/streamgroups/{encodedStreamGroupNumber}/epg.xml";
            streamGroup.HDHRLink = $"{url}/api/streamgroups/{encodedStreamGroupNumber}";
        }

        var encodedZero = 0.EncodeValue128(_configFileProvider.Setting.ServerKey);
        var zeroGroup = new StreamGroupDto
        {
            Id = 0,
            Name = "All",
            M3ULink = $"{url}/api/streamgroups/{encodedZero}/m3u.m3u",
            XMLLink = $"{url}/api/streamgroups/{encodedZero}/epg.xml",
            HDHRLink = $"{url}/api/streamgroups/{encodedZero}",
        };

        ret.Insert(0,zeroGroup);

        return ret;
    }

}
