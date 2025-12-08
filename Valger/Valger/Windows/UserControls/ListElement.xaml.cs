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
    /// Логика взаимодействия для ListElement.xaml
    /// </summary>
    public partial class ListElement : UserControl
    {
        public Equipment Equipment { get; set; }
        public ListElement(Equipment eq)
        {
            Equipment = eq;
            InitializeComponent();
            DataContext = eq;

            if(eq.Discount > 15)
            {
                run_discount.Foreground = new SolidColorBrush(Color.FromRgb(46, 139, 87));
                run_discount.TextDecorations = TextDecorations.Underline;
            }

            if(eq.Discount > 0)
            {
                run_price.Foreground = new SolidColorBrush(Colors.Red);
                run_price.TextDecorations = TextDecorations.Strikethrough;
                run_newPrice.Text = $"{eq.RentalCost - ((eq.RentalCost * eq.Discount) / 100)}";
            }

            if(eq.AvailableQuantity < 1)
            {
                run_amount.Foreground = new SolidColorBrush(Colors.Yellow);
                run_amount.TextDecorations = TextDecorations.Strikethrough;
            }
        }
    }
}
