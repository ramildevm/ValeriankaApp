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
        void AddProductPanel(Product product,string name, string purpose, int quantity, int price)
        {
            var borderPanel = new Border() { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(2), Style = (Style)contentPanel.Resources["contentBorderStyle"] };
            StackPanel sp = new StackPanel() { };
            Image img = new Image() { };
            TextBlock nameUp = new TextBlock() { Margin = new Thickness(17, -28, 0, 0), Foreground = (Brush)(new BrushConverter().ConvertFrom("#A500F3")), FontSize = 16 };
            TextBlock purposeTxt = new TextBlock() { Text = "Назначение: " };
            TextBlock availabilityTxt = new TextBlock() { Text = "Наличие:", Margin = new Thickness(12, 0, 3, 0) };
            TextBlock priceTxt = new TextBlock() { Text = "Цена: " };
            priceTxt.Inlines.Add(new TextBlock() { Text = $" {price} руб.", Foreground = (Brush)(new BrushConverter().ConvertFrom("#A500F3")), Margin = new Thickness(0) });

            //Bottom
            StackPanel bottomSp = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(12, 5, 0, 0) };
            Button changeBtn = new Button() { Width = 80, Height = 30, Content = "Изменить", Foreground = Brushes.White, Margin = new Thickness(12, 0, 0, 6) };
            changeBtn.Style = (Style)contentPanel.Resources["RoundedButtonStyle"];
            Button delBtn = new Button() { Width = 80, Height = 30, Content = "Удалить", Foreground = Brushes.White, Margin = new Thickness(12, 0, 0, 6) };
            delBtn.Tag = product;
            delBtn.Click += ButtonDelete_Click;
            delBtn.Style = (Style)contentPanel.Resources["RoundedButtonStyle"];
            bottomSp.Children.Add(changeBtn);
            bottomSp.Children.Add(delBtn);

            //добавление данных
            nameUp.Text += name;
            purposeTxt.Text += purpose;
            availabilityTxt.Text += quantity.ToString();
            //Добавление элементов в контейнер
            var borderPanel2 = new Border() { CornerRadius = new CornerRadius(0, 0, 10, 10), Background = (Brush)(new BrushConverter().ConvertFrom("#f6f6f6")) };
            var sp2 = new StackPanel() { };
            sp.Children.Add(img);
            sp.Children.Add(nameUp);
            sp2.Children.Add(purposeTxt);
            sp2.Children.Add(availabilityTxt);
            sp2.Children.Add(priceTxt);
            sp2.Children.Add(bottomSp);
            borderPanel2.Child = sp2;
            borderPanel.Child = sp;
            sp.Children.Add(borderPanel2);
            contentPanel.Children.Add(borderPanel);
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                var product = (Product)(sender as Button).Tag;
                db.Entry(product).State = System.Data.Entity.EntityState.Deleted;
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

        }
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            contentPanel.Children.Clear();
            LoadContent(searchTxt.Text);
        }
    }
}
