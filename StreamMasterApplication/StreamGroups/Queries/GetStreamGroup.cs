using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Common.Extensions;

using StreamMasterDomain.Authentication;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;

using System;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetStreamGroup(int Id) : IRequest<StreamGroupDto?>;

internal class GetStreamGroupHandler : IRequestHandler<GetStreamGroup, StreamGroupDto?>
{
    private readonly IAppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
 
    public GetStreamGroupHandler(
           IHttpContextAccessor httpContextAccessor,
        IAppDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public async Task<StreamGroupDto?> Handle(GetStreamGroup request, CancellationToken cancellationToken = default)
    {
        if (request.Id == 0) return new StreamGroupDto { Id = 0, Name = "All" };

        var url = _httpContextAccessor.GetUrl();
        var streamGroup = await _context.GetStreamGroupDto(request.Id, url, cancellationToken).ConfigureAwait(false);
           
        return streamGroup;
    }
}
