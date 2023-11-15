using StreamMaster.SchedulesDirectAPI.Domain.EPG;

namespace StreamMasterApplication.Programmes.Queries;

public record GetProgramme(string Channel) : IRequest<IEnumerable<Programme>?>;

internal class GetProgrammeHandler(ISender sender) : IRequestHandler<GetProgramme, IEnumerable<Programme>?>
{
    [LogExecutionTimeAspect]
    public async Task<IEnumerable<Programme>?> Handle(GetProgramme request, CancellationToken cancellationToken)
    {
        IEnumerable<Programme> cprogrammes = await sender.Send(new GetProgrammesRequest(), cancellationToken).ConfigureAwait(false);

        IEnumerable<Programme> programmes = cprogrammes.Where(a => a.Channel.ToLower() == request.Channel.ToLower());
        if (programmes == null)
        {
            return null;
        }
        //SettingDto setting = await sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);

        //List<IconFileDto> icons = memoryCache.Icons();

        //foreach (Programme? prog in programmes.Where(a => a.Icon.Any()))
        //{
        //    foreach (TvIcon progIcon in prog.Icon)
        //    {
        //        if (progIcon != null && !string.IsNullOrEmpty(progIcon.Src))
        //        {
        //            IconFileDto? icon = icons.FirstOrDefault(a => a.SMFileType == SMFileTypes.ProgrammeIcon && a.Source == progIcon.Src);
        //            if (icon == null)
        //            {
        //                continue;
        //            }
        //            string IconSource = $"/api/files/{(int)SMFileTypes.ProgrammeIcon}/{HttpUtility.UrlEncode(icon.Source)}";
        //            progIcon.Src = IconSource;
        //        }
        //    }
        //}

        return programmes;
    }
}
