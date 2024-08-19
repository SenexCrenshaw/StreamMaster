using StreamMaster.Application.Programmes.Queries;

namespace StreamMaster.Application.EPG.Queries;

public record GetEPGChannelNameByDisplayName(string DisplayName) : IRequest<string?>;

internal class GetEPGChannelNameByDisplayNameHandler(ISender Sender)
    : IRequestHandler<GetEPGChannelNameByDisplayName, string?>
{
    public async Task<string?> Handle(GetEPGChannelNameByDisplayName request, CancellationToken cancellationToken = default)
    {
        IEnumerable<ProgrammeNameDto> programmeNames = await Sender.Send(new GetProgrammeNamesDto(), cancellationToken).ConfigureAwait(false);
        ProgrammeNameDto? pn = programmeNames.FirstOrDefault(a => a.DisplayName == request.DisplayName);
        if (pn == null)
        {
            pn = programmeNames.FirstOrDefault(a => a.ChannelName == request.DisplayName);
            if (pn == null)
            {
                return null;
            }
        }
        return pn.Channel;
    }
}
