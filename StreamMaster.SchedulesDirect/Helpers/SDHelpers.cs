
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using StreamMaster.Domain.Common;

using System.Security.Cryptography;
using System.Text;

namespace StreamMaster.SchedulesDirect.Helpers;

public static partial class SDHelpers
{
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

            // Set target aspect/image size
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

            //var cropImg = new Image<Rgba32>(new Configuration(), cropRectangle.Width, cropRectangle.Height + (offsetY * 2));
            Image<Rgba32> cropImg = origImg.Clone(ctx =>
            {
                ctx.Resize(new ResizeOptions
                {
                    Size = new Size(tgtWidth, tgtHeight),//cropRectangle.Width, cropRectangle.Height),
                    Mode = ResizeMode.Crop
                });
                //var destinationRectangle = new Rectangle(0, offsetY, cropRectangle.Width, cropRectangle.Height);
                //_ = ctx.DrawImage(origImg, 1).Crop(destinationRectangle);
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

    public static string GetSDImageFullPath(this string fileName)
    {
        string subdirectoryName = fileName[0].ToString().ToLower();
        string logoPath = Path.Combine(BuildInfo.SDImagesFolder, subdirectoryName, fileName);

        return logoPath;
    }

    public static List<ProgramArtwork> GetArtWork(this MxfProgram program)
    {
        List<ProgramArtwork> artwork = [];
        // a movie or sport event will have a guide image from the program
        if (program.extras.TryGetValue("artwork", out dynamic? value))
        {
            artwork = value;
        }

        // get the season class from the program if it has a season
        if (artwork.Count == 0 && (program.mxfSeason?.extras.ContainsKey("artwork") ?? false))
        {
            artwork = program.mxfSeason.extras["artwork"];
        }

        // get the series info class from the program if it is a series
        if (artwork.Count == 0 && (program.mxfSeriesInfo?.extras.ContainsKey("artwork") ?? false))
        {
            artwork = program.mxfSeriesInfo.extras["artwork"];
        }
        return artwork;
    }

    public static List<ProgramArtwork> GetTieredImages(List<ProgramArtwork> sdImages, List<string> tiers, string artWorkSize)
    {

        List<ProgramArtwork> ret = [];
        IEnumerable<ProgramArtwork> images = sdImages.Where(arg =>
            !string.IsNullOrEmpty(arg.Category) && !string.IsNullOrEmpty(arg.Aspect) && !string.IsNullOrEmpty(arg.Uri) &&
            (string.IsNullOrEmpty(arg.Tier) || tiers.Contains(arg.Tier.ToLower())) &&
            !string.IsNullOrEmpty(arg.Size) && arg.Size.Equals(artWorkSize));

        // get the aspect ratios available and fix the URI
        HashSet<string> aspects = [];
        foreach (ProgramArtwork? image in images)
        {
            _ = aspects.Add(image.Aspect);
            //if (!image.Uri.ToLower().StartsWith("http"))
            //{
            //    image.Uri = $"{api.BaseArtworkAddress}image/{image.Uri.ToLower()}";
            //}
        }

        // determine which image to return with each aspect
        foreach (string aspect in aspects)
        {
            IEnumerable<ProgramArtwork> imgAspects = images.Where(arg => arg.Aspect.Equals(aspect));

            ProgramArtwork[] links = new ProgramArtwork[11];
            foreach (ProgramArtwork? image in imgAspects)
            {
                switch (image.Category.ToLower())
                {
                    case "box art":     // DVD box art, for movies only
                        if (links[0] == null)
                        {
                            links[0] = image;
                        }

                        break;
                    case "vod art":
                        if (links[1] == null)
                        {
                            links[1] = image;
                        }

                        break;
                    case "poster art":  // theatrical movie poster, standard sizes
                        if (links[2] == null)
                        {
                            links[2] = image;
                        }

                        break;
                    case "banner":      // source-provided image, usually shows cast ensemble with source-provided text
                        if (links[3] == null)
                        {
                            links[3] = image;
                        }

                        break;
                    case "banner-l1":   // same as Banner
                        if (links[4] == null)
                        {
                            links[4] = image;
                        }

                        break;
                    case "banner-l2":   // source-provided image with plain text
                        if (links[5] == null)
                        {
                            links[5] = image;
                        }

                        break;
                    case "banner-lo":   // banner with Logo Only
                        if (links[6] == null)
                        {
                            links[6] = image;
                        }

                        break;
                    case "logo":        // official logo for program, sports organization, sports conference, or TV station
                        if (links[7] == null)
                        {
                            links[7] = image;
                        }

                        break;
                    case "banner-l3":   // stock photo image with plain text
                        if (links[8] == null)
                        {
                            links[8] = image;
                        }

                        break;
                    case "iconic":      // representative series/season/episode image, no text
                        if (tiers.Contains("series") && links[9] == null)
                        {
                            links[9] = image;
                        }

                        break;
                    case "staple":      // the staple image is intended to cover programs which do not have a unique banner image
                        if (links[10] == null)
                        {
                            links[10] = image;
                        }

                        break;
                    case "banner-l1t":
                    case "banner-lot":  // banner with Logo Only + Text indicating season number
                        break;
                }
            }

            foreach (ProgramArtwork link in links)
            {
                if (link == null)
                {
                    continue;
                }

                ret.Add(link);
                break;
            }
        }

        if (ret.Count > 1)
        {
            ret = ret.OrderBy(arg => arg.Width).ToList();
        }
        return ret;
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

            if (!exactMatch && str.ToLower().Contains(text.ToLower()))
            {
                return true;
            }

            if (str.ToLower().Equals(text.ToLower()))
            {
                return true;
            }
        }

        return false;
    }

    public static bool StringContains(this string str, string text)
    {
        return str != null && str.ToLower().Contains(text.ToLower());
    }


    public static string GenerateHashFromStringContent(StringContent content)
    {
        byte[] contentBytes = Encoding.UTF8.GetBytes(content.ReadAsStringAsync().Result); // Extract string from StringContent and convert to bytes
        byte[] hashBytes = SHA256.HashData(contentBytes); // Compute SHA-256 hash
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower(); // Convert byte array to hex string
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
