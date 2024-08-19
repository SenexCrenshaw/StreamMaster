namespace StreamMaster.Application.Programmes.Queries;

public record GetProgrammeNames : IRequest<List<string>>;

internal class GetProgrammeNamesHandler(ISender Sender)
    : IRequestHandler<GetProgrammeNames, List<string>>
{
    public async Task<List<string>> Handle(GetProgrammeNames request, CancellationToken cancellationToken)
    {
        IEnumerable<ProgrammeNameDto> programmes = await Sender.Send(new GetProgrammeNamesDto(), cancellationToken).ConfigureAwait(false);

        List<string> ret = programmes
            .Where(a => !string.IsNullOrEmpty(a.Channel))
            .OrderBy(a => a.DisplayName, StringComparer.OrdinalIgnoreCase)
            .Select(a => a.DisplayName).Distinct().ToList();

        return ret;
    }
}
