using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.M3UFiles.Queries;

public record GetM3UFileByIdQuery(int Id) : IRequest<M3UFileDto?>;

internal class GetM3UFileByIdQueryHandler : BaseRequestHandler, IRequestHandler<GetM3UFileByIdQuery, M3UFileDto?>
{

    public GetM3UFileByIdQueryHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper) { }

    public async Task<M3UFileDto?> Handle(GetM3UFileByIdQuery request, CancellationToken cancellationToken = default)
    {
        var m3uFile = await Repository.M3UFile.GetM3UFileByIdAsync(request.Id);
        var ret = Mapper.Map<M3UFileDto?>(m3uFile);
        return ret;
    }
}
