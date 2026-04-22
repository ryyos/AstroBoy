using AstroBoy.Views.Auth;
using Database;
using Microsoft.Extensions.DependencyInjection;

namespace AstroBoy
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            InitDatabase();
            MainPage = new NavigationPage(new LoginPage());
        }

        private async void InitDatabase()
        {
            var db = new DatabaseContext();
            await db.InitDatabaseAsync();
        }
    }
}