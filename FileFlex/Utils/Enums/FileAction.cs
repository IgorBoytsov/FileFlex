namespace FileFlex.Utils.Enums
{
    enum FileAction
    {
        /// <summary>
        /// Действие для открытие одного файла.
        /// </summary>
        Open = 0,
        /// <summary>
        /// Действие для добавление файлов.
        /// </summary>
        Add = 1,
        /// <summary>
        /// 
        /// </summary>
        Update = 2,
        /// <summary>
        /// Действие для удаление файлов из памяти.
        /// </summary>
        Remove = 3,
        /// <summary>
        /// Действие для удаление файлов с устройства.
        /// </summary>
        Delete = 4,
        /// <summary>
        /// Действие для перемещение файлов в корзину, с последующей возможностью восстановить их.
        /// </summary>
        MoveToTrash = 5,
        /// <summary>
        /// Действие для очистки списка файлов, либо данных о файле.
        /// </summary>
        Clear = 6,
        /// <summary>
        /// Дейсвтие для получение файло из деректории
        /// </summary>
        AddFromDirectory = 7,
    }
}
