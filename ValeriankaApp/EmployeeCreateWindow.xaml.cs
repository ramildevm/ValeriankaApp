using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для EmployeeCreateWindow.xaml
    /// </summary>
    public partial class EmployeeCreateWindow : Window
    {
        public Boolean isCreate { get; set; } = false;
        List<TextBox> quantityTxtList = new List<TextBox>();
        List<ShopAddressLink> addressList = new List<ShopAddressLink>();
        byte[] imageBytes = null;

        public EmployeeCreateWindow()
        {
            InitializeComponent();
            LoadContent();
        }

        private void LoadContent()
        {
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                //var product = SystemContext.Product;
                var product = (from p in db.Product where p.ProductID == 1 select p).FirstOrDefault();
                if (product == null)
                    return;

                List<Shop> shopList = (from s in db.Shop select s).ToList();

                foreach (var shop in shopList)
                {
                    string address = shop.ShopAddress;
                    var shopLabel = new TextBlock() { Text = $"● {address}" };
                    adressListPanel.Children.Add(shopLabel);

                    var border = new Border() { BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(5) };
                    var quantityTxt = new TextBox() { Background = Brushes.Transparent, BorderThickness = new Thickness(0) };
                    if (!isCreate)
                    {
                        quantityTxt.Text = (from sc in db.ShopAddressLink
                                            where sc.ProductID == product.ProductID & sc.ShopID == shop.ShopID
                                            select sc).FirstOrDefault().ShopAddressLinkAvailability.ToString();
                    }
                    quantityTxt.Tag = shop;
                    quantityTxtList.Add(quantityTxt);
                    border.Child = quantityTxt;
                    adressListPanel.Children.Add(border);
                }

                if (!isCreate)
                {
                    createButton.Content = "Сохранить";
                    nameTxt.Text = product.ProductName;
                    manufacturerTxt.Text = product.ProductManufacturer;
                    typeTxt.Text = product.ProductType;
                    purposeTxt.Text = product.ProductPurpose;
                    quantityTxt.Text = product.ProductCount.ToString();
                    priceTxt.Text = product.ProductPrice.ToString();
                    descriptionTxt.Document.Blocks.Clear();
                    descriptionTxt.Document.Blocks.Add(new Paragraph(new Run(product.ProductDescription)));
                    try
                    {
                        imageHolder.Source = ByteArrayToImage(product.ProductImage);
                    }
                    catch { }
                }

            }

        }

        private void ButtonCatalog_Click(object sender, RoutedEventArgs e)
        {
            var emp = new EmployeeMainWindow();
            this.Close();
            emp.ShowDialog();
        }
        private void ButtonOrders_Click(object sender, RoutedEventArgs e)
        {

        }
        public BitmapSource ByteArrayToImage(byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            {
                return BitmapFrame.Create(stream,
                    BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }
        private void ButtonViewImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Image";
            dialog.DefaultExt = ".jpg";
            dialog.Filter = "Image Files(*.BMP; *.JPG; *.GIF; *.PNG)| *.BMP; *.JPG; *.GIF; *.PNG | All files(*.*) | *.*";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string filename = dialog.FileName;
                imageBytes = File.ReadAllBytes(filename);
                imageHolder.Source = ByteArrayToImage(imageBytes);
                imgBorder.Visibility = Visibility.Visible;
            }
        }
        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            CheckData();
        }

        private string CheckData()
        {
            string name = nameTxt.Text,
                manufacturer = manufacturerTxt.Text,
                type = typeTxt.Text,
                purpose = purposeTxt.Text,
                description = new TextRange(descriptionTxt.Document.ContentStart, descriptionTxt.Document.ContentEnd).Text;
            try
            {
                int quantity = Convert.ToInt32(quantityTxt.Text),
                    price = Convert.ToInt32(priceTxt.Text);
            }
            catch { return "Введены некорректные данные"; }

            foreach (var qTxt in quantityTxtList)
            {
                if (qTxt.Text == "")
                {
                    try
                    {
                        
                    }
                    catch
                    {
                        return "Введены некорректные данные";
                    }
                }
                else
                {

                }
            }
            if ( name == "" || manufacturer == "" || type == "" || purpose == "" || description == "")
                return "Не все поля заполнены";

            return "Its ok";
        }
    }
}
