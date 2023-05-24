using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.Hubs;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.M3UFiles.Commands;

public class UpdateM3UFileRequest : BaseFileRequest, IRequest<M3UFilesDto?>
{
    public int? MaxStreamCount { get; set; }
}

public class UpdateM3UFileRequestValidator : AbstractValidator<UpdateM3UFileRequest>
{
    public UpdateM3UFileRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().GreaterThanOrEqualTo(0);
    }
}

public class UpdateM3UFileRequestHandler : IRequestHandler<UpdateM3UFileRequest, M3UFilesDto?>
{
    private readonly IAppDbContext _context;
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly IMapper _mapper;

    public UpdateM3UFileRequestHandler(
     IMapper mapper,
      IHubContext<StreamMasterHub, IStreamMasterHub> hubContext,
        IAppDbContext context)
    {
        _mapper = mapper;
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<M3UFilesDto?> Handle(UpdateM3UFileRequest command, CancellationToken cancellationToken)
    {
        try
        {
            M3UFile? m3UFile = await _context.M3UFiles.FindAsync(new object?[] { command.Id, cancellationToken }, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (m3UFile == null)
            {
                return null;
            }

            bool isChanged = false;

            if (!string.IsNullOrEmpty(command.Description) && m3UFile.Description != command.Description)
            {
                isChanged = true;
                m3UFile.Description = command.Description;
            }

            if (!string.IsNullOrEmpty(command.Url) && m3UFile.Url != command.Url)
            {
                isChanged = true;
                m3UFile.OriginalSource = command.Url;
                m3UFile.Url = command.Url;
            }
            if (!string.IsNullOrEmpty(command.MetaData) && m3UFile.MetaData != command.MetaData)
            {
                isChanged = true;
                m3UFile.MetaData = command.MetaData;
            }
            if (!string.IsNullOrEmpty(command.Name) && m3UFile.Name != command.Name)
            {
                isChanged = true;
                m3UFile.Name = command.Name;
            }

            if (command.MaxStreamCount != null && m3UFile.MaxStreamCount != command.MaxStreamCount)
            {
                isChanged = true;
                m3UFile.MaxStreamCount = (int)command.MaxStreamCount;
            }

            if (command.AutoUpdate != null && m3UFile.AutoUpdate != command.AutoUpdate)
            {
                isChanged = true;
                m3UFile.AutoUpdate = (bool)command.AutoUpdate;
            }

            if (command.DaysToUpdate != null && m3UFile.DaysToUpdate != command.DaysToUpdate)
            {
                isChanged = true;
                m3UFile.DaysToUpdate = (int)command.DaysToUpdate;
            }

            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            M3UFilesDto ret = _mapper.Map<M3UFilesDto>(m3UFile);
            if (isChanged)
            {
                await _hubContext.Clients.All.M3UFilesDtoUpdate(ret).ConfigureAwait(false);
            }

            return ret;
        }
        catch (Exception)
        {
        }
        return null;
    }
}
