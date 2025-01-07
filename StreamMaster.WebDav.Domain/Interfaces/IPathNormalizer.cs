namespace StreamMaster.WebDav.Domain.Interfaces;

/// <summary>
/// Interface for normalizing file paths across platforms.
/// </summary>
public interface IPathNormalizer
{
    /// <summary>
    /// Normalizes a file or directory path to ensure consistency across platforms.
    /// </summary>
    /// <param name="path">The input path to normalize.</param>
    /// <returns>A normalized path string.</returns>
    string Normalize(string path);
}