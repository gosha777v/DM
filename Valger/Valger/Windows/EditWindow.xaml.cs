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

namespace Valger.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private Context _context;
        private Equipment _equipment;
        public EditWindow(Context context, Equipment equipment)
        {
            _context = context;
            _equipment = equipment;
            DataContext = equipment;
            InitializeComponent();

            cb_man.ItemsSource = _context.Manufacturers.ToList();
            cb_supplier.ItemsSource = _context.Suppliers.ToList();
            cb_type.ItemsSource = _context.EquipmentTypes.ToList();

            tbox_article.Text = equipment.Article;
            tbox_name.Text = equipment.Name;
            tbox_amount.Text = equipment.AvailableQuantity.ToString();

            tbox_unit.Text = equipment.RentalUnit;
            tbox_cost.Text = equipment.RentalCost.ToString();
            tbox_discount.Text = equipment.Discount.ToString();

            cb_supplier.SelectedItem = equipment.Supplier;
            cb_man.SelectedItem = equipment.Manufacturer;
            cb_type.SelectedItem = equipment.Type;

            tbox_description.Text = equipment.Description;
        }

        private void bt_cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            return;
        }

        private void bt_create_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(tbox_amount.Text, out int availableQuantity) &&
                decimal.TryParse(tbox_cost.Text, out decimal rentalCost) &&
                decimal.TryParse(tbox_discount.Text, out decimal discount))
            {
                // Обновление данных оборудования
                _equipment.Article = tbox_article.Text;
                _equipment.Name = tbox_name.Text;
                _equipment.AvailableQuantity = availableQuantity;
                _equipment.Description = tbox_description.Text;
                _equipment.Discount = discount;
                _equipment.Manufacturer = cb_man.SelectedItem as Manufacturer;
                _equipment.RentalCost = rentalCost;
                _equipment.RentalUnit = tbox_unit.Text;
                _equipment.Supplier = cb_supplier.SelectedItem as Supplier;
                _equipment.Type = cb_type.SelectedItem as EquipmentType;

                // Сохранение изменений в базе данных
                _context.SaveChanges();

                // Закрытие окна и возвращение успешного результата
                DialogResult = true;
                this.Close();
            }
            else
            {
                // Если данные введены неверно, показываем сообщение об ошибке
                MessageBox.Show("Пожалуйста, убедитесь, что все числовые поля заполнены корректно.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
