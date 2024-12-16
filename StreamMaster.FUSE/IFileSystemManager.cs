namespace StreamMaster.FUSE;

public interface IFileSystemManager
{
    bool CreateFile(string path);

    bool GetAttr(string path, out bool isDir, out long size);

    bool Mkdir(string path);

    System.Collections.Generic.IEnumerable<IFileSystemNode> ReadDir(string path);

    int ReadFile(string path, long offset, int size, byte[] buffer);

    bool Rmdir(string path);

    bool Unlink(string path);

    bool WriteFile(string path, long offset, byte[] data);
}