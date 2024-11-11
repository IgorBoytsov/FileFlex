using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace FileFlex.Utils.Helpers
{
    public static class IconExtractionHelper
    {
        /// <summary>
        /// Извлекает текущую иконку файла. Подходят любые файлы.
        /// </summary>
        /// <param name="filePath"> Путь к файлу.</param>
        /// <returns></returns>
        public static async Task<BitmapImage> ExtractionFileIconAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                try
                { 
                    Icon icon = Icon.ExtractAssociatedIcon(filePath); //Извлекаем иконку из файла.

                    using (MemoryStream ms = new()) //Используем MemoryStream для временного хранение иконка файла.
                    {
                        icon.ToBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Png); // Сохранение иконки в формате .png в MemoryStream
                        ms.Seek(0, SeekOrigin.Begin); // Перемещаем указатель в начало потока для последующего чтения

                        BitmapImage bitmapImage = new();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = ms;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // Загружаем данные в кэш для освобождения потока после загрузки
                        bitmapImage.EndInit();
                        bitmapImage.Freeze(); // "Замораживаем" изображение для потокобезопасности

                        return bitmapImage;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            });
        }
    }
}