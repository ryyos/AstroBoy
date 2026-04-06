using AstroBoy.Views.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace AstroBoy
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new LoginPage());
        }
    }
}