using StreamMaster.Application.Programmes.Queries;

namespace StreamMaster.Application.EPG.Queries;

public record GetEPGNameTvgName(string User_Tvg_Name) : IRequest<string?>;

internal class GetEPGNameTvgNameHandler(ILogger<GetEPGNameTvgName> logger, ISender Sender)
{
    public async Task<string?> Handle(GetEPGNameTvgName request, CancellationToken cancellationToken = default)
    {
        IEnumerable<ProgrammeNameDto> programmeNames = await Sender.Send(new GetProgrammeNamesDto(), cancellationToken).ConfigureAwait(false);

        ProgrammeNameDto? pn = programmeNames.FirstOrDefault(a => a.DisplayName == request.User_Tvg_Name);
        if (pn == null)
        {
            pn = programmeNames.FirstOrDefault(a => a.ChannelName == request.User_Tvg_Name);
            if (pn == null)
            {
                return null;
            }
        }
        return request.User_Tvg_Name;
    }
}
