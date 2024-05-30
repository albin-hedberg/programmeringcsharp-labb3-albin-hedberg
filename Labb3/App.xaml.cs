using System.Threading.Tasks;
using System.Windows;
using Labb3ProgTemplate.Managers;

namespace Labb3ProgTemplate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Task.WhenAll(UserManager.LoadUsersFromFile(), ProductManager.LoadProductsFromFile());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            Task.WhenAll(UserManager.SaveUsersToFile(), ProductManager.SaveProductsToFile());
        }
    }
}
