using System.Windows;
using Valger.Data;
using Valger.Windows;

namespace Valger
{
    public partial class App : Application
    {
        private Context _context = new Context();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Инициализируем базу данных
            DatabaseInitializer.Initialize(_context);

            // Создаем и показываем окно логина
            LoginWindow loginWindow = new LoginWindow(_context);
            loginWindow.Show();
        }
    }
}