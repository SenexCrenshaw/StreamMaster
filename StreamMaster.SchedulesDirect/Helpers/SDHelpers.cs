
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

    /// <summary>
    /// Filters and selects tiered images from a list of program artwork based on specified criteria.
    /// </summary>
    /// <param name="sdImages">The list of program artwork to filter.</param>
    /// <param name="artWorkSize">The desired size of the artwork. Options include Sm, Md, Lg.</param>
    /// <param name="tiers">The preferred tiers of artwork.</param>
    /// <param name="aspect">The desired aspect ratio.</param>
    /// <returns>A filtered and prioritized list of program artwork.</returns>
    public static List<ProgramArtwork> GetTieredImages(List<ProgramArtwork> sdImages, string artWorkSize, List<string>? tiers = null, string? aspect = null)
    {
        if (sdImages == null)
        {
            throw new ArgumentNullException(nameof(sdImages), "Input list cannot be null.");
        }

        // Order of priority for artwork sizes
        List<string> sizePriority = ["Lg", "Md", "Sm"];
        int requestedSizeIndex = sizePriority.IndexOf(artWorkSize);
        if (requestedSizeIndex == -1)
        {
            throw new ArgumentException("Invalid artWorkSize. Expected one of: Sm, Md, Lg.", nameof(artWorkSize));
        }

        // Adjust size priority based on requested artwork size
        List<string> applicableSizes = [.. sizePriority.Skip(requestedSizeIndex)];

        // Filter images based on provided criteria
        List<ProgramArtwork> filteredImages = [.. sdImages
            .Where(image =>
                !string.IsNullOrEmpty(image.Category) &&
                !string.IsNullOrEmpty(image.Aspect) &&
                !string.IsNullOrEmpty(image.Uri) &&
                (string.IsNullOrEmpty(image.Tier) || tiers?.Contains(image.Tier.ToLower()) != false) &&
                (string.IsNullOrEmpty(aspect) || image.Aspect.Equals(aspect, StringComparison.OrdinalIgnoreCase)))];

        // Process each artwork size in priority order to gather images
        List<ProgramArtwork> prioritizedImages = [];
        foreach (string size in applicableSizes)
        {
            List<ProgramArtwork> imagesOfSize = [.. filteredImages.Where(image => string.Equals(image.Size, size, StringComparison.OrdinalIgnoreCase))];

            if (imagesOfSize.Count != 0)
            {
                // Group images by aspect ratio
                Dictionary<string, List<ProgramArtwork>> aspects = imagesOfSize
                    .GroupBy(image => image.Aspect)
                    .ToDictionary(group => group.Key, group => group.ToList());

                // Process each aspect group to select the highest priority image based on category
                foreach (KeyValuePair<string, List<ProgramArtwork>> aspectGroup in aspects)
                {
                    List<ProgramArtwork> aspectImages = aspectGroup.Value;

                    IEnumerable<ProgramArtwork> prioritizedCategoryImages = aspectImages
                        .OrderBy(image => categoryPriority.TryGetValue(image.Category, out int value) ? value : int.MaxValue)
                        .Take(3);

                    prioritizedImages.AddRange(prioritizedCategoryImages);
                }
            }

            // If we have found some images, break the loop since we have a satisfactory size
            if (prioritizedImages.Count != 0)
            {
                break;
            }
        }

        return prioritizedImages;
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
