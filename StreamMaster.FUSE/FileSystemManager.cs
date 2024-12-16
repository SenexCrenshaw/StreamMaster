namespace StreamMaster.FUSE;

public sealed class FileSystemManager : IFileSystemManager
{
    private readonly DirectoryNode root;

    public FileSystemManager()
    {
        root = new DirectoryNode("");
    }

    public bool CreateFile(string path)
    {
        (DirectoryNode? parent, string name) = SplitPath(path);
        if (parent == null || string.IsNullOrEmpty(name))
        {
            return false;
        }

        if (parent.Children.ContainsKey(name))
        {
            return false;
        }

        parent.Children[name] = new FileNode(name);
        return true;
    }

    public bool GetAttr(string path, out bool isDir, out long size)
    {
        IFileSystemNode? node = Traverse(path);
        if (node == null)
        {
            isDir = false; size = 0;
            return false;
        }

        if (node.IsDirectory)
        {
            isDir = true; size = 0;
        }
        else
        {
            FileNode file = (FileNode)node;
            isDir = false; size = file.Size;
        }
        return true;
    }

    public bool Mkdir(string path)
    {
        (DirectoryNode? parent, string name) = SplitPath(path);
        if (parent == null || string.IsNullOrEmpty(name))
        {
            return false;
        }

        if (parent.Children.ContainsKey(name))
        {
            return false;
        }

        parent.Children[name] = new DirectoryNode(name);
        return true;
    }

    public System.Collections.Generic.IEnumerable<IFileSystemNode> ReadDir(string path)
    {
        return Traverse(path) is not DirectoryNode dir ? System.Linq.Enumerable.Empty<IFileSystemNode>() : dir.Children.Values;
    }

    public int ReadFile(string path, long offset, int size, byte[] buffer)
    {
        return Traverse(path) is not FileNode file ? -1 : file.Read(offset, buffer);
    }

    public bool Rmdir(string path)
    {
        if (Traverse(path) is not DirectoryNode node)
        {
            return false;
        }

        if (!node.Children.IsEmpty)
        {
            return false;
        }

        (DirectoryNode? parent, string name) = SplitPath(path);
        return parent?.Children.TryRemove(name, out _) == true;
    }

    public bool Unlink(string path)
    {
        if (Traverse(path) is not FileNode)
        {
            return false;
        }

        (DirectoryNode? parent, string name) = SplitPath(path);
        return parent?.Children.TryRemove(name, out _) == true;
    }

    public bool WriteFile(string path, long offset, byte[] data)
    {
        if (Traverse(path) is not FileNode file)
        {
            return false;
        }

        file.Write(offset, data);
        return true;
    }

    private IFileSystemNode? Traverse(string path)
    {
        if (string.IsNullOrEmpty(path) || path == "/")
        {
            return root;
        }

        string[] parts = path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        IFileSystemNode current = root;
        foreach (string part in parts)
        {
            if (current is DirectoryNode dir)
            {
                if (!dir.Children.TryGetValue(part, out IFileSystemNode? child))
                {
                    return null;
                }

                current = child;
            }
            else
            {
                return null;
            }
        }
        return current;
    }

    private (DirectoryNode? parent, string name) SplitPath(string path)
    {
        string cleaned = path.Trim('/');
        if (string.IsNullOrEmpty(cleaned))
        {
            return (null, "");
        }

        int idx = cleaned.LastIndexOf('/');
        string parentPath = idx == -1 ? "/" : "/" + cleaned[..idx];
        string name = cleaned[(idx + 1)..];

        DirectoryNode? parentNode = Traverse(parentPath) as DirectoryNode;
        return (parentNode, name);
    }
}