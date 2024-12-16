namespace StreamMaster.FUSE;

public sealed class DirectoryNode(string name) : IFileSystemNode
{
    public string Name { get; } = name;
    public bool IsDirectory => true;
    public System.Collections.Concurrent.ConcurrentDictionary<string, IFileSystemNode> Children { get; } = new System.Collections.Concurrent.ConcurrentDictionary<string, IFileSystemNode>(StringComparer.Ordinal);
}