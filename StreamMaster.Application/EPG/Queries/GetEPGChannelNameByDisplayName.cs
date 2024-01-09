﻿using StreamMaster.Application.Programmes.Queries;

namespace StreamMaster.Application.EPG.Queries;

public record GetEPGChannelNameByDisplayName(string displayName) : IRequest<string?>;

internal class GetEPGChannelNameByDisplayNameHandler(ILogger<GetEPGChannelNameByDisplayName> logger, ISender Sender)
    : IRequestHandler<GetEPGChannelNameByDisplayName, string?>
{
    public async Task<string?> Handle(GetEPGChannelNameByDisplayName request, CancellationToken cancellationToken = default)
    {
        IEnumerable<ProgrammeNameDto> programmeNames = await Sender.Send(new GetProgrammeNamesDto(), cancellationToken).ConfigureAwait(false);
        ProgrammeNameDto? pn = programmeNames.FirstOrDefault(a => a.DisplayName == request.displayName);
        if (pn == null)
        {
            pn = programmeNames.FirstOrDefault(a => a.ChannelName == request.displayName);
            if (pn == null)
            {
                return null;
            }
        }
        return pn.Channel;
    }
}
