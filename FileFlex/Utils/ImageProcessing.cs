using FileFlex.Model;
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
            return result;
        }

        public static async Task<BitmapImage> RenderImageAsync(Bitmap bitmap)
        {
            return await Task.Run(() =>
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
            });
        } 

        public static Bitmap SaveRender(BitmapImage bitmapImage)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(memory);
                memory.Seek(0, SeekOrigin.Begin);

                Bitmap bitmap = new Bitmap(memory);

                return bitmap;
            }
        }

        #endregion
    }
}