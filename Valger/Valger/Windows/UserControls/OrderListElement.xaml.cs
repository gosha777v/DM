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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Valger.Models;

namespace Valger.Windows.UserControls
{
    /// <summary>
    /// Логика взаимодействия для OrderListElement.xaml
    /// </summary>
    public partial class OrderListElement : UserControl
    {
        public Order Order { get; set; }
        public event MouseButtonEventHandler DoubleClick;
        public OrderListElement(Order order)
        {
            Order = order;
            InitializeComponent();
            
            DataContext = order;
            this.MouseDoubleClick += (s, e) => DoubleClick?.Invoke(this, e);
        }
        public void SetSelected(bool isSelected)
        {
            if (isSelected)
            {
                this.BorderBrush = Brushes.Blue;
                this.BorderThickness = new Thickness(2);
                this.Background = Brushes.LightBlue;
            }
            else
            {
                this.BorderBrush = Brushes.Black;
                this.BorderThickness = new Thickness(1);
                this.Background = Brushes.White;
            }
        }
    }
}
