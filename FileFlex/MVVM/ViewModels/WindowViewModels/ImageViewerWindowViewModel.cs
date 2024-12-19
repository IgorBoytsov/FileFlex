using FileFlex.Command;
using FileFlex.MVVM.Model.AppModel;
using FileFlex.MVVM.ViewModels.BaseVM;
using FileFlex.Utils.Services.NavigationServices;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows;
using FileFlex.Utils.Enums;

namespace FileFlex.MVVM.ViewModels.WindowViewModels
{
    class ImageViewerWindowViewModel : BaseViewModel, IUpdatable
    {

        #region Трансформация изображение

        private double _scaleX = 1.0;
        public double ScaleX
        {
            get => _scaleX;
            set
            {
                _scaleX = value;
                OnPropertyChanged();
            }
        }

        private double _scaleY = 1.0;
        public double ScaleY
        {
            get => _scaleY;
            set
            {
                _scaleY = value;
                OnPropertyChanged();
            }
        }

        private double _translateX = 0.0;
        public double TranslateX
        {
            get => _translateX;
            set
            {
                _translateX = value;
                OnPropertyChanged();
            }
        }

        private double _translateY = 0.0;
        public double TranslateY
        {
            get => _translateY;
            set
            {
                _translateY = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _resetTransformCommand;
        public RelayCommand ResetTransformCommand { get => _resetTransformCommand ??= new(obj => { ResetTransform(); }); }

        private void ResetTransform()
        {
            ScaleX = 1.0;
            ScaleY = 1.0;

            TranslateX = 0.0;
            TranslateY = 0.0;
        }

        #endregion

        public ImageViewerWindowViewModel()
        {

        }

        private string _currentImage;
        public string CurrentImage
        {
            get => _currentImage;
            set
            {
                _currentImage = value;
                OnPropertyChanged();
            }
        }

        private TypeFile _currentDisplayFile;
        public TypeFile CurrentDisplayFile
        {
            get => _currentDisplayFile;
            set
            {
                _currentDisplayFile = value;
                OnPropertyChanged();
            }
        }

        public void Update(object parameter)
        {
            if (parameter is FileData fileData)
            {
                Action action = fileData.FileExtension.ToLower() switch
                {
                    ".jpg" or ".jpeg" or ".jfif" or ".jpe" or ".png" or ".ico" or ".webp" or ".heic" => () =>
                    {
                        LoadImage(fileData.FilePath);
                        CurrentDisplayFile = TypeFile.Image;
                    }
                    ,
                    ".gif" => () =>
                    {
                        LoadGif(fileData.FilePath);
                        CurrentDisplayFile = TypeFile.GIF;
                    }
                    ,
                    _ => () => throw new Exception("Такой формат не поддерживается")
                };
                action.Invoke();

                ResetTransform();
            }
        }

        #region Отображение : jpg, jpeg, jfif, jpe, png, ico, webp, heic

        private void LoadImage(string filePath)
        {
            CurrentImage = filePath;
        }

        #endregion

        #region Отображение GIF

        private BitmapSource _currentFrame;
        public BitmapSource CurrentFrame
        {
            get => _currentFrame;
            private set
            {
                if (_currentFrame != value)
                {
                    _currentFrame = value;
                    OnPropertyChanged(nameof(CurrentFrame));
                }
            }
        }

        private Bitmap gifBitmap;
        private BitmapSource[] gifFrames;
        private int currentFrameIndex;
        private DispatcherTimer frameTimer;

        private void LoadGif(string filePath)
        {
            gifBitmap = new Bitmap(filePath);
            gifFrames = ExtractFrames(gifBitmap);

            frameTimer = new DispatcherTimer();
            frameTimer.Interval = TimeSpan.FromMilliseconds(100);
            frameTimer.Tick += UpdateFrame;
            frameTimer.Start();
        }

        private BitmapSource[] ExtractFrames(Bitmap gif)
        {
            int frameCount = gif.GetFrameCount(FrameDimension.Time);
            var frames = new BitmapSource[frameCount];

            for (int i = 0; i < frameCount; i++)
            {
                gif.SelectActiveFrame(FrameDimension.Time, i);
                var frame = new Bitmap(gif);
                frames[i] = Imaging.CreateBitmapSourceFromHBitmap(
                    frame.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                );
                frame.Dispose();
            }

            return frames;
        }

        private void UpdateFrame(object sender, EventArgs e)
        {
            CurrentFrame = gifFrames[currentFrameIndex];
            currentFrameIndex = (currentFrameIndex + 1) % gifFrames.Length;
        }

        public void Dispose()
        {
            frameTimer?.Stop();
            gifBitmap?.Dispose();
            foreach (var frame in gifFrames)
            {
                frame?.Freeze();
            }
        }

        #endregion

    }
}