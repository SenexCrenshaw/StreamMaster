namespace StreamMaster.FUSE;

public sealed class FileNode(string name) : IFileSystemNode
{
    private readonly System.IO.MemoryStream content = new();

    public string Name { get; } = name;
    public bool IsDirectory => false;
    public long Size => content.Length;

    public int Read(long offset, byte[] buffer)
    {
        if (offset >= content.Length)
        {
            return 0;
        }

        content.Position = offset;
        return content.Read(buffer, 0, buffer.Length);
    }

    public void Write(long offset, byte[] data)
    {
        if (offset > content.Length)
        {
            content.SetLength(offset);
        }
        content.Position = offset;
        content.Write(data, 0, data.Length);
    }
}