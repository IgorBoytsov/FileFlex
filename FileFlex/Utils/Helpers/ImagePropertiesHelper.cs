using System.IO;
using System.Windows.Media.Imaging;
using Image = System.Drawing.Image;

namespace FileFlex.Utils.Helpers
{
    public static class ImagePropertiesHelper
    {
        public static Task<(int width, int height)> WidthAndHeightAsync(string path, string extension)
        {
            return Task.Run(() =>
            {
                try
                {
                    string format = extension.ToLower();

                    if (format == ".webp" || format == ".heic")
                    {
                        using (FileStream fileStream = new (path, FileMode.Open, FileAccess.Read))
                        {
                            BitmapDecoder decoder = BitmapDecoder.Create(
                                fileStream,
                                BitmapCreateOptions.PreservePixelFormat,
                                BitmapCacheOption.OnLoad);

                            BitmapFrame frame = decoder.Frames[0];

                            int width = frame.PixelWidth;
                            int height = frame.PixelHeight;

                            return (width, height);
                        }
                    }
                    else
                    {
                        using (var image = Image.FromFile(path))
                        {
                            int width = image.Width;
                            int height = image.Height;
                            return (width, height);
                        }
                    }
                }
                catch (Exception)
                {
                    return (0, 0);
                }
            });
        }

        public static string PixelFormat(string path, string extension)
        {
            string format = extension.ToLower();

            if (format == ".jpg" || format == ".jpeg" || format == ".jfif" || format == ".jpe" ||
                format == ".png" ||
                format == ".ico")
            {
                using (Image image = Image.FromFile(path))
                {
                    System.Drawing.Imaging.PixelFormat pixelFormat = image.PixelFormat;
                    return image.PixelFormat.ToString();
                }
            }
            if (format == ".webp" ||
                format == ".heic" ||
                format == ".gif")
            {
                using (FileStream stream = new(path, FileMode.Open, FileAccess.Read))
                {
                    BitmapDecoder decoder = BitmapDecoder.Create(
                        stream,
                        BitmapCreateOptions.IgnoreColorProfile,
                        BitmapCacheOption.Default
                    );

                    BitmapFrame frame = decoder.Frames[0];

                    return frame.Format.ToString();
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static (double dpiX, double dpiY) Dpi(string path)
        {
            try
            {
                using (FileStream fileStream = new(path, FileMode.Open, FileAccess.Read))
                {
                    BitmapDecoder decoder = BitmapDecoder.Create(
                        fileStream,
                        BitmapCreateOptions.PreservePixelFormat,
                        BitmapCacheOption.OnLoad);

                    BitmapFrame frame = decoder.Frames[0];

                    double DpiX = frame.DpiX;
                    double DpiY = frame.DpiY;

                    return (DpiX, DpiY);
                }
            }
            catch (Exception)
            {
                return (0, 0);
            }
        }

        public static string Authors(string imagePath)
        {
            try
            {
                var bitmapDecoder = BitmapDecoder.Create(
                new Uri(imagePath),
                BitmapCreateOptions.IgnoreColorProfile,
                BitmapCacheOption.Default);

                string authors = string.Empty;
                var frame = bitmapDecoder.Frames[0];
                var metadata = frame.Metadata as BitmapMetadata;

                if (metadata != null && metadata.Author != null && metadata.Author.Count > 0)
                {
                    authors = string.Join(", ", metadata.Author);
                }
                else
                {
                    authors = "Информация по автору не найдена.";
                }

                return authors;

            }
            catch (Exception)
            {
                return string.Empty;
            }  
        }

        public static int FramesCount(string imagePath)
        {
            var bitmapDecoder = BitmapDecoder.Create(
                new Uri(imagePath),
                BitmapCreateOptions.IgnoreColorProfile,
                BitmapCacheOption.Default);
            
            return bitmapDecoder.Frames.Count;
        }
    }
}