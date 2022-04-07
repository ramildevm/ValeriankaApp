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

namespace ValeriankaApp
{
    /// <summary>
    /// Логика взаимодействия для OrderingWindow.xaml
    /// </summary>
    public partial class OrderingWindow : Window
    {
        public OrderingWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void ConfirmOrderButton_Click(object sender, MouseButtonEventArgs e)
        {

        }

        private void ButtonCatalog_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonCart_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonMyOrders_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
