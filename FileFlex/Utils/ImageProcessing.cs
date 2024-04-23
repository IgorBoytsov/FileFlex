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
        /// Производит преоброазование из Bitmap в BitmapImage. 
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
                    MessageBox.Show($"Не получилось отрендерить изображение \n {ex.Message}");
                    return null;
                }
            });
        }

        /// <summary>
        /// Производит преоброазование из BitmapImage в Bitmap. 
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
    }
}