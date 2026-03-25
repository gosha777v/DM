using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Valger.Data;
using Valger.Models;
using Valger.Windows;
using Valger.Windows.UserControls;

namespace Valger
{
    public partial class MainWindow : Window
    {
        private Context _context = new();
        private string _currentSearchQuery = string.Empty;
        private string _currentManufacturerFilter = "Все производители";
        private bool _sortAscending = true;

        public MainWindow()
        {
            InitializeComponent();

            // Теперь БД уже инициализирована в App.xaml.cs
            InitializeManufacturerComboBox();
            DisplayUserInfo();
            UpdateList();
            SetButtonVisibilityByRole();
        }

        private void DisplayUserInfo()
        {
            if (LoginCookies.loggedUser != null)
            {
                if (LoginCookies.loggedUser.UserId == 0)
                {
                    tb_userInfo.Text = "Гость";
                }
                else
                {
                    var role = _context.Roles
                        .FirstOrDefault(r => r.RoleId == LoginCookies.loggedUser.RoleId);

                    string userRole = role != null ? role.RoleName : "Пользователь";
                    tb_userInfo.Text = $"{LoginCookies.loggedUser.FullName} ({userRole})";
                }
            }
        }

        private void InitializeManufacturerComboBox()
        {
            cb_manufacturer.Items.Add(new ComboBoxItem { Content = "Все производители" });

            foreach (var manufacturer in _context.Manufacturers.ToList())
            {
                cb_manufacturer.Items.Add(manufacturer);
            }

            cb_manufacturer.SelectedIndex = 0;
        }

        private void SetButtonVisibilityByRole()
        {
            if (LoginCookies.loggedUser == null) return;

            string userRole;
            if (LoginCookies.loggedUser.UserId == 0)
            {
                userRole = "Гость";
            }
            else
            {
                var role = _context.Roles
                    .FirstOrDefault(r => r.RoleId == LoginCookies.loggedUser.RoleId);
                userRole = role != null ? role.RoleName : "Пользователь";
            }

            bool isManagerOrAdmin = userRole == "Менеджер" || userRole == "Администратор";

            bt_rental.Visibility = isManagerOrAdmin ? Visibility.Visible : Visibility.Collapsed;
            bt_create.Visibility = userRole == "Администратор" ? Visibility.Visible : Visibility.Collapsed;
            bt_delete.Visibility = userRole == "Администратор" ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateList()
        {
            var filteredEquipment = _context.Equipment
                .Include(e => e.Manufacturer)
                .Include(e => e.Type)
                .Include(e => e.Supplier)
                .AsQueryable();

            string userRole = "";
            if (LoginCookies.loggedUser != null)
            {
                if (LoginCookies.loggedUser.UserId == 0)
                {
                    userRole = "Гость";
                }
                else
                {
                    var role = _context.Roles
                        .FirstOrDefault(r => r.RoleId == LoginCookies.loggedUser.RoleId);
                    userRole = role != null ? role.RoleName : "Пользователь";
                }
            }

            bool isManagerOrAdmin = userRole == "Менеджер" || userRole == "Администратор";

            if (isManagerOrAdmin)
            {
                if (!string.IsNullOrEmpty(_currentSearchQuery))
                {
                    string searchQuery = _currentSearchQuery.ToLower();
                    filteredEquipment = filteredEquipment.Where(eq =>
                        (eq.Name != null && eq.Name.ToLower().Contains(searchQuery)) ||
                        (eq.Article != null && eq.Article.ToLower().Contains(searchQuery)) ||
                        (eq.Description != null && eq.Description.ToLower().Contains(searchQuery)) ||
                        (eq.Manufacturer != null && eq.Manufacturer.ManufacturerName != null &&
                         eq.Manufacturer.ManufacturerName.ToLower().Contains(searchQuery)) ||
                        (eq.Type != null && eq.Type.TypeName != null &&
                         eq.Type.TypeName.ToLower().Contains(searchQuery)) ||
                        (eq.Supplier != null && eq.Supplier.SupplierName != null &&
                         eq.Supplier.SupplierName.ToLower().Contains(searchQuery)));
                }

                if (_currentManufacturerFilter != "Все производители")
                {
                    filteredEquipment = filteredEquipment.Where(eq =>
                        eq.Manufacturer != null &&
                        eq.Manufacturer.ManufacturerName == _currentManufacturerFilter);
                }

                filteredEquipment = _sortAscending
                    ? filteredEquipment.OrderBy(eq => eq.AvailableQuantity)
                    : filteredEquipment.OrderByDescending(eq => eq.AvailableQuantity);
            }
            else
            {
                filteredEquipment = filteredEquipment.OrderBy(eq => eq.Name);
            }

            lv_equipment.Items.Clear();

            foreach (var equipment in filteredEquipment.ToList())
            {
                lv_equipment.Items.Add(new ListElement(equipment));
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LoginCookies.loggedUser != null && LoginCookies.loggedUser.UserId != 0)
            {
                var role = _context.Roles
                    .FirstOrDefault(r => r.RoleId == LoginCookies.loggedUser.RoleId);

                string userRole = role != null ? role.RoleName : "";

                if (userRole == "Менеджер" || userRole == "Администратор")
                {
                    _currentSearchQuery = searchBox.Text;
                    UpdateList();
                }
            }
        }

        private void Manufacturer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LoginCookies.loggedUser != null && LoginCookies.loggedUser.UserId != 0)
            {
                var role = _context.Roles
                    .FirstOrDefault(r => r.RoleId == LoginCookies.loggedUser.RoleId);

                string userRole = role != null ? role.RoleName : "";

                if (userRole == "Менеджер" || userRole == "Администратор")
                {
                    if (cb_manufacturer.SelectedItem is ComboBoxItem comboBoxItem)
                    {
                        _currentManufacturerFilter = comboBoxItem.Content.ToString();
                    }
                    else if (cb_manufacturer.SelectedItem is Manufacturer manufacturer)
                    {
                        _currentManufacturerFilter = manufacturer.ManufacturerName;
                    }
                    else
                    {
                        _currentManufacturerFilter = "Все производители";
                    }

                    UpdateList();
                }
            }
        }

        private void SortByQuantityAscending_Click(object sender, RoutedEventArgs e)
        {
            if (LoginCookies.loggedUser != null && LoginCookies.loggedUser.UserId != 0)
            {
                var role = _context.Roles
                    .FirstOrDefault(r => r.RoleId == LoginCookies.loggedUser.RoleId);

                string userRole = role != null ? role.RoleName : "";

                if (userRole == "Менеджер" || userRole == "Администратор")
                {
                    _sortAscending = true;
                    UpdateList();
                }
            }
        }

        private void SortByQuantityDescending_Click(object sender, RoutedEventArgs e)
        {
            if (LoginCookies.loggedUser != null && LoginCookies.loggedUser.UserId != 0)
            {
                var role = _context.Roles
                    .FirstOrDefault(r => r.RoleId == LoginCookies.loggedUser.RoleId);

                string userRole = role != null ? role.RoleName : "";

                if (userRole == "Менеджер" || userRole == "Администратор")
                {
                    _sortAscending = false;
                    UpdateList();
                }
            }
        }

        private void bt_create_Click(object sender, RoutedEventArgs e)
        {
            if (LoginCookies.loggedUser != null && LoginCookies.loggedUser.UserId != 0 &&
                LoginCookies.loggedUser.RoleId == 1) // 1 = Администратор
            {
                EditWindow editWindow = new EditWindow(_context, new Equipment());
                editWindow.ShowDialog();
                if (editWindow.DialogResult == true) UpdateList();
            }
            else
            {
                MessageBox.Show("У вас нет прав для добавления оборудования.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void bt_delete_Click(object sender, RoutedEventArgs e)
        {
            if (LoginCookies.loggedUser != null && LoginCookies.loggedUser.UserId != 0 &&
                LoginCookies.loggedUser.RoleId == 1)
            {
                if (lv_equipment.SelectedItem is ListElement listElement)
                {
                    Equipment equipment = listElement.Equipment;

                    if (_context.Orders.Any(o => o.EquipmentId == equipment.EquipmentId))
                    {
                        MessageBox.Show("Нельзя удалить оборудование, которое присутствует в аренде.",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _context.Equipment.Remove(equipment);
                    _context.SaveChanges();
                    UpdateList();
                }
            }
            else
            {
                MessageBox.Show("У вас нет прав для удаления оборудования.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void bt_exit_Click(object sender, RoutedEventArgs e)
        {
            LoginCookies.loggedUser = null;
            LoginWindow authorization = new LoginWindow(_context);
            authorization.Show();
            this.Close();
        }

        private void lv_equipment_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lv_equipment.SelectedItem is ListElement listElement)
            {
                Equipment equipment = listElement.Equipment;
                if (LoginCookies.loggedUser != null && LoginCookies.loggedUser.UserId != 0 &&
                    LoginCookies.loggedUser.RoleId == 1)
                {
                    EditWindow editWindow = new EditWindow(_context, equipment);
                    editWindow.ShowDialog();
                    if (editWindow.DialogResult == true) UpdateList();
                }
                else
                {
                    MessageBox.Show("У вас нет прав для редактирования оборудования.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void bt_rental_Click(object sender, RoutedEventArgs e)
        {
            if (LoginCookies.loggedUser != null && LoginCookies.loggedUser.UserId != 0)
            {
                OrdersWindow ordersWindow = new OrdersWindow(_context);
                ordersWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("У вас нет доступа к управлению арендой.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}