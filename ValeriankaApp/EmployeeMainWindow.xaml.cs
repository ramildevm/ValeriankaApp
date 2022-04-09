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
    /// Логика взаимодействия для EmployeeMainWindow.xaml
    /// </summary>
    public partial class EmployeeMainWindow : Window
    {
        List<TextBox> quantityList = new List<TextBox>();
        public EmployeeMainWindow()
        {
            InitializeComponent();
            LoadContent();
            try
            {
                btnProfileText.Text = SystemContext.User.UserLogin;
                btnProfile.Click += ButtonMyProfile_Click;
            }
            catch { }
        }

        void LoadContent(string searchText = "")
        {
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                List<Product> products;
                try
                {
                    if (searchText == "")
                    {
                            products = (from p in db.Product select p).ToList<Product>();
                    }
                    else
                    {
                        IEnumerable<Product> productsSet;
                        productsSet = (from p in db.Product select p);
                        products = productsSet.Where(product => product.ProductName.Contains($"{searchText}")).ToList<Product>();
                    }
                    int i = 0;
                    foreach (var product in products)
                    {
                        AddProductPanel(product, product.ProductName, product.ProductPurpose, product.ProductCount, product.ProductPrice);
                        i++;
                    }
                }
                catch { }
            }
        }
        public BitmapSource ByteArrayToImage(byte[] buffer)
        {
            using (var stream = new System.IO.MemoryStream(buffer))
            {
                return BitmapFrame.Create(stream,
                    BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }
        void AddProductPanel(Product product, string name, string purpose, int quantity, int price)
        {
            var borderPanel = new Border() { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(2), Style = (Style)contentPanel.Resources["contentBorderStyle"] };
            StackPanel sp = new StackPanel() { };
            sp.Tag = product;
            sp.MouseLeftButtonDown += Sp_MouseLeftButtonDown;
            Image img = new Image() { };
            if (product.ProductImage != null)
                img.Source = ByteArrayToImage(product.ProductImage);
            TextBlock nameUp = new TextBlock() { Margin = new Thickness(17, -28, 0, 0), FontWeight = FontWeights.Bold, Foreground = (Brush)(new BrushConverter().ConvertFrom("#A500F3")), FontSize = 16 };
            StackPanel.SetZIndex(nameUp, 1);
            TextBlock backNameUp = new TextBlock() { Foreground = Brushes.White, Margin = new Thickness(0, -25, 0, 0), FontSize = 14, Background = Brushes.White };
            backNameUp.Effect = new System.Windows.Media.Effects.BlurEffect() { Radius = 10, KernelType = System.Windows.Media.Effects.KernelType.Gaussian };
            TextBlock purposeTxt = new TextBlock() { Text = "Назначение: " };
            TextBlock availabilityTxt = new TextBlock() { Text = "Количество: ", Margin = new Thickness(12, 0, 3, 0) };

            TextBlock priceTxt = new TextBlock() { Text = "Цена: " };
            priceTxt.Inlines.Add(new TextBlock() { Text = $" {price} руб.", Foreground = (Brush)(new BrushConverter().ConvertFrom("#A500F3")), Margin = new Thickness(0) });

            //Bottom
            StackPanel bottomSp = new StackPanel() { Orientation = Orientation.Horizontal, MinHeight=40, Margin = new Thickness(12, 0, 0, 0) };
            Button changeBtn = new Button() {Cursor=Cursors.Hand, Width = 80, Height = 30, Content = "Изменить", Foreground = Brushes.White, Margin = new Thickness(5, 0, 0, 6) };
            changeBtn.Style = (Style)contentPanel.Resources["RoundedButtonStyle"];
            changeBtn.Click += ButtonChange_Click;
            changeBtn.Tag = product;
            Button delBtn = new Button() { Cursor = Cursors.Hand, Width = 80, Height = 30, Content = "Удалить", Foreground = Brushes.White, Margin = new Thickness(24, 0, 0, 6) };
            delBtn.Tag = product;
            delBtn.Click += ButtonDelete_Click;
            delBtn.Style = (Style)contentPanel.Resources["RoundedButtonStyle"];
            bottomSp.Children.Add(changeBtn);
            bottomSp.Children.Add(delBtn);
            var borderPanel2 = new Border() { CornerRadius = new CornerRadius(0, 0, 10, 10), Background = (Brush)(new BrushConverter().ConvertFrom("#f6f6f6")) };

            //добавление данных
            nameUp.Text += name;
            backNameUp.Text += name;
            purposeTxt.Text += purpose;
            availabilityTxt.Text += quantity.ToString();
            if (product.ProductType == "раствор")
                availabilityTxt.Text += $" мл.";
            else if (product.ProductType == "таблетки")
                availabilityTxt.Text += $" таблеток.";

            //Добавление элементов в контейнер
            var sp2 = new StackPanel() { MinHeight = 73, Background = (Brush)(new BrushConverter().ConvertFrom("#f6f6f6")) };
            sp.Children.Add(img);
            sp.Children.Add(nameUp);
            sp.Children.Add(backNameUp);
            sp2.Children.Add(purposeTxt);
            sp2.Children.Add(availabilityTxt);
            sp2.Children.Add(priceTxt);
            borderPanel2.Child = bottomSp;
            borderPanel.Child = sp;
            sp.Children.Add(sp2);
            sp.Children.Add(borderPanel2);
            contentPanel.Children.Add(borderPanel);
        }

        private void ButtonChange_Click(object sender, RoutedEventArgs e)
        {
            SystemContext.Product = (sender as Button).Tag as Product;
            var ecw = new EmployeeCreateWindow(false);
            this.Close();
            ecw.ShowDialog();
        }

        private void Sp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void ButtonMyProfile_Click(object sender, RoutedEventArgs e)
        {
            EmployeeProfileWindow epw = new EmployeeProfileWindow();
            this.Close();
            epw.ShowDialog();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Удалить товар?", "Сообщение",MessageBoxButton.YesNo,MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                using (var db = new Pharmacy_ValeriankaEntities())
                {
                    var product = (Product)(sender as Button).Tag;
                    db.Entry(product).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                contentPanel.Children.Clear();
                LoadContent();
            }
        }

        private void QuantityTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (Convert.ToInt32((sender as TextBox).Text) < 1)
                    (sender as TextBox).Text = "1";
            }
            catch { if ((sender as TextBox).Text != "") (sender as TextBox).Text = "1"; }
        }
        private void ButtonCatalog_Click(object sender, RoutedEventArgs e)
        {
            contentPanel.Children.Clear();
            LoadContent();
        }
        private void ButtonOrders_Click(object sender, RoutedEventArgs e)
        {
            EmployeeOrderListWindow eolw = new EmployeeOrderListWindow();
            this.Close();
            eolw.ShowDialog();
        }
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            contentPanel.Children.Clear();
            LoadContent(searchTxt.Text);
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            SystemContext.Product = null;
            var ecw = new EmployeeCreateWindow(true);
            this.Close();
            ecw.Show();
        }
    }
}
