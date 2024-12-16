namespace StreamMaster.FUSE;

public interface IFileSystemNode
{
    string Name { get; }
    bool IsDirectory { get; }
}