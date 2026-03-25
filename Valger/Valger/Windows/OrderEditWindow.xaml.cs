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
    /// Логика взаимодействия для OrderEditWindow.xaml
    /// </summary>
    public partial class OrderEditWindow : Window
    {
        private Context _context;
        private Order _order;
        public OrderEditWindow(Context context, Order order)
        {
            _context = context;
            _order = order;
            InitializeComponent();
            LoadDataForComboBoxes();
            FillFormWithData();
        }
        private void LoadDataForComboBoxes()
        {
            cb_equipment.ItemsSource = _context.Equipment.ToList();
            cb_pickupPoint.ItemsSource = _context.PickupPoints.ToList();
        }

        private void FillFormWithData()
        {
            if (_order.OrderId == 0) 
            {
                dp_startDate.SelectedDate = DateTime.Now;
            }
            else 
            {
                var equipment = _context.Equipment
                    .FirstOrDefault(e => e.EquipmentId == _order.EquipmentId);
                cb_equipment.SelectedItem = equipment;

                tb_quantity.Text = _order.RentalQuantity.ToString();
                var pickupPoint = _context.PickupPoints
                    .FirstOrDefault(p => p.PointId == _order.PickupPointId);
                cb_pickupPoint.SelectedItem = pickupPoint;

                dp_startDate.SelectedDate = _order.RentalStartDate;
            }
        }

        private void bt_cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void bt_save_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckIfAllFieldsAreCorrect())
                return;

            try
            {
                if (_order.OrderId == 0)
                {
                    FillRequiredFieldsForNewOrder();
                }
                SaveFormDataToOrder();
                if (_order.OrderId == 0)
                {
                    _context.Orders.Add(_order);
                }
                _context.SaveChanges();

                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}\n\nПолная ошибка: {ex.ToString()}", "Ошибка");
            }
        }
        private bool CheckIfAllFieldsAreCorrect()
        {
            if (cb_equipment.SelectedItem == null)
            {
                MessageBox.Show("Выберите оборудование", "Ошибка");
                return false;
            }

            if (!int.TryParse(tb_quantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите правильное количество дней аренды", "Ошибка");
                return false;
            }

            if (cb_pickupPoint.SelectedItem == null)
            {
                MessageBox.Show("Выберите пункт выдачи", "Ошибка");
                return false;
            }

            if (dp_startDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату начала аренды", "Ошибка");
                return false;
            }

            return true;
        }

        private void FillRequiredFieldsForNewOrder()
        {
            decimal maxOrderNumber = _context.Orders.Any()
                ? _context.Orders.Max(o => o.OrderNumber)
                : 0;
            _order.OrderNumber = maxOrderNumber + 1;
            Random random = new Random();
            _order.ReceiptCode = random.Next(100000, 999999);

            _order.StatusId = 1;

            if (LoginCookies.loggedUser != null)
            {
                _order.ClientUserId = LoginCookies.loggedUser.UserId;
            }
            else
            {
                var firstUser = _context.Users.FirstOrDefault();
                if (firstUser != null)
                    _order.ClientUserId = firstUser.UserId;
                else
                    _order.ClientUserId = 1; 
            }
        }

        private void SaveFormDataToOrder()
        {
            var selectedEquipment = (Equipment)cb_equipment.SelectedItem;
            _order.EquipmentId = selectedEquipment.EquipmentId;

            _order.RentalQuantity = int.Parse(tb_quantity.Text);

            var selectedPickupPoint = (PickupPoint)cb_pickupPoint.SelectedItem;
            _order.PickupPointId = selectedPickupPoint.PointId;

            _order.RentalStartDate = dp_startDate.SelectedDate.Value;
        }
    }
}
