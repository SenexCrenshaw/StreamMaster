namespace StreamMaster.Application.Icons.CommandsOld;

public record ReadDirectoryLogosRequest : IRequest { }

public class ReadDirectoryLogosRequestHandler(IIconService IconService) : IRequestHandler<ReadDirectoryLogosRequest>
{
    public async Task Handle(ReadDirectoryLogosRequest command, CancellationToken cancellationToken)
    {
        _ = await IconService.ReadDirectoryTVLogos(cancellationToken);
    }
}
