namespace FileFlex.Utils.Services.NavigationServices
{
    public interface IWindowNavigationService
    {
        /// <summary>
        /// Требуется передать полное название страницы, без .xaml
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="parameter"></param>
        public void NavigateTo(string pageName, object parameter = null);
    }
}
