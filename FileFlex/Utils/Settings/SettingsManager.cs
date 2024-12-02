using FileFlex.MVVM.Model.AppModel;
using FileFlex.MVVM.Model.SettingsModel;
using System.IO;
using System.Text.Json;

namespace FileFlex.Utils.Settings
{
    public class SettingsManager
    {
        /*--Свойства и поля-------------------------------------------------------------------------------*/

        #region Пути к дирректории и файлу с настройками

        // Путь к папке AppData\Roaming
        private readonly static string RoamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        // Путь к папке для хранение настроек у приложения AppData\Roaming\FileFlexApp
        private readonly static string AppDirectory = Path.Combine(RoamingPath, "FileFlexApp");

        // Путь к файлу Json с настройками
        private readonly static string JsonFilePath = AppDirectory + @"\settings.json";

        #endregion

        #region Свойства : Хранение настроек

        public string SaveFilePath { get; private set; }

        public string OpenFilePath { get; private set; }

        #endregion

        #region Поле : Настройки по умолчанию

        private readonly static SettingsData _defaultSettings = new()
        {
            SaveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"),
            OpenFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"),
        };

        #endregion

        #region Событие

        public event Action UpdateSettings;

        #endregion

        public SettingsManager()
        {
            if (!Directory.Exists(AppDirectory))
            {
                CreateDirectory();
            }
            if (!File.Exists(JsonFilePath))
            {
                SetDefaultSettings();
            }

            SetSettings();
        }

        public void NotifyAboutChange()
        {
            UpdateSettings?.Invoke();
        }

        #region Методы : Создание деректории | Создание файла с настройками

        private static void CreateDirectory()
        {
            Directory.CreateDirectory(AppDirectory);
        }

        private static void CreateSettingsFile(string savePath, SettingsData settingsData)
        {
            string json = JsonSerializer.Serialize(settingsData);
            File.WriteAllText(savePath, json);
        }

        #endregion

        #region Методы : Установка | Считывание данных файла настроек

        public void WriteSettings(SettingsData settingsData)
        {
            CreateSettingsFile(JsonFilePath, settingsData);
            SetSettings();
        }

        private void SetSettings()
        {
            string json = File.ReadAllText(JsonFilePath);
            var settingsData = JsonSerializer.Deserialize<SettingsData>(json);

            SaveFilePath = settingsData.SaveFilePath;
            OpenFilePath = settingsData.OpenFilePath;
        }

        public void SetDefaultSettings()
        {
            WriteSettings(_defaultSettings);
            NotifyAboutChange();
        }

        #endregion
    }
}