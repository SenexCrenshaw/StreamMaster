namespace StreamMaster.WebDav.Services;

/// <summary>
/// Ensures consistent path normalization across platforms.
/// </summary>
public class PathNormalizer : IPathNormalizer
{
    public string Normalize(string path)
    {
        return path.Replace("\\", "/").TrimEnd('/');
    }
}