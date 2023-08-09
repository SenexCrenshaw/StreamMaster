using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetStreamGroups(StreamGroupParameters Parameters) : IRequest<IPagedList<StreamGroupDto>>;

internal class GetStreamGroupsHandler : BaseMediatorRequestHandler, IRequestHandler<GetStreamGroups, IPagedList<StreamGroupDto>>
{
    protected Setting _setting = FileUtil.GetSetting();

    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetStreamGroupsHandler(IHttpContextAccessor httpContextAccessor, ILogger<DeleteM3UFileHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IPagedList<StreamGroupDto>> Handle(GetStreamGroups request, CancellationToken cancellationToken = default)
    {
        string url = _httpContextAccessor.GetUrl();
        var ret = await Repository.StreamGroup.GetStreamGroupDtosPagedAsync(request.Parameters, url).ConfigureAwait(false);

        return ret;
    }
}