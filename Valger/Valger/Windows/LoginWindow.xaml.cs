using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls; // Добавьте эту директиву для PasswordBox
using Valger.Data;
using Valger.Models;

namespace Valger.Windows
{
    public partial class LoginWindow : Window
    {
        private Context _context;

        public LoginWindow(Context context)
        {
            _context = context;
            InitializeComponent();
            tbox_login.Focus();
        }

        private void bt_login_Click(object sender, RoutedEventArgs e)
        {
            // Используйте Password вместо Text для PasswordBox
            if (!string.IsNullOrEmpty(tbox_login.Text.Trim())
                && !string.IsNullOrEmpty(tbox_pass.Password.Trim())) // ← ИЗМЕНИТЕ ЗДЕСЬ
            {
                try
                {
                    User? loggedUser = _context.Users
                        .Include(u => u.Role)
                        .Where(u => u.Login == tbox_login.Text.Trim()
                            && u.Password == tbox_pass.Password.Trim()) // ← И ЗДЕСЬ
                        .FirstOrDefault();

                    if (loggedUser != null)
                    {
                        LoginCookies.loggedUser = loggedUser;

                        // Открываем главное окно и закрываем окно логина
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль",
                            "Ошибка авторизации",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void ButtonGuest(object sender, RoutedEventArgs e)
        {
            LoginCookies.loggedUser = new User
            {
                UserId = 0,
                FullName = "Гость",
                RoleId = 0
            };

            // Открываем главное окно и закрываем окно логина
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}