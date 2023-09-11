﻿using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Hubs;
using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.EPGFiles.Commands;

public class UpdateEPGFileRequest : BaseFileRequest, IRequest<EPGFileDto?>
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

public class UpdateEPGFileRequestHandler : BaseMemoryRequestHandler, IRequestHandler<UpdateEPGFileRequest, EPGFileDto?>
{

    public UpdateEPGFileRequestHandler(ILogger<UpdateEPGFileRequest> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper,settingsService, publisher, sender, hubContext, memoryCache) { }


    public async Task<EPGFileDto?> Handle(UpdateEPGFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            EPGFile? epgFile = await Repository.EPGFile.GetEPGFileByIdAsync(request.Id).ConfigureAwait(false);
            if (epgFile == null)
            {
                return null;
            }

            bool isChanged = false;
            bool isNameChanged = false;

            if (!string.IsNullOrEmpty(request.Description) && epgFile.Description != request.Description)
            {
                isChanged = true;
                epgFile.Description = request.Description;
            }

            if (request.Url != null && epgFile.Url != request.Url)
            {
                isChanged = true;
                epgFile.Url = request.Url == "" ? null : request.Url;
            }

            if (!string.IsNullOrEmpty(request.Name) && epgFile.Name != request.Name)
            {
                isChanged = true;
                isNameChanged = true;
                epgFile.Name = request.Name;
            }

            if (request.AutoUpdate != null && epgFile.AutoUpdate != request.AutoUpdate)
            {
                isChanged = true;
                epgFile.AutoUpdate = (bool)request.AutoUpdate;
            }

            if (request.HoursToUpdate != null && epgFile.HoursToUpdate != request.HoursToUpdate)
            {
                isChanged = true;
                epgFile.HoursToUpdate = (int)request.HoursToUpdate;
            }

            Repository.EPGFile.UpdateEPGFile(epgFile);
            _ = await Repository.SaveAsync().ConfigureAwait(false);
            epgFile.WriteJSON();
            EPGFileDto ret = Mapper.Map<EPGFileDto>(epgFile);

            if (isNameChanged)
            {
                List<ChannelLogoDto> programmes = MemoryCache.ChannelLogos();
                _ = programmes.RemoveAll(a => a.EPGFileId == epgFile.Id);
                MemoryCache.Set(programmes);

                List<ChannelLogoDto> channels = MemoryCache.ChannelLogos();
                _ = channels.RemoveAll(a => a.EPGFileId == epgFile.Id);
                MemoryCache.Set(channels);

                List<ChannelLogoDto> channelLogos = MemoryCache.ChannelLogos();
                _ = channelLogos.RemoveAll(a => a.EPGFileId == epgFile.Id);
                MemoryCache.Set(channelLogos);

                List<IconFileDto> programmeIcons = MemoryCache.ProgrammeIcons();
                _ = programmeIcons.RemoveAll(a => a.FileId == epgFile.Id);
                MemoryCache.SetProgrammeLogos(programmeIcons);

                await Publisher.Publish(new EPGFileAddedEvent(ret), cancellationToken).ConfigureAwait(false);
            }

            if (isChanged)
            {
                await HubContext.Clients.All.EPGFilesRefresh().ConfigureAwait(false);
            }

            return ret;
        }
        catch (Exception)
        {
        }
        return null;
    }
}
