public class FileSaver
{
    private readonly int _maxRevisions;

    public FileSaver(int maxRevisions)
    {
        _maxRevisions = maxRevisions;
    }

    public async Task SaveVideoWithRevisionsAsync(byte[] videoMemory, string baseFilePath)
    {
        string? directory = Path.GetDirectoryName(baseFilePath);
        string baseFileName = Path.GetFileNameWithoutExtension(baseFilePath);
        string extension = Path.GetExtension(baseFilePath);

        // Ensure the directory exists
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }

        // Find the next available file name with versioning
        string newFilePath = baseFilePath;
        for (int i = 1; i <= _maxRevisions; i++)
        {
            string versionedFilePath = Path.Combine(directory!, $"{baseFileName}.{i}{extension}");
            if (!File.Exists(versionedFilePath))
            {
                newFilePath = versionedFilePath;
                break;
            }
        }

        // Save the new file
        using (FileStream fs = new(newFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
        {
            await fs.WriteAsync(videoMemory, 0, videoMemory.Length).ConfigureAwait(false);
        }

        // Manage old revisions: Delete the oldest file if the number of revisions exceeds the limit
        ManageOldRevisions(directory!, baseFileName, extension);
    }

    private void ManageOldRevisions(string directory, string baseFileName, string extension)
    {
        // Get all versioned files matching the pattern
        List<string> files = Directory.GetFiles(directory, $"{baseFileName}.*{extension}")
                             .Where(f => Path.GetFileNameWithoutExtension(f).StartsWith($"{baseFileName}.") &&
                                         int.TryParse(Path.GetFileNameWithoutExtension(f)[(baseFileName.Length + 1)..], out _))
                             .OrderBy(f => f)
                             .ToList();

        // Delete files if the number of revisions exceeds the limit
        while (files.Count > _maxRevisions)
        {
            File.Delete(files[0]);
            files.RemoveAt(0);
        }
    }
}

