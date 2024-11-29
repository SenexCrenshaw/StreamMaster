using System.Drawing;
using System.Drawing.Imaging;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using Svg;

namespace StreamMaster.Application.Common
{
    public static class ImageConverter
    {

        public static string ConvertDataToPNG(string Name, string Source)
        {

            if (ImageConverter.IsData(Source))
            {
                LogoInfo nl = new(Name, Source, iconType: SMFileTypes.CustomLogo);
                if (!File.Exists(nl.FullPath))
                {
                    ImageConverter.ConvertBase64ToTransparentPng(Source, nl.FullPath);
                }
                return $"/api/files/cu/{nl.FileName}";
            }
            else if (ImageConverter.IsSVGData(Source))
            {
                LogoInfo nl = new(Name, Source, iconType: SMFileTypes.CustomLogo);
                if (!File.Exists(nl.FullPath))
                {
                    ImageConverter.ConvertSvgBase64ToPng(Source, nl.FullPath);
                }
                return $"/api/files/cu/{nl.FileName}";

            }

            return Source;
        }

        public static bool IsData(string Source)
        {
            return Source.StartsWithIgnoreCase("data:image");
        }


        public static bool IsSVGData(string Source)
        {
            return Source.StartsWithIgnoreCase("data:image/svg");
        }


        public static bool IsCustomSource(string Source)
        {
            return Source.StartsWithIgnoreCase("/api/files/cu/");
        }

        /// <summary>
        /// Converts a Base64-encoded image string into a PNG with a transparent background.
        /// </summary>
        /// <param name="base64Image">The Base64-encoded image string.</param>
        /// <param name="outputPath">The file path to save the PNG image.</param>
        public static void ConvertBase64ToTransparentPng(string base64Image, string outputPath)
        {
            if (string.IsNullOrWhiteSpace(base64Image))
            {
                throw new ArgumentException("Base64 image string cannot be null or empty.", nameof(base64Image));
            }

            // Extract MIME type and Base64 data
            string[] parts = base64Image.Split(',');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid Base64 image string format.", nameof(base64Image));
            }

            string mimeType = parts[0].Split(':')[1].Split(';')[0];
            string base64Data = parts[1];

            // Validate supported MIME types
            string[] supportedFormats = new[] { "image/jpeg", "image/png", "image/gif", "image/webp", "image/bmp", "image/tiff" };
            if (!Array.Exists(supportedFormats, format => format.Equals(mimeType, StringComparison.OrdinalIgnoreCase)))
            {
                throw new NotSupportedException($"The format '{mimeType}' is not supported for conversion.");
            }

            // Decode Base64 and process the image
            byte[] imageBytes = Convert.FromBase64String(base64Data);
            using Image<Rgba32> image = SixLabors.ImageSharp.Image.Load<Rgba32>(imageBytes);
            image.Mutate(ctx => ctx.BackgroundColor(SixLabors.ImageSharp.Color.Transparent));
            image.SaveAsPng(outputPath);
        }

        /// <summary>
        /// Converts a Base64-encoded SVG image to a PNG file.
        /// </summary>
        /// <param name="base64Svg">The Base64-encoded SVG string.</param>
        /// <param name="outputPath">The path to save the PNG file.</param>
        public static void ConvertSvgBase64ToPng(string base64Svg, string outputPath)
        {
            if (string.IsNullOrWhiteSpace(base64Svg))
            {
                throw new ArgumentException("Base64 SVG string cannot be null or empty.", nameof(base64Svg));
            }

            // Remove the data URI prefix if present
            string base64Data = base64Svg.Contains(",")
                ? base64Svg[(base64Svg.IndexOf(",") + 1)..]
                : base64Svg;

            // Decode Base64 to byte array
            byte[] svgBytes = Convert.FromBase64String(base64Data);

            // Load the SVG from the byte array
            using MemoryStream stream = new(svgBytes);
            SvgDocument svgDocument = SvgDocument.Open<SvgDocument>(stream);

            // Determine width and height
            int width = (int)svgDocument.Width.Value;
            int height = (int)svgDocument.Height.Value;

            if (svgDocument.ViewBox.Width != 0)
            {
                width = (int)svgDocument.ViewBox.Width;
            }

            if (svgDocument.ViewBox.Height != 0)
            {
                height = (int)svgDocument.ViewBox.Height;
            }

            // Render the SVG to a bitmap
            using Bitmap bitmap = new(width, height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(System.Drawing.Color.Transparent);
                svgDocument.Draw(graphics);
            }

            // Save the bitmap as PNG
            bitmap.Save(outputPath, ImageFormat.Png);
        }
    }
}
