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
    /// Логика взаимодействия для ProductInfoWindow.xaml
    /// </summary>
    public partial class ProductInfoWindow : Window
    {
        List<TextBox> quantityTxtList = new List<TextBox>();
        public ProductInfoWindow()
        {
            InitializeComponent();
            LoadContetnt();
            try
            {
                btnProfile.Content = SystemContext.User.UserLogin;
                btnProfile.Click += ButtonMyProfile_Click;
            }
            catch { }
        }

        private void LoadContetnt()
        {
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                var product = SystemContext.Product;
                List<Shop> shopList = (from s in db.Shop select s).ToList();
                Boolean isNotAvaible = true;
                foreach (var shop in shopList)
                {
                    var productShop = (from sc in db.ShopAddressLink
                                       where sc.ProductID == product.ProductID & sc.ShopID == shop.ShopID
                                       select sc).FirstOrDefault();
                    if (productShop != null)
                    {
                        string address = shop.ShopAddress;
                        var shopLabel = new TextBlock() { Margin = new Thickness(0, 0, 0, 5), Text = $"● {address}:   {productShop.ShopAddressLinkAvailability.ToString()}" };
                        adressListPanel.Children.Add(shopLabel);
                        isNotAvaible = false;
                    }
                }
                if (isNotAvaible)
                {
                    var shopLabel = new TextBlock() { Margin = new Thickness(0, 0, 0, 5), Text = $"● Товара нет в наличии!" };
                    adressListPanel.Children.Add(shopLabel);
                }
                nameTxt.Text = product.ProductName; 
                manufacturerTxt.Inlines.Add(new TextBlock() { Text = $" {product.ProductManufacturer}", Foreground = Brushes.Black, Margin = new Thickness(0), TextWrapping=TextWrapping.Wrap });
                typeTxt.Inlines.Add(new TextBlock() { Text = $" {product.ProductType}", Foreground = Brushes.Black, Margin = new Thickness(0), TextWrapping = TextWrapping.WrapWithOverflow });
                purposeTxt.Inlines.Add(new TextBlock() { Text = $" {product.ProductPurpose}", Foreground = Brushes.Black, Margin = new Thickness(0), TextWrapping = TextWrapping.WrapWithOverflow });
                quantityTxt.Inlines.Add(new TextBlock() { Text = $" {product.ProductCount.ToString()}", Foreground = Brushes.Black, Margin = new Thickness(0), TextWrapping = TextWrapping.WrapWithOverflow });
                if (product.ProductType == "раствор")
                    quantityTxt.Inlines.Add(new TextBlock() { Text = $" мл.", Foreground = Brushes.Black, Margin = new Thickness(0), TextWrapping = TextWrapping.WrapWithOverflow });
                else if (product.ProductType == "таблетки")
                    quantityTxt.Inlines.Add(new TextBlock() { Text = $" таблеток.", Foreground = Brushes.Black, Margin = new Thickness(0), TextWrapping = TextWrapping.WrapWithOverflow });
                priceTxt.Inlines.Add(new TextBlock() { Text = $" {product.ProductPrice.ToString()} руб.", Foreground = Brushes.Black, Margin = new Thickness(0), TextWrapping = TextWrapping.WrapWithOverflow });
                descriptionTxt.Document.Blocks.Clear();
                descriptionTxt.Document.Blocks.Add(new Paragraph(new Run(product.ProductDescription)));
                try
                {
                    imageHolder.Source = ByteArrayToImage(product.ProductImage);
                }
                catch { }
            }
        }
        private void QuantityTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (Convert.ToInt32((sender as TextBox).Text) < 1)
                    (sender as TextBox).Text = "1";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if ((sender as TextBox).Text != "")
                    (sender as TextBox).Text = "1";
            }
        }
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                var client = SystemContext.Client;
                var product = SystemContext.Product;
                var productShopCart = (from sc in db.Basket where sc.ClientID == client.ClientID & sc.ProductID == product.ProductID select sc).FirstOrDefault<Basket>();
                if (productShopCart == null)
                    db.Basket.Add(new Basket() { ClientID = client.ClientID, ProductID = product.ProductID, BasketProductCount = Convert.ToInt32(quantityBasketTxt.Text) });
                else
                {
                    productShopCart.BasketProductCount += Convert.ToInt32(quantityBasketTxt.Text);
                    db.Entry(productShopCart).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
            }
        }
        private void ButtonIncrease_Click(object sender, RoutedEventArgs e)
        {
            quantityBasketTxt.Text = (Convert.ToInt32(quantityBasketTxt.Text) + 1).ToString();
        }

        private void ButtonReduce_Click(object sender, RoutedEventArgs e)
        {
            int num = Convert.ToInt32(quantityBasketTxt.Text);
            quantityBasketTxt.Text = (num - 1).ToString();
        }
        public BitmapSource ByteArrayToImage(byte[] buffer)
        {
            using (var stream = new System.IO.MemoryStream(buffer))
            {
                return BitmapFrame.Create(stream,
                    BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }

        private void ButtonCatalog_Click(object sender, RoutedEventArgs e)
        {
            SystemContext.typeWindow = "Каталог";
            var cmw = new ClientMainWindow();
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

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            if (SystemContext.typeWindow == "Каталог")
                ButtonCatalog_Click(this, new RoutedEventArgs());
            else
                ButtonCart_Click(this, new RoutedEventArgs());
        }

        private void ButtonCart_Click(object sender, RoutedEventArgs e)
        {
            SystemContext.typeWindow = "Корзина";
            var cmw = new ClientMainWindow();
            this.Close();
            cmw.ShowDialog();
        }
    }
}
