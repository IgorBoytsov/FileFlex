using FileFlex.Command;
using FileFlex.MVVM.ViewModels.BaseVM;
using FileFlex.Utils.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FileFlex.MVVM.ViewModels.WindowViewModels
{
    public class MessageWindowViewModel : BaseViewModel
    {
        /*-Хранение входных параметров--------------------------------------------------------------------*/

        public bool ResultAction { get; set; }

        public string Message { get; set; }

        public string HeaderMessage { get; set; }

        /*--Visibility------------------------------------------------------------------------------------*/

        #region Свойства : Visibility у кнопок 

        private Visibility _okCancelButtonVisibility;
        public Visibility OkCancelButtonVisibility
        {
            get => _okCancelButtonVisibility;
            set
            {
                _okCancelButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _yesNoButtonVisibility;
        public Visibility YesNoButtonVisibility
        {
            get => _yesNoButtonVisibility;
            set
            {
                _yesNoButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        #endregion

        /*--Конструктор-----------------------------------------------------------------------------------*/

        public MessageWindowViewModel(string message, string headerMessage, TypeMessage typeMessage)
        {
            Message = message;
            HeaderMessage = headerMessage;

            TypeMessageSelector(typeMessage);
        }

        /*--Команды---------------------------------------------------------------------------------------*/

        private RelayCommand _checkButtonClickCommand;
        public RelayCommand CheckButtonClickCommand => _checkButtonClickCommand ??= new RelayCommand(CheckButtonClick);

        /*--Выбор и установка свойства для окна-----------------------------------------------------------*/

        #region Свойства : Цвета

        private Brush _borderBackground;
        public Brush BorderBackground
        {
            get => _borderBackground;
            set
            {
                _borderBackground = value;
                OnPropertyChanged();
            }
        }

        #endregion      

        #region Метод : Выбор нужных свойств взависимости от TypeMessage

        private void TypeMessageSelector(TypeMessage typeMessage)
        {
            Action action = typeMessage switch
            {
                TypeMessage.Information => () =>
                {
                    OkCancelButtonVisibility = Visibility.Visible;
                    YesNoButtonVisibility = Visibility.Collapsed;

                    BorderBackground = (Brush)new BrushConverter().ConvertFromString("#FF0A791F"); // Зеленый
                }
                ,
                TypeMessage.Warning => () =>
                {
                    OkCancelButtonVisibility = Visibility.Visible;
                    YesNoButtonVisibility = Visibility.Collapsed;

                    BorderBackground = (Brush)new BrushConverter().ConvertFromString("#FFB29A1E"); // Желтый
                }
                ,
                TypeMessage.Error => () =>
                {
                    OkCancelButtonVisibility = Visibility.Visible;
                    YesNoButtonVisibility = Visibility.Collapsed;

                    BorderBackground = (Brush)new BrushConverter().ConvertFromString("#FF7D1515"); // Красный
                }
                ,
                TypeMessage.Question => () =>
                {
                    OkCancelButtonVisibility = Visibility.Collapsed;
                    YesNoButtonVisibility = Visibility.Visible;

                    BorderBackground = (Brush)new BrushConverter().ConvertFromString("#FF0043B4"); // Синий
                }
                ,
                _ => () => throw new Exception("Такого типа сообщений нету")
            };
            action.Invoke();
        }

        #endregion

        /*--Проверки--------------------------------------------------------------------------------------*/

        #region Метод : Проверка на нажатую кнопку

        private void CheckButtonClick(object value)
        {
            if (value is Button button)
            {
                Action action = button.Content switch
                {
                    "Ок" => () =>
                    {
                        ResultAction = true;
                        CloseWnd();
                    }
                    ,
                    "Да" => () =>
                    {
                        ResultAction = true;
                        CloseWnd();
                    }
                    ,
                    "Отмена" => () =>
                    {
                        ResultAction = false;
                        CloseWnd();
                    }
                    ,
                    "Нет" => () =>
                    {
                        ResultAction = false;
                        CloseWnd();
                    }
                    ,
                    _ => () => ResultAction = false
                };
                action.Invoke();
            }
        }

        #endregion

        #region Метод : Закрытие окна, после нажатия на кнопку результата

        private static void CloseWnd()
        {
            Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.GetType().Name == "MessageWindow").Close();
        }

        #endregion  
    }
}