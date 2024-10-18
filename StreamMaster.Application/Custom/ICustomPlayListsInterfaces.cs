namespace StreamMaster.Application.Custom;

public interface ICustomPlayListsTasks
{
    ValueTask ScanForCustomPlayLists(CancellationToken cancellationToken = default);
}