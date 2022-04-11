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
    /// Логика взаимодействия для OrderListWindow.xaml
    /// </summary>
    public partial class OrderListWindow : Window
    {
        public OrderListWindow()
        {
            InitializeComponent();
            LoadContent();
            try
            {
                btnProfile.Content = SystemContext.User.UserLogin;
                btnProfile.Click += ButtonMyProfile_Click;
            }
            catch { }
        }

        void LoadContent()
        {
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                List<Orders> orders;
                try
                {
                    SystemContext.Client = (from c in db.Client where c.UserID == SystemContext.User.UserID select c).FirstOrDefault();
                    orders = (from o in db.Orders where o.ClientID == SystemContext.Client.ClientID orderby o.OrderData descending select o).ToList<Orders>();
                    int i = 0;
                    foreach (var order in orders)
                    {
                        AddNewOrder(i, order.OrderData.ToString(), order.ProductID.ToString(), order.OrderProductCount, order.ShopID.ToString(), order.OrderStatus, order.OrderTotalPrice);
                        i++;
                    }
                }
                catch
                {

                }
            }
        }

        void AddNewOrder(int i, string OrderData, string ProductID, int OrderProductCount, string ShopID, string OrderStatus, int OrderTotalPrice)
        {
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                string[] data = OrderData.Split(' ');
                SystemContext.Client = (from c in db.Client where c.UserID == SystemContext.User.UserID select c).FirstOrDefault();
                SystemContext.Shop = (from s in db.Shop where s.ShopID.ToString() == ShopID select s).FirstOrDefault();
                SystemContext.Product = (from p in db.Product where p.ProductID.ToString() == ProductID select p).FirstOrDefault();

                var borderPanel = new Border() { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(2), Style = (Style)OrdersView.Resources["contentBorderStyle"] };
                var mainGrid = new Grid() { };
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(37, GridUnitType.Star) });
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(33, GridUnitType.Star) });

                TextBlock Order = new TextBlock() { Text = "Заказ от ", Style = (Style)OrdersView.Resources["Lbl"], Margin = new Thickness(5, 0, 0, 0) };
                TextBlock OrderProduct = new TextBlock() { Text = " - ", Style = (Style)OrdersView.Resources["Lbl"], Margin = new Thickness(5, 20, 0, 0), Foreground = Brushes.Black };
                TextBlock OrderAddressText = new TextBlock() { Text = "Адрес доставки:", Style = (Style)OrdersView.Resources["Lbl"], Margin = new Thickness(0, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Center };
                TextBlock OrderAddress = new TextBlock() { Text = $"{SystemContext.Shop.ShopAddress}", Style = (Style)OrdersView.Resources["Lbl"], Margin = new Thickness(0, 20, 0, 0), Foreground = Brushes.Black, HorizontalAlignment = HorizontalAlignment.Center };
                TextBlock OrderStatusCheck = new TextBlock() { Text = "Статус: ", Style = (Style)OrdersView.Resources["Lbl"], Margin = new Thickness(0, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Right };
                TextBlock OrderTotalPriceCheck = new TextBlock() { Text = "Общая стоимость: ", Style = (Style)OrdersView.Resources["Lbl"], Margin = new Thickness(0, 40, 0, 0), HorizontalAlignment = HorizontalAlignment.Right, Foreground = Brushes.Black, FontSize = 17 };
                Order.Inlines.Add(new TextBlock() { Text = $" {data[0]}", Margin = new Thickness(0) });
                OrderProduct.Inlines.Add(new TextBlock() { Text = $" {SystemContext.Product.ProductName}.  Кол-во: {OrderProductCount}", Foreground = (Brush)(new BrushConverter().ConvertFrom("Black")), Margin = new Thickness(0) });
                OrderStatusCheck.Inlines.Add(new TextBlock() { Text = $" {OrderStatus}", Foreground = (Brush)(new BrushConverter().ConvertFrom("Black")), Margin = new Thickness(0) });
                OrderTotalPriceCheck.Inlines.Add(new TextBlock() { Text = $" {OrderTotalPrice}", Foreground = (Brush)(new BrushConverter().ConvertFrom("#A500F3")), Margin = new Thickness(0) });

                Grid.SetColumn(Order, 0);
                Grid.SetColumn(OrderProduct, 0);
                Grid.SetColumn(OrderAddressText, 1);
                Grid.SetColumn(OrderAddress, 1);
                Grid.SetColumn(OrderStatusCheck, 2);
                Grid.SetColumn(OrderTotalPriceCheck, 2);

                mainGrid.Children.Add(Order);
                mainGrid.Children.Add(OrderProduct);
                mainGrid.Children.Add(OrderAddressText);
                mainGrid.Children.Add(OrderAddress);
                mainGrid.Children.Add(OrderStatusCheck);
                mainGrid.Children.Add(OrderTotalPriceCheck);
                borderPanel.Child = mainGrid;
                OrdersView.Children.Add(borderPanel);
            }
        }

        private void ClientMainButton_Click(object sender, RoutedEventArgs e)
        {
            SystemContext.typeWindow = "Каталог";
            ClientMainWindow cmw = new ClientMainWindow();
            this.Close();
            cmw.ShowDialog();
        }

        private void ButtonMyProfile_Click(object sender, RoutedEventArgs e)
        {
            UserProfileWindow upw = new UserProfileWindow();
            this.Close();
            upw.ShowDialog();
        }

        private void ClientCardButton_Click(object sender, RoutedEventArgs e)
        {
            SystemContext.typeWindow = "Корзина";
            ClientMainWindow cmw = new ClientMainWindow();
            this.Close();
            cmw.ShowDialog();
        }
    }


}
