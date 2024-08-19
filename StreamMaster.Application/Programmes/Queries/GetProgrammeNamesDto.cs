namespace StreamMaster.Application.Programmes.Queries;

public record GetProgrammeNamesDto : IRequest<IEnumerable<ProgrammeNameDto>>;

internal class GetProgrammeNamesDtoHandler( ISender Sender)
    : IRequestHandler<GetProgrammeNamesDto, IEnumerable<ProgrammeNameDto>>
{
    public async Task<IEnumerable<ProgrammeNameDto>> Handle(GetProgrammeNamesDto request, CancellationToken cancellationToken)
    {
        List<XmltvProgramme> programmes = await Sender.Send(new GetProgrammesRequest(), cancellationToken).ConfigureAwait(false);

        if (programmes.Any())
        {
            //IEnumerable<ProgrammeNameDto> progs = programmes.GroupBy(a => a.Channel).Select(group => group.First()).Select(a => new ProgrammeNameDto
            //{
            //    Channel = a.Channel,
            //    VideoStreamName = a.VideoStreamName,
            //    DisplayName = a.DisplayName
            //});

            //List<ProgrammeNameDto> ret = progs.OrderBy(a => a.DisplayName).ToList();

            //ret.Insert(0, new ProgrammeNameDto
            //{
            //    Channel = "Dummy",
            //    VideoStreamName = "Dummy",
            //    DisplayName = "Dummy"
            //});

            return [];
        }

        return new List<ProgrammeNameDto>();
    }
}
