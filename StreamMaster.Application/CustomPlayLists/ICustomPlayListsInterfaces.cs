namespace StreamMaster.Application.CustomPlayLists;

public interface ICustomPlayListsTasks
{
    ValueTask ScanForCustomPlayLists(CancellationToken cancellationToken = default);
}