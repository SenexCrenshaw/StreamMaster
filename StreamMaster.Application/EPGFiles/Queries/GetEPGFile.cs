﻿namespace StreamMaster.Application.EPGFiles.Queries;

public record GetEPGFile(int Id) : IRequest<EPGFileDto?>;

internal class GetEPGFileHandler(ILogger<GetEPGFile> logger, IRepositoryWrapper Repository, ISchedulesDirectDataService schedulesDirectDataService, IMapper Mapper)
    : IRequestHandler<GetEPGFile, EPGFileDto?>
{
    public async Task<EPGFileDto?> Handle(GetEPGFile request, CancellationToken cancellationToken = default)
    {
        EPGFile? epgFile = await Repository.EPGFile.GetEPGFileById(request.Id).ConfigureAwait(false);
        if (epgFile == null)
        {
            return null;
        }
        EPGFileDto epgFileDto = Mapper.Map<EPGFileDto>(epgFile);
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.GetEPGData(epgFileDto.Id);
        int programmes = schedulesDirectData.Programs.Count;
        int channels = schedulesDirectData.Services.Count;

        //var c = await Sender.Send(new GetProgrammesRequest(), cancellationToken).ConfigureAwait(false);
        //  var proprammes = c.Where(a => a.EPGFileId == epgFile.Id).ToList();
        //if (proprammes.Any())
        //{
        //    epgFileDto.EPGStartDate = proprammes.Min(a => a.s);
        //    epgFileDto.EPGStopDate = proprammes.Max(a => a.StopDateTime);
        //}
        epgFileDto.ProgrammeCount = programmes;
        epgFileDto.ChannelCount = channels;
        return epgFileDto;
    }
}
