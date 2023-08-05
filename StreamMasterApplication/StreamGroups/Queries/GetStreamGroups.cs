using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Authentication;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetStreamGroups() : IRequest<IEnumerable<StreamGroupDto>>;

internal class GetStreamGroupsHandler : BaseMediatorRequestHandler, IRequestHandler<GetStreamGroups, IEnumerable<StreamGroupDto>>
{
    protected Setting _setting = FileUtil.GetSetting();

    private readonly IHttpContextAccessor _httpContextAccessor;


    public GetStreamGroupsHandler(IHttpContextAccessor httpContextAccessor, ILogger<DeleteM3UFileHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<StreamGroupDto>> Handle(GetStreamGroups request, CancellationToken cancellationToken = default)
    {

        string url = _httpContextAccessor.GetUrl();
        List<StreamGroupDto> ret = await Repository.StreamGroup.GetStreamGroupDtos(url, cancellationToken).ConfigureAwait(false);

        string encodedZero = 0.EncodeValue128(_setting.ServerKey);
        StreamGroupDto zeroGroup = new()
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
