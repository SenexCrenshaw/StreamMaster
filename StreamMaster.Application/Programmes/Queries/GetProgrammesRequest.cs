namespace StreamMaster.Application.Programmes.Queries;

public record GetProgrammesRequest : IRequest<List<XmltvProgramme>>;

public class GetProgrammesRequestHandler()
: IRequestHandler<GetProgrammesRequest, List<XmltvProgramme>>
{
    public async Task<List<XmltvProgramme>> Handle(GetProgrammesRequest request, CancellationToken cancellationToken)
    {
        //List<EPGProgramme> programmes = MemoryCache.Programmes();

        //Setting setting = await GetSettingsAsync();
        //if (setting.SDSettings.SDEnabled)
        //{        

        //XMLTV? xmltv = schedulesDirect.CreateXmltv(httpContextAccessor.GetUrl());
        //if (xmltv == null)
        //{
        //    if (MemoryCache.GetSyncJobStatus().IsRunning)
        //    {
        //        await Task.Delay(1000, cancellationToken);
        //    }
        //    xmltv = schedulesDirect.CreateXmltv(httpContextAccessor.GetUrl());
        //    if (xmltv == null)
        //    {
        //        return [];
        //    }

        //}
        //List<XmltvProgramme> progs = xmltv.Programs.ToList();

        //List<XmltvProgramme> sdprogrammes = progs.OrderBy(a => a.Channel).ToList();
        //programmes = programmes.Concat(sdprogrammes).OrderBy(a => a.Channel).ToList();
        return await Task.FromResult(new List<XmltvProgramme>());
        //}

        //List<LogoFileDto> icons = MemoryCache.Icons();

        //foreach (Programme? prog in programmes.Where(a => a.M3ULogo.Any()))
        //{
        //    foreach (TvIcon progIcon in prog.M3ULogo)
        //    {
        //        if (progIcon != null && !string.IsNullOrEmpty(progIcon.Src))
        //        {
        //            LogoFileDto? icon = icons.Find(a => a.SMFileType == SMFileTypes.ProgrammeLogo && a.Source == progIcon.Src);
        //            if (icon == null)
        //            {
        //                continue;
        //            }
        //            progIcon.Src = $"/api/files/{(int)SMFileTypes.ProgrammeLogo}/{HttpUtility.UrlEncode(icon.Source)}";
        //        }
        //    }
        //}

    }
}