namespace FileFlex.Utils.Services.NavigationServices
{
    public interface IWindowNavigationService
    {
        public void NavigateTo(string pageName, object parameter = null);
    }
}
