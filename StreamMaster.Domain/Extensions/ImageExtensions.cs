using StreamMaster.Domain.Configuration;

namespace StreamMaster.Domain.Extensions;

public static class ImageExtensions
{
    public static async Task<byte[]> GetStreamBytes(this FileStream fileStream, CancellationToken cancellationToken)
    {
        using (fileStream)
        {
            await using MemoryStream memoryStream = new();
            await fileStream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
            return memoryStream.ToArray();
        }
    }
    public static string? GetLogoImageFullPath(this string Source)
    {
        return Source.Contains('\\') ? GetProgramTvLogoImageFullPath(Source) : GetImageFullPath(Source, SMFileTypes.Logo);
    }

    public static string? GetProgramLogoFullPath(this string fileName)
    {
        return GetImageFullPath(fileName, SMFileTypes.ProgramLogo);
    }

    public static string? GetCustomLogoImageFullPath(this string fileName)
    {
        return GetImageFullPath(fileName, SMFileTypes.CustomLogo);
    }

    public static string? GetProgramTvLogoImageFullPath(this string fileName)
    {
        return string.IsNullOrWhiteSpace(fileName) ? null : Path.Combine(BuildInfo.TVLogoFolder, fileName);
    }

    public static string GetPNGPath(this string fileName)
    {
        return Path.GetExtension(fileName).EqualsIgnoreCase(".svg")
            ? Path.ChangeExtension(fileName, ".png")
            : fileName;
    }

    public static string? GetImageFullPath(this string fileName, SMFileTypes smFileType)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return null;
        }

        // Determine the FileDefinition based on the file type
        FileDefinition? fd = FileDefinitions.GetFileDefinition(smFileType);
        if (fd is null)
        {
            return null;
        }

        // If the file has an .svg extension, replace it with .png
        string updatedFileName = fileName.GetPNGPath();

        string subDir = char.ToLower(updatedFileName[0]).ToString();
        return Path.Combine(fd.DirectoryLocation, subDir, updatedFileName);
    }
}