using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Valger.Data;
using Valger.Models;
using Valger.Windows.UserControls;

namespace Valger.Windows
{
    /// <summary>
    /// Логика взаимодействия для OrdersWindow.xaml
    /// </summary>
    public partial class OrdersWindow : Window
    {
        private Context _context = new();
        private Order _selectedRental;

        public OrdersWindow(Context context)
        {
            _context = context;
            InitializeComponent();
            LoadRentals();
            SetButtonVisibilityByRole();
        }

        private void SetButtonVisibilityByRole()
        {
            if (LoginCookies.loggedUser == null) return;

            var userRole = _context.Roles
                .Where(r => r.RoleId == LoginCookies.loggedUser.RoleId)
                .Select(r => r.RoleName)
                .FirstOrDefault();

            if (userRole == "Администратор")
            {
                bt_addRental.Visibility = Visibility.Visible;
                bt_deleteRental.Visibility = Visibility.Visible;
            }
            else
            {
                bt_addRental.Visibility = Visibility.Collapsed;
                bt_deleteRental.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadRentals()
        {
            wp_rentals.Children.Clear();

            var rentals = _context.Orders
                .Include(o => o.Equipment)
                .Include(o => o.PickupPoint)
                .Include(o => o.ClientUser)
                .Include(o => o.Status)
                .OrderByDescending(o => o.RentalStartDate)
                .ToList();

            // Очищаем панель
            wp_rentals.Children.Clear();

            // Просто добавляем все карточки в WrapPanel
            foreach (var rental in rentals)
            {
                OrderListElement card = new OrderListElement(rental);
                card.Margin = new Thickness(0, 0, 10, 10);
                card.MouseLeftButtonDown += (s, e) => SelectRental(rental);
                card.DoubleClick += (s, e) => EditSelectedRental();
                wp_rentals.Children.Add(card);
            }

            // Настраиваем WrapPanel для 2 колонок
            wp_rentals.Width = 800; // Ширина окна минус отступы
            _selectedRental = null;
        }

        private void SelectRental(Order rental)
        {
            _selectedRental = rental;

            // Снимаем выделение со всех карточек
            foreach (UIElement element in wp_rentals.Children)
            {
                if (element is OrderListElement card)
                {
                    card.SetSelected(card.Order.OrderId == rental.OrderId);
                }
            }
        }

        // Метод для редактирования выбранной аренды
        private void EditSelectedRental()
        {
            if (_selectedRental != null)
            {
                // Редактирование доступно только администратору
                if (LoginCookies.loggedUser.RoleId == 1)
                {
                    OrderEditWindow editWindow = new OrderEditWindow(_context, _selectedRental);
                    editWindow.ShowDialog();
                    if (editWindow.DialogResult == true) LoadRentals();
                }
                else
                {
                    MessageBox.Show("У вас нет прав для редактирования аренды.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void bt_addRental_Click(object sender, RoutedEventArgs e)
        {
            if (LoginCookies.loggedUser.RoleId == 1)
            {
                OrderEditWindow editWindow = new OrderEditWindow(_context, new Order());
                editWindow.ShowDialog();
                if (editWindow.DialogResult == true) LoadRentals();
            }
            else
            {
                MessageBox.Show("У вас нет прав для добавления аренды.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void bt_deleteRental_Click(object sender, RoutedEventArgs e)
        {
            if (LoginCookies.loggedUser.RoleId == 1)
            {
                if (_selectedRental != null)
                {
                    var result = MessageBox.Show("Вы уверены, что хотите удалить эту аренду?",
                                               "Подтверждение удаления",
                                               MessageBoxButton.YesNo,
                                               MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _context.Orders.Remove(_selectedRental);
                        _context.SaveChanges();
                        LoadRentals();
                    }
                }
                else
                {
                    MessageBox.Show("Выберите аренду для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("У вас нет прав для удаления аренды.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Альтернативный вариант: обработчик двойного клика для всей формы
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Если кликнули не по карточке, а по пустому месту
            if (e.OriginalSource is Border || e.OriginalSource is Grid || e.OriginalSource is StackPanel)
            {
                EditSelectedRental();
            }
        }

        private void bt_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}


