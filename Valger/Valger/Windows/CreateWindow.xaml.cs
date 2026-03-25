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
    /// Логика взаимодействия для CreateWindow.xaml
    /// </summary>
    public partial class CreateWindow : Window
    {
        private Context _context;
        public CreateWindow(Context context)
        {
            _context = context;
            InitializeComponent();
            Equipment equipment = new();
            DataContext = equipment;

            cb_man.ItemsSource = _context.Manufacturers.ToList(); 
            cb_man.SelectedIndex = 0;
            cb_supplier.ItemsSource = _context.Suppliers.ToList();
            cb_supplier.SelectedIndex = 0;
            cb_type.ItemsSource = _context.EquipmentTypes.ToList();
            cb_type.SelectedIndex = 0;
        }

        private void bt_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bt_create_Click(object sender, RoutedEventArgs e)
        {
            Equipment equipment = new()
            {
                Article = tbox_article.Text,
                Name = tbox_name.Text,
                AvailableQuantity = int.Parse(tbox_amount.Text),
                Description = tbox_description.Text,
                Discount = decimal.Parse(tbox_discount.Text),
                Manufacturer = cb_man.SelectedItem as Manufacturer,
                RentalCost = decimal.Parse(tbox_cost.Text),
                RentalUnit = tbox_unit.Text,
                Supplier = cb_supplier.SelectedItem as Supplier,
                Type = cb_type.SelectedItem as EquipmentType,
            };

            _context.Equipment.Add(equipment);
            _context.SaveChanges();

            DialogResult = true;
            return;
        }
    }
}
