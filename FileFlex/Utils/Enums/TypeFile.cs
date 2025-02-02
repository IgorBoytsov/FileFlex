﻿namespace FileFlex.Utils.Enums
{
    public enum TypeFile
    {
        /// <summary>
        /// Для отсутствия выбранного файла. При запуске приложение, либо после очистки списка файлов.
        /// </summary>
        None,
        /// <summary>
        ///  Для следующих файлов: JPEG, PNG, Ico, TIFF, WebP
        /// </summary>
        Image,
        /// <summary>
        /// Для файлов без возможности отображение содержимого. К примеру .exe
        /// </summary>
        IconFile,
        GIF,
        Exe,
    }
}
