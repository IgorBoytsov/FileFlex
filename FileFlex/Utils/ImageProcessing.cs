using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FileFlex.Utils
{
    internal static class ImageProcessing
    {
        #region Методы для конвертации

        public static void SaveFileToPng(Bitmap Bitmap, string PathFile)
        {
            Bitmap.Save(PathFile, ImageFormat.Png);
        }

        public static void SaveFileToJpeg(Bitmap Bitmap, string PathFile)
        {
            Bitmap.Save(PathFile, ImageFormat.Jpeg);
        }  

        #endregion
      

        #region Методы для обработки изображение в формате Bitmap / BitmapImage

        /// <summary>
        /// Преобразует ссылку на изображение в Bitmap
        /// </summary>
        /// <param name="PathFile"></param>
        /// <returns></returns>
        public static Bitmap ImageToBitmap(string  PathFile)
        {
            using (Bitmap bitmap = new(PathFile))
            {
                return bitmap;
            }
        }

       /// <summary>
       /// Удаляет половину пикселей из изображения.
       /// </summary>
       /// <param name="bitmap"></param>
       /// <returns></returns>
        public static async Task<Bitmap> RemoveHalfPixelsAsync(Bitmap bitmap)
        {

            Bitmap result = new Bitmap(bitmap.Width / 2, bitmap.Height / 2);
            await Task.Run(() =>
            {
                try
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            bool isEvenX = x % 2 == 0;
                            bool isEvenY = y % 2 == 0;

                            if (isEvenX && isEvenY)
                            {
                                continue;
                            }

                            Color color = bitmap.GetPixel(x, y);
                            result.SetPixel(x / 2, y / 2, color);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }          
            });
            bitmap.Dispose();
            return result;
        }

        /// <summary>
        /// Производит преобразование из Bitmap в BitmapImage. 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static async Task<BitmapImage> ConvertBitmapToBitmapImageAsync(Bitmap bitmap)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        bitmap.Save(memory, ImageFormat.Png);

                        memory.Seek(0, SeekOrigin.Begin); // Перемещаем указатель потока в начало

                        BitmapImage bitmapImage = new BitmapImage();

                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memory;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze(); // Обеспечиваем доступ к свойствам BitmapImage на основном UI потоке

                        return bitmapImage;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не получилось от рендерить изображение \n {ex.Message}");
                    return null;
                }
            });
        }

        /// <summary>
        /// Производит преобразование из BitmapImage в Bitmap. 
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        public static Bitmap ConvertBitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                Bitmap bitmap;
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(memory);
                memory.Seek(0, SeekOrigin.Begin);
                bitmap = new Bitmap(memory);

                return bitmap;
            }
        }

        /// <summary>
        /// Возвращает в виде кортежа ширину и высоту изображение.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static (int width, int height) GetImageWidthAndHeight(string path)
        {
            try
            {
                using (var image = Image.FromFile(path))
                {
                    int width = image.Width;
                    int height = image.Height;
                    return (width, height);
                }
            }
            catch (Exception)
            {

                return (0,0);
            }
            
        }

        /// <summary>
        /// Возвращает в виде кортежа ширину и высоту изображение.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task<(int width, int height)> GetImageWidthAndHeightAsync(string path)
        {
            return Task.Run( () =>
            {
                try
                {
                    using (var image = Image.FromFile(path))
                    {
                        int width = image.Width;
                        int height = image.Height;
                        return (width, height);
                    }
                }
                catch (Exception)
                {

                    return (0, 0);
                }
            });
            
        }

        /// <summary>
        /// Возвращает строковое значение разрешение файла в формате "Ширина x Высота" изображение
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// 
        public static string GetImageResolution(string path)
        {
                try
                {
                    using (var image = Image.FromFile(path))
                    {
                        string width = image.Width.ToString();
                        string height = image.Height.ToString();
                        return width + " x " + height;
                    }
                }
                catch (Exception)
                {
                    return string.Empty;
                }
        }

        /// <summary>
        /// Возвращает строковое значение разрешение файла в формате "Ширина x Высота" изображение
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task<string> GetImageResolutionAsync(string path)
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var image = Image.FromFile(path))
                    {
                        string width = image.Width.ToString();
                        string height = image.Height.ToString();
                        return width + " x " + height;
                    }
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            });
        }

        /// <summary>
        /// Изменение размера изображение, без его обрезки
        /// </summary>
        /// <returns>Изображение в Bitmap</returns>
        public static Task<Bitmap> ResizeImageAsync(Bitmap image, int width, int height)
        {
            return Task.Run(() =>
            {
                try
                {
                    Bitmap newImage = new Bitmap(width, height);

                    using (Graphics graphics = Graphics.FromImage(newImage))
                    {
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        graphics.DrawImage(image, 0, 0, width, height);
                    }
                    return newImage;
                }
                catch (Exception)
                {
                    MessageBox.Show($"Значение слишком велики, изображение вернулось обычного размера");
                    return image;
                }
                
            });
        }

        /// <summary>
        /// Устанавливает параметр качества изображение. Где 0 это наихудшее качествао, а 100 наилудшее 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        public static Bitmap SetQualityImage(Bitmap bitmap, int quality)
        {
            return null;
        }

        #endregion
        
        #region Фильтры

        /// <summary>
        /// Накладывает "Ч/Б" фильтр на изображение.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Task<Bitmap> BlackAndWhiteFilter(Bitmap bitmap, double intensity = 1, int alpha = 100)
        {
           // Bitmap BAWBitmap = new(bitmap.Width, bitmap.Height);

            return Task.Run(() =>
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        Color OriginalColor = bitmap.GetPixel(x, y);

                        int grayValue = (int)(OriginalColor.R * (0.3 * intensity) + OriginalColor.G * (0.59 * intensity) + OriginalColor.B * (0.11 * intensity));

                        grayValue = Math.Min(Math.Max(grayValue, 0), 255);

                        bitmap.SetPixel(x, y, Color.FromArgb(alpha ,grayValue, grayValue, grayValue));
                    }
                }
                return bitmap;
            });

            
        }
        
        /// <summary>
        /// Накладывает фильтр "Сепия" на изображение
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="intensity"></param>
        /// <returns></returns>
        public static Task<Bitmap> SepiaFilter(Bitmap bitmap, double intensity = 1, int alpha = 100)
        {
           // Bitmap SepiaBitmap = new(bitmap.Width, bitmap.Height);

            return Task.Run(() => 
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        Color OriginalColor = bitmap.GetPixel(x, y);

                        int sepiaR = (int)(OriginalColor.R * (0.393 * intensity) + OriginalColor.G * (0.769 * intensity) + OriginalColor.B * (0.189 * intensity));
                        int sepiaG = (int)(OriginalColor.R * (0.349 * intensity) + OriginalColor.G * (0.686 * intensity) + OriginalColor.B * (0.168 * intensity));
                        int sepiaB = (int)(OriginalColor.R * (0.272 * intensity) + OriginalColor.G * (0.534 * intensity) + OriginalColor.B * (0.131 * intensity));

                        //int sepiaR = (int)(OriginalColor.R * (0.6 * intensity) + OriginalColor.G * (0.2 * intensity) + OriginalColor.B * (0.7 * intensity));
                        //int sepiaG = (int)(OriginalColor.R * (0.1 * intensity) + OriginalColor.G * (0.1 * intensity) + OriginalColor.B * (0.8 * intensity));
                        //int sepiaB = (int)(OriginalColor.R * (0.54 * intensity) + OriginalColor.G * (0.9 * intensity) + OriginalColor.B * (0.5 * intensity));

                        sepiaR = Math.Min(sepiaR, 255);
                        sepiaG = Math.Min(sepiaG, 255);
                        sepiaB = Math.Min(sepiaB, 255);

                        bitmap.SetPixel(x, y, Color.FromArgb(alpha, sepiaR, sepiaG, sepiaB));
                    }
                }
                return bitmap;
            });           
        }

        /// <summary>
        /// Инверсия цветов изображение.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Task<Bitmap> ColoriInversionFilter(Bitmap bitmap, int alpha)
        {
           // Bitmap InvertedBitmap = new(bitmap.Width, bitmap.Height);

            return Task.Run(() =>
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        Color originalColor = bitmap.GetPixel(x, y);

                        int invertedR = 255 - originalColor.R;
                        int invertedG = 255 - originalColor.G;
                        int invertedB = 255 - originalColor.B;

                        bitmap.SetPixel(x, y, Color.FromArgb(alpha, invertedR, invertedG, invertedB));
                    }
                }
                return bitmap;
            });
        }

        /// <summary>
        /// Накладывает фильтр "Ретро"
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="intensity"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static Task<Bitmap> RetroFilter(Bitmap bitmap, int intensity = 1, int alpha = 100) 
        {
            Random random = new();

            return Task.Run(() => 
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        Color OriginalColor = bitmap.GetPixel(x, y);

                        //int retroR = (int)(OriginalColor.R * (0.393 * intensity) + OriginalColor.G * (0.769 * intensity) + OriginalColor.B * (0.189 * intensity));
                        //int retroG = (int)(OriginalColor.R * (0.349 * intensity) + OriginalColor.G * (0.686 * intensity) + OriginalColor.B * (0.168 * intensity));
                        //int retroB = (int)(OriginalColor.R * (0.272 * intensity) + OriginalColor.G * (0.534 * intensity) + OriginalColor.B * (0.131 * intensity));

                        int retroR = (int)(OriginalColor.R * 0.393 + OriginalColor.G * 0.769 + OriginalColor.B * 0.189);
                        int retroG = (int)(OriginalColor.R * 0.349 + OriginalColor.G * 0.686 + OriginalColor.B * 0.168);
                        int retroB = (int)(OriginalColor.R * 0.272 + OriginalColor.G * 0.534 + OriginalColor.B * 0.131);

                        int noise = random.Next(-30,31);

                        retroR = Math.Clamp(retroR + noise, 0, 255);
                        retroG = Math.Clamp(retroG + noise, 0, 255);
                        retroB = Math.Clamp(retroB + noise, 0, 255);

                        bitmap.SetPixel(x,y, Color.FromArgb(alpha ,retroR, retroG, retroB));

                    }
                }
                return bitmap;
            });
        }

        #endregion   
    }
}