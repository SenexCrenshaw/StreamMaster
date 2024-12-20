using System.IO.Compression;
using System.Text;

namespace StreamMaster.Domain.Compression;

public static class CompressionUtils
{
    /// <summary>
    /// Compresses a string into a GZip-compressed byte array.
    /// </summary>
    /// <param name="json">The string to compress.</param>
    /// <returns>A GZip-compressed byte array.</returns>
    public static byte[] Compress(this string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return [];
        }

        using MemoryStream memoryStream = new();
        using (GZipStream gzipStream = new(memoryStream, CompressionLevel.Optimal, leaveOpen: true))
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            gzipStream.Write(jsonBytes, 0, jsonBytes.Length);
        }

        return memoryStream.ToArray();
    }

    /// <summary>
    /// Decompresses a GZip-compressed byte array back into a string.
    /// </summary>
    /// <param name="compressedData">The compressed byte array.</param>
    /// <returns>The decompressed string.</returns>
    public static string Decompress(this byte[] compressedData)
    {
        if (compressedData == null || compressedData.Length == 0)
        {
            return string.Empty;
        }

        using MemoryStream memoryStream = new(compressedData);
        using GZipStream gzipStream = new(memoryStream, CompressionMode.Decompress);
        using StreamReader reader = new(gzipStream, Encoding.UTF8);
        return reader.ReadToEnd();
    }
}
