using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.Hubs;
using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;


namespace StreamMasterApplication.EPGFiles.Commands;

public class UpdateEPGFileRequest : BaseFileRequest, IRequest<EPGFilesDto?>
{
    public int? EPGRank { get; set; }
}

public class UpdateEPGFileRequestValidator : AbstractValidator<UpdateEPGFileRequest>
{
    public UpdateEPGFileRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().GreaterThanOrEqualTo(0);
    }
}

public class UpdateEPGFileRequestHandler : IRequestHandler<UpdateEPGFileRequest, EPGFilesDto?>
{
    private readonly IAppDbContext _context;
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly IMapper _mapper;

    public UpdateEPGFileRequestHandler(
  IMapper mapper,
   IHubContext<StreamMasterHub, IStreamMasterHub> hubContext,
        IAppDbContext context)
    {
        _mapper = mapper;
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<EPGFilesDto?> Handle(UpdateEPGFileRequest command, CancellationToken cancellationToken)
    {
        try
        {
            EPGFile? epgFile = await _context.EPGFiles.FindAsync(new object?[] { command.Id, cancellationToken }, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (epgFile == null)
            {
                return null;
            }
            bool isChanged = false;

            if (!string.IsNullOrEmpty(command.Description) && epgFile.Description != command.Description)
            {
                isChanged = true;
                epgFile.Description = command.Description;
            }

            if (!string.IsNullOrEmpty(command.Url) && epgFile.Url != command.Url)
            {
                isChanged = true;
                epgFile.OriginalSource = command.Url;
                epgFile.Url = command.Url;
            }
            if (!string.IsNullOrEmpty(command.MetaData) && epgFile.MetaData != command.MetaData)
            {
                isChanged = true;
                epgFile.MetaData = command.MetaData;
            }
            if (!string.IsNullOrEmpty(command.Name) && epgFile.Name != command.Name)
            {
                isChanged = true;
                epgFile.Name = command.Name;
            }

            if (command.AutoUpdate != null && epgFile.AutoUpdate != command.AutoUpdate)
            {
                isChanged = true;
                epgFile.AutoUpdate = (bool)command.AutoUpdate;
            }

            if (command.DaysToUpdate != null && epgFile.DaysToUpdate != command.DaysToUpdate)
            {
                isChanged = true;
                epgFile.DaysToUpdate = (int)command.DaysToUpdate;
            }

            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            EPGFilesDto ret = _mapper.Map<EPGFilesDto>(epgFile);
            if (isChanged)
            {
                await _hubContext.Clients.All.EPGFilesDtoUpdate(ret).ConfigureAwait(false);
            }

            return ret;
        }
        catch (Exception)
        {
        }
        return null;
    }
}
