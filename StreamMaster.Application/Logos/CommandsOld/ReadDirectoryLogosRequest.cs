namespace StreamMaster.Application.Logos.CommandsOld;

public record ReadDirectoryLogosRequest : IRequest { }

public class ReadDirectoryLogosRequestHandler(ILogoService IconService) : IRequestHandler<ReadDirectoryLogosRequest>
{
    public async Task Handle(ReadDirectoryLogosRequest command, CancellationToken cancellationToken)
    {
        _ = await IconService.ReadDirectoryTVLogos(cancellationToken);
    }
}
