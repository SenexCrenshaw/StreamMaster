

namespace StreamMasterApplication.Icons.Commands;

public record ReadDirectoryLogosRequest : IRequest { }

public class ReadDirectoryLogosRequestHandler(IMemoryCache memoryCache) : IRequestHandler<ReadDirectoryLogosRequest>
{
    public async Task Handle(ReadDirectoryLogosRequest command, CancellationToken cancellationToken)
    {
        _ = await IconHelper.ReadDirectoryLogos(memoryCache, cancellationToken);
    }
}
