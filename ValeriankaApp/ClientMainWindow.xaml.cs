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
    /// Логика взаимодействия для ClientMainWindow.xaml
    /// </summary>
    public partial class ClientMainWindow : Window
    {
        List<TextBox> quantityList = new List<TextBox>();
        public ClientMainWindow()
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
                        IEnumerable<Product> productsSet = (from p in db.Product select p);
                        products = productsSet.Where(product => product.ProductName.Contains($"{searchText}")).ToList<Product>();
                    }
                    int i = 0;
                    foreach (var product in products)
                    {
                        AddProductPanel(i, product.ProductName, product.ProductPurpose, product.ProductCount, product.ProductPrice);
                        i++;
                    }
                }
                catch { }
            }
        }
        void AddProductPanel(int i, string name, string purpose, int quantity, int price)
        {
            var borderPanel = new Border() { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(2), Style = (Style)contentPanel.Resources["contentBorderStyle"] };
            StackPanel sp = new StackPanel() { };
            Image img = new Image() { };
            TextBlock nameUp = new TextBlock() { Margin = new Thickness(17, -28, 0, 0), Foreground = (Brush)(new BrushConverter().ConvertFrom("#A500F3")), FontSize = 16 };
            TextBlock purposeTxt = new TextBlock() { Text = "Назначение: " };
            TextBlock availabilityTxt = new TextBlock() { Text = "Наличие: ", Margin = new Thickness(12, 0, 3, 0) };
            TextBlock priceTxt = new TextBlock() { Text = "Цена: " };
            priceTxt.Inlines.Add(new TextBlock() { Text = $" {price} руб.", Foreground = (Brush)(new BrushConverter().ConvertFrom("#A500F3")), Margin = new Thickness(0) });

            //Bottom
            StackPanel bottomSp = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(12, 0, 0, 0) };
            Button reduceBtn = new Button() { Tag = i, Width = 30, Height = 40, Background = Brushes.Transparent, Content = "-", FontWeight = FontWeights.Bold, FontSize = 20, BorderThickness = new Thickness(0) };
            reduceBtn.Click += ButtonReduce_Click;
            TextBox quantityTxtBox = new TextBox() { Text = "1", Width = 40, Height = 25, Background = Brushes.Transparent, FontWeight = FontWeights.Bold, FontSize = 14, TextAlignment = TextAlignment.Center };
            quantityTxtBox.TextChanged += QuantityTxtBox_TextChanged;
            quantityList.Add(quantityTxtBox);
            Button increaseBtn = new Button() { Tag = i, Width = 30, Height = 40, Background = Brushes.Transparent, Content = "+", FontWeight = FontWeights.Bold, FontSize = 20, BorderThickness = new Thickness(0) };
            increaseBtn.Click += ButtonIncrease_Click;
            Button addBtn = new Button() { Width = 80, Height = 30, Content = "Добавить", Foreground = Brushes.White, Margin = new Thickness(12, 0, 0, 0) };
            addBtn.Style = (Style)contentPanel.Resources["RoundedButtonStyle"];
            bottomSp.Children.Add(reduceBtn);
            bottomSp.Children.Add(quantityTxtBox);
            bottomSp.Children.Add(increaseBtn);
            bottomSp.Children.Add(addBtn);

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

        private void QuantityTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (Convert.ToInt32((sender as TextBox).Text) < 1)
                    (sender as TextBox).Text = "1";
            }
            catch { if ((sender as TextBox).Text != "") (sender as TextBox).Text = "1"; }
        }

        void LoadProduct()
        {
            contentPanel.Children.Clear();
            StackPanel sp = new StackPanel() { Style = (Style)contentPanel.Resources["productSpStyle"] };


            contentPanel.Children.Add(sp);

        }

        private void ButtonIncrease_Click(object sender, RoutedEventArgs e)
        {
            int tag = Convert.ToInt32(((Button)sender).Tag);
            quantityList[tag].Text = (Convert.ToInt32(quantityList[tag].Text) + 1).ToString();
        }

        private void ButtonReduce_Click(object sender, RoutedEventArgs e)
        {
            int tag = Convert.ToInt32(((Button)sender).Tag);
            int num = Convert.ToInt32(quantityList[tag].Text);
            if (num > 1)
                quantityList[tag].Text = (num - 1).ToString();
        }
        private void ButtonCatalog_Click(object sender, RoutedEventArgs e)
        {
            contentPanel.Children.Clear();
            LoadContent();
        }
        private void ButtonMyOrders_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ButtonCart_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            contentPanel.Children.Clear();
            LoadContent(searchTxt.Text);
        }
    }
}
