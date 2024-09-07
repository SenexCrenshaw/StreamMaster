namespace StreamMaster.Application.Programmes.Queries;

public record GetProgramme(string Channel) : IRequest<IEnumerable<XmltvProgramme>?>;

internal class GetProgrammeHandler(ISender sender) : IRequestHandler<GetProgramme, IEnumerable<XmltvProgramme>?>
{
    [LogExecutionTimeAspect]
    public async Task<IEnumerable<XmltvProgramme>?> Handle(GetProgramme request, CancellationToken cancellationToken)
    {
        IEnumerable<XmltvProgramme> cprogrammes = await sender.Send(new GetProgrammesRequest(), cancellationToken).ConfigureAwait(false);

        IEnumerable<XmltvProgramme> programmes = cprogrammes.Where(a => string.Equals(a.Channel, request.Channel, StringComparison.OrdinalIgnoreCase));
        if (programmes == null)
        {
            return null;
        }
        //SettingDto setting = await sender.Send(new GetSettingsRequest(), cancellationToken).ConfigureAwait(false);

        //List<LogoFileDto> icons = memoryCache.Icons();

        //foreach (Programme? prog in programmes.Where(a => a.M3ULogo.Any()))
        //{
        //    foreach (TvIcon progIcon in prog.M3ULogo)
        //    {
        //        if (progIcon != null && !string.IsNullOrEmpty(progIcon.Src))
        //        {
        //            LogoFileDto? icon = icons.FirstOrDefault(a => a.SMFileType == SMFileTypes.ProgrammeLogo && a.Source == progIcon.Src);
        //            if (icon == null)
        //            {
        //                continue;
        //            }
        //            string IconSource = $"/api/files/{(int)SMFileTypes.ProgrammeLogo}/{HttpUtility.UrlEncode(icon.Source)}";
        //            progIcon.Src = IconSource;
        //        }
        //    }
        //}

        return programmes;
    }
}
