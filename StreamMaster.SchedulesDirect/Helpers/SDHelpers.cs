
using System.Security.Cryptography;
using System.Text;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace StreamMaster.SchedulesDirect.Helpers;

public static partial class SDHelpers
{
    // Priority categories in descending order
    private static readonly List<string> categories =
        [
        "box art",
            "key art",
            "vod art",
            "poster art",
            "banner",
            "banner-l1",
            "banner-l2",
            "banner-lo",
            "logo",
            "banner-l3",
            "iconic",
            "staple"
        ];
    private static readonly Dictionary<string, int> categoryPriority = categories
        .Select((category, index) => new { category, index })
        .ToDictionary(item => item.category, item => item.index, StringComparer.OrdinalIgnoreCase);
    public static Image? CropAndResizeImage(Image<Rgba32> origImg)
    {
        try
        {
            if (origImg == null)
            {
                return null;
            }

            // Set target image size
            const int tgtWidth = 360;
            const int tgtHeight = 270;

            // Set target asp/image size
            const double tgtAspect = 3.0;

            // Find the min/max non-transparent pixels
            Point min = new(int.MaxValue, int.MaxValue);
            Point max = new(int.MinValue, int.MinValue);

            for (int x = 0; x < origImg.Width; ++x)
            {
                for (int y = 0; y < origImg.Height; ++y)
                {
                    Rgba32 pixel = origImg[x, y];
                    if (pixel.A <= 0)
                    {
                        continue;
                    }

                    if (x < min.X)
                    {
                        min.X = x;
                    }

                    if (y < min.Y)
                    {
                        min.Y = y;
                    }

                    if (x > max.X)
                    {
                        max.X = x;
                    }

                    if (y > max.Y)
                    {
                        max.Y = y;
                    }
                }
            }

            // Create a new image from the crop rectangle and increase canvas size if necessary
            int offsetY = 0;
            Rectangle cropRectangle = new(min.X, min.Y, max.X - min.X + 1, max.Y - min.Y + 1);
            if ((max.X - min.X + 1) / tgtAspect > (max.Y - min.Y + 1))
            {
                offsetY = (int)(((max.X - min.X + 1) / tgtAspect) - (max.Y - min.Y + 1) + 0.5) / 2;
            }
            Image<Rgba32> cropImg = origImg.Clone(ctx =>
                 {
                     _ = ctx.Resize(new ResizeOptions
                     {
                         Size = new Size(tgtWidth, tgtHeight),//cropRectangle.Width, cropRectangle.Height),
                         Mode = ResizeMode.Crop
                     });
                 });

            return cropImg;
            //if (tgtHeight >= cropImg.Height && tgtWidth >= cropImg.Width)
            //{
            //    return cropImg;
            //}

            //// Resize image if needed
            //var scale = Math.Min((double)tgtWidth / cropImg.Width, (double)tgtHeight / cropImg.Height);
            //var destWidth = (int)(cropImg.Width * scale);
            //var destHeight = (int)(cropImg.Height * scale);

            //return cropImg.Clone(ctx => ctx.Resize(new ResizeOptions
            //{
            //    Size = new Size(destWidth, destHeight),
            //    Mode = ResizeMode.Max
            //}));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public static List<ProgramArtwork> GetArtWork(this MxfProgram program)
    {
        List<ProgramArtwork> artwork = [];
        // a movie or sport event will have a guide image from the program
        if (program.Extras.TryGetValue("artwork", out dynamic? value))
        {
            artwork = value;
        }

        // get the season class from the program if it has a season
        if (artwork.Count == 0 && (program.mxfSeason?.Extras.ContainsKey("artwork") ?? false))
        {
            artwork = program.mxfSeason.Extras["artwork"];
        }

        // get the series info class from the program if it is a series
        if (artwork.Count == 0 && (program.mxfSeriesInfo?.Extras.ContainsKey("artwork") ?? false))
        {
            artwork = program.mxfSeriesInfo.Extras["artwork"];
        }
        return artwork;
    }

    /// <summary>
    /// Filters and selects tiered images from a list of program artwork based on specified criteria.
    /// </summary>
    /// <param name="sdImages">The list of program artwork to filter.</param>
    /// <param name="artWorkSize">The desired size of the artwork.</param>
    /// <param name="tiers">The preferred tiers of artwork.</param>
    /// <param name="aspect">The desired aspect ratio.</param>
    /// <returns>A filtered and prioritized list of program artwork.</returns>
    public static List<ProgramArtwork> GetTieredImages(
        List<ProgramArtwork> sdImages,
        string artWorkSize,
         List<string>? tiers = null,
        string? aspect = null)
    {
        if (sdImages == null || tiers == null)
        {
            throw new ArgumentNullException(nameof(sdImages), "Input list cannot be null.");
        }

        /// A filtered list of <see cref="ProgramArtwork"/> objects that meet the following criteria:
        /// <list type="bullet">
        ///     <item><see cref="ProgramArtwork.Category"/> is not null or empty.</item>
        ///     <item><see cref="ProgramArtwork.Aspect"/> is not null or empty.</item>
        ///     <item><see cref="ProgramArtwork.Uri"/> is not null or empty.</item>
        ///     <item>If <see cref="ProgramArtwork.Tier"/> is not null or empty, it must match one of the values in <paramref name="tiers"/> (case-insensitive).</item>
        ///     <item>The <see cref="ProgramArtwork.Aspect"/> must match the specified <paramref name="aspect"/> (case-insensitive).</item>
        ///     <item>The <see cref="ProgramArtwork.Size"/> must match the specified <paramref name="artWorkSize"/> (case-insensitive).</item>
        /// </list>
        List<ProgramArtwork> filteredImages = sdImages
            .Where(image =>
                !string.IsNullOrEmpty(image.Category) &&
                !string.IsNullOrEmpty(image.Aspect) &&
                !string.IsNullOrEmpty(image.Uri) &&
                (string.IsNullOrEmpty(image.Tier) || tiers?.Contains(image.Tier.ToLower()) != false) &&
                (string.IsNullOrEmpty(aspect) || image.Aspect.EqualsIgnoreCase(aspect)) &&
                string.Equals(image.Size, artWorkSize, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Group images by aspect ratio
        Dictionary<string, List<ProgramArtwork>> aspects = filteredImages
            .GroupBy(image => image.Aspect)
            .ToDictionary(group => group.Key, group => group.ToList());

        List<ProgramArtwork> result = [];

        // Process each aspect group to select the highest priority image
        foreach (KeyValuePair<string, List<ProgramArtwork>> aspectGroup in aspects)
        {
            List<ProgramArtwork> aspectImages = aspectGroup.Value;

            List<ProgramArtwork> tests =
                [.. aspectImages.OrderBy(image => categoryPriority.TryGetValue(image.Category, out int value)
               ? value : int.MaxValue)];

            //List<ProgramArtwork> tests2 = tests.OrderBy(image => image.PixelCount).ToList();

            //ProgramArtwork? prioritizedImage = aspectImages
            //   .OrderBy(image => categoryPriority.ContainsKey(image.Category)
            //       ? categoryPriority[image.Category]
            //       : int.MaxValue)
            //   .FirstOrDefault();
            IEnumerable<ProgramArtwork> prioritizedImages =
                aspectImages.OrderBy(image => categoryPriority.TryGetValue(image.Category, out int value) ? value : int.MaxValue).Take(3);

            //if (prioritizedImages != null)
            //{
            result.AddRange(prioritizedImages);
            //}
        }

        return result;
    }

    public static bool TableContains(string[] table, string text, bool exactMatch = false)
    {
        if (table == null)
        {
            return false;
        }

        foreach (string str in table)
        {
            if (string.IsNullOrEmpty(str))
            {
                continue;
            }

            if (!exactMatch && str.ContainsIgnoreCase(text))
            {
                return true;
            }

            if (str.EqualsIgnoreCase(text))
            {
                return true;
            }
        }

        return false;
    }

    public static bool StringContains(this string str, string text)
    {
        return str?.ToLower().ContainsIgnoreCase(text) == true;
    }

    public static string GenerateHashFromStringContent(StringContent content)
    {
        byte[] contentBytes = Encoding.UTF8.GetBytes(content.ReadAsStringAsync().Result); // Extract string from StringContent and convert to bytes
        byte[] hashBytes = SHA256.HashData(contentBytes); // Compute SHA-256 hash
        return Convert.ToHexStringLower(hashBytes); // Convert byte array to hex string
    }

    public static string GenerateCacheKey(string command)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        StringBuilder sanitized = new(command.Length);
        foreach (char c in command)
        {
            if (!invalidChars.Contains(c))
            {
                _ = sanitized.Append(c);
            }
            else
            {
                _ = sanitized.Append('_');  // replace invalid chars with underscore or another desired character
            }
        }
        _ = sanitized.Append(".json");
        return sanitized.ToString();
    }

    public static UserStatus GetStatusOffline()
    {
        UserStatus ret = new();
        ret.SystemStatus.Add(new SystemStatus { Status = "Offline" });
        return ret;
    }
}
