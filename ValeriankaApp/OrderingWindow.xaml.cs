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
            LoadAddress();
            LoadDefault();
            try
            {
                btnProfileText.Text = SystemContext.User.UserLogin;
                btnProfile.Click += ButtonMyProfile_Click;
            }
            catch { }
            TotalPrice.Inlines.Add(new TextBlock() { Text = $" {CalculationTotalPrice()} руб.", Foreground = (Brush)(new BrushConverter().ConvertFrom("#A500F3")), Margin = new Thickness(0) });
        }

        private void LoadDefault()
        {
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                SystemContext.Client = (from c in db.Client where c.UserID == SystemContext.User.UserID select c).FirstOrDefault();
            }

            var client = SystemContext.Client;
            txtFIO.Text = client.ClientFIO;
            txtNumber.Text = client.ClientNumber;
            ComBoxBaseAddress.SelectedItem = client.ClientPreferredAddress;
        }


        private int CalculationTotalPrice()
        {
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                List<Basket> baskets;
                baskets = (from b in db.Basket where SystemContext.Client.ClientID == b.ClientID select b).ToList<Basket>();
                int total = 0;
                foreach (var basket in baskets)
                {
                    SystemContext.Product = (from p in db.Product where basket.ProductID == p.ProductID select p).FirstOrDefault();
                    total += basket.BasketProductCount * SystemContext.Product.ProductPrice;
                }

                return total;
            }
        }

        private void LoadAddress()
        {
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                List<Shop> shops;
                shops = (from s in db.Shop select s).ToList<Shop>();
                int i = 0;
                foreach (var address in shops)
                {
                    ComBoxBaseAddress.Items.Add(address.ShopAddress);
                    i++;
                }
            }
        }
        
        private void CreateNewOrder(string ProductID, string ShopAddress, int OrderProcuctCount, string OrderPaymentMethod)
        {
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                Orders orders = new Orders();
                SystemContext.Client = (from c in db.Client where c.UserID == SystemContext.User.UserID select c).FirstOrDefault();
                SystemContext.Shop = (from s in db.Shop where s.ShopAddress.ToString() == ShopAddress select s).FirstOrDefault();
                SystemContext.Product = (from p in db.Product where p.ProductID.ToString() == ProductID select p).FirstOrDefault();
                int totalPrice = SystemContext.Product.ProductPrice * OrderProcuctCount;

                orders.ClientID = SystemContext.Client.ClientID;
                orders.ProductID = SystemContext.Product.ProductID;
                orders.ShopID = SystemContext.Shop.ShopID;
                orders.OrderProductCount = OrderProcuctCount;
                orders.OrderTotalPrice = totalPrice;
                orders.OrderData = System.DateTimeOffset.UtcNow.Date;
                orders.OrderPaymentMethod = OrderPaymentMethod;
                if(OrderPaymentMethod == "Наличными")
                    orders.OrderStatus = "Не оплачен";
                else
                    orders.OrderStatus = "Оплачен";
                db.Orders.Add(orders);
                db.SaveChanges();

                DeleteProductFromBasket(ProductID);
            }
        }

        private void DeleteProductFromBasket(string ProductID)
        {
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                var client = (from c in db.Client where c.UserID == SystemContext.User.UserID select c).FirstOrDefault();
                var product = (from p in db.Product where p.ProductID.ToString() == ProductID select p).FirstOrDefault();
                var productShopCart = (from sc in db.Basket where sc.ClientID == client.ClientID & sc.ProductID == product.ProductID select sc).FirstOrDefault<Basket>();
                db.Entry(productShopCart).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            MessageBox.Show("Заказ успешно оформлен");
            OrderListWindow olw = new OrderListWindow();
            this.Close();
            olw.ShowDialog();
        }

        private void CancelButton_Click(object sender, MouseButtonEventArgs e)
        {
            SystemContext.typeWindow = "Каталог";
            ClientMainWindow cmw = new ClientMainWindow();
            this.Close();
            cmw.ShowDialog();
        }

        private void ConfirmOrderButton_Click(object sender, MouseButtonEventArgs e)
        {
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                SystemContext.Client = (from c in db.Client where c.UserID == SystemContext.User.UserID select c).FirstOrDefault();
                Client client = SystemContext.Client;
                List<Basket> baskets;
                baskets = (from b in db.Basket where SystemContext.Client.ClientID == b.ClientID select b).ToList<Basket>();
                if (baskets.Count == 0)
                {
                    MessageBox.Show("Корзина пуста");
                    OrderListWindow orlw = new OrderListWindow();
                    this.Close();
                    orlw.ShowDialog();
                    return;
                }

                if (txtFIO.Text != "")
                {
                    if (txtNumber.Text != "")
                    {
                        if (ComBoxBaseAddress.SelectedItem != null)
                        {

                            if (SystemContext.Client.ClientFIO == txtFIO.Text && SystemContext.Client.ClientNumber == txtNumber.Text && SystemContext.Client.ClientPreferredAddress == ComBoxBaseAddress.Text)
                            {
                                client.ClientPreferredAddress = ComBoxBaseAddress.Text;
                                if (CardRadioBtn.IsChecked == true)
                                {
                                    foreach (var basket in baskets)
                                    {
                                        CreateNewOrder(basket.ProductID.ToString(), client.ClientPreferredAddress, basket.BasketProductCount, "Карта");
                                    }
                                }
                                else if (MoneyRadioBtn.IsChecked == true)
                                {
                                    foreach (var basket in baskets)
                                    {
                                        CreateNewOrder(basket.ProductID.ToString(), client.ClientPreferredAddress, basket.BasketProductCount, "Наличными");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Выберите метод оплаты");
                                }
                            }
                            else
                            {
                                client.ClientFIO = txtFIO.Text;
                                client.UserID = SystemContext.User.UserID;
                                client.ClientNumber = txtNumber.Text;
                                client.ClientPreferredAddress = ComBoxBaseAddress.Text;
                                db.Entry(client).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();

                                if (CardRadioBtn.IsChecked == true)
                                {
                                    foreach (var basket in baskets)
                                    {
                                        CreateNewOrder(basket.ProductID.ToString(), client.ClientPreferredAddress, basket.BasketProductCount, "Карта");
                                    }
                                }
                                else if (MoneyRadioBtn.IsChecked == true)
                                {
                                    foreach (var basket in baskets)
                                    {
                                        CreateNewOrder(basket.ProductID.ToString(), client.ClientPreferredAddress, basket.BasketProductCount, "Наличными");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Выберите метод оплаты");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Выберите адрес!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Введите номер");
                    }
                }
                else
                {
                    MessageBox.Show("Введите ФИО");
                }
            }
        }

        private void ButtonCatalog_Click(object sender, RoutedEventArgs e)
        {
            SystemContext.typeWindow = "Каталог";
            ClientMainWindow cmw = new ClientMainWindow();
            this.Close();
            cmw.ShowDialog();
        }

        private void ButtonCart_Click(object sender, RoutedEventArgs e)
        {
            SystemContext.typeWindow = "Корзина";
            ClientMainWindow cmw = new ClientMainWindow();
            this.Close();
            cmw.ShowDialog();
        }

        private void ButtonMyOrders_Click(object sender, RoutedEventArgs e)
        {
            OrderListWindow olw = new OrderListWindow();
            this.Close();
            olw.ShowDialog();
        }

        private void ButtonMyProfile_Click(object sender, RoutedEventArgs e)
        {
            UserProfileWindow upw = new UserProfileWindow();
            this.Close();
            upw.ShowDialog();
        }

        private void MoneyRadioBtn_Click(object sender, RoutedEventArgs e)
        {
            txtNumberOfCard.IsEnabled = false;
            txtMonthAndYear.IsEnabled = false;
            txtCVVCode.IsEnabled = false;
        }

        private void CardRadioBtn_Click(object sender, RoutedEventArgs e)
        {
            txtNumberOfCard.IsEnabled = true;
            txtMonthAndYear.IsEnabled = true;
            txtCVVCode.IsEnabled = true;
        }
    }
}
