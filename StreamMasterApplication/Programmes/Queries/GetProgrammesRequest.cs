using Microsoft.AspNetCore.Http;

using StreamMaster.SchedulesDirectAPI.Domain.XmltvXml;

using StreamMasterApplication.Common.Extensions;

namespace StreamMasterApplication.Programmes.Queries;

public record GetProgrammesRequest : IRequest<List<XmltvProgramme>>;

public class GetProgrammesRequestHandler(ILogger<GetProgrammesRequest> logger, IHttpContextAccessor httpContextAccessor, ISchedulesDirect schedulesDirect, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetProgrammesRequest, List<XmltvProgramme>>
{
    public async Task<List<XmltvProgramme>> Handle(GetProgrammesRequest request, CancellationToken cancellationToken)
    {
        //List<EPGProgramme> programmes = MemoryCache.Programmes();

        Setting setting = await GetSettingsAsync();
        //if (setting.SDSettings.SDEnabled)
        //{        

        XMLTV? xmltv = schedulesDirect.CreateXmltv(httpContextAccessor.GetUrl());
        if (xmltv == null)
        {
            if (MemoryCache.GetSyncJobStatus().IsRunning)
            {
                await Task.Delay(1000, cancellationToken);
            }
            xmltv = schedulesDirect.CreateXmltv(httpContextAccessor.GetUrl());
            if (xmltv == null)
            {
                return [];
            }

        }
        List<XmltvProgramme> progs = xmltv.Programs.ToList();

        List<XmltvProgramme> sdprogrammes = progs.OrderBy(a => a.Channel).ToList();
        //programmes = programmes.Concat(sdprogrammes).OrderBy(a => a.Channel).ToList();
        return sdprogrammes;
        //}

        //List<IconFileDto> icons = MemoryCache.Icons();

        //foreach (Programme? prog in programmes.Where(a => a.Icon.Any()))
        //{
        //    foreach (TvIcon progIcon in prog.Icon)
        //    {
        //        if (progIcon != null && !string.IsNullOrEmpty(progIcon.Src))
        //        {
        //            IconFileDto? icon = icons.Find(a => a.SMFileType == SMFileTypes.ProgrammeIcon && a.Source == progIcon.Src);
        //            if (icon == null)
        //            {
        //                continue;
        //            }
        //            progIcon.Src = $"/api/files/{(int)SMFileTypes.ProgrammeIcon}/{HttpUtility.UrlEncode(icon.Source)}";
        //        }
        //    }
        //}

        return [];
    }
}