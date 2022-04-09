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
        public Boolean isCreate { get; set; }
        List<TextBox> quantityTxtList = new List<TextBox>();
        List<ShopAddressLink> addressList = new List<ShopAddressLink>();
        byte[] imageBytes = null;

        public EmployeeCreateWindow(Boolean flag)
        {
            InitializeComponent();
            isCreate = flag;
            LoadContent();
            try
            {
                btnProfileText.Text = SystemContext.User.UserLogin;
                btnProfile.Click += ButtonMyProfile_Click;
            }
            catch { }
        }

        private void LoadContent()
        {
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                var product = SystemContext.Product;
                //SystemContext.Product = (from p in db.Product where p.ProductID == 1 select p).FirstOrDefault();
                //var product = SystemContext.Product;


                List<Shop> shopList = (from s in db.Shop select s).ToList();

                foreach (var shop in shopList)
                {
                    string address = shop.ShopAddress;
                    var shopLabel = new TextBlock() { Margin = new Thickness(0, 5, 0, 0), Text = $"● {address}" };
                    adressListPanel.Children.Add(shopLabel);

                    var border = new Border() { BorderBrush = Brushes.Gray, Height = 25, BorderThickness = new Thickness(1), MinWidth = 197, Background = Brushes.White, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 5, 20, 0), CornerRadius = new CornerRadius(5) };
                    var quantityTxt = new TextBox() { Background = Brushes.Transparent, FontSize = 14, BorderThickness = new Thickness(0) };
                    if (!isCreate)
                    {
                        var productShop = (from sc in db.ShopAddressLink
                                           where sc.ProductID == product.ProductID & sc.ShopID == shop.ShopID
                                           select sc).FirstOrDefault();
                        if (productShop != null)
                            quantityTxt.Text = productShop.ShopAddressLinkAvailability.ToString();

                    }
                    quantityTxt.Tag = shop;
                    quantityTxtList.Add(quantityTxt);
                    border.Child = quantityTxt;
                    adressTxtPanel.Children.Add(border);
                }
                if (product == null)
                    return;

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

        private void ButtonMyProfile_Click(object sender, RoutedEventArgs e)
        {
            EmployeeProfileWindow epw = new EmployeeProfileWindow();
            this.Close();
            epw.ShowDialog();
        }

        private void ButtonCatalog_Click(object sender, RoutedEventArgs e)
        {
            var emp = new EmployeeMainWindow();
            this.Close();
            emp.ShowDialog();
        }

        private void ButtonOrders_Click(object sender, RoutedEventArgs e)
        {
            EmployeeOrderListWindow eolw = new EmployeeOrderListWindow();
            this.Close();
            eolw.ShowDialog();
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
            string result = CheckData();
            MessageBox.Show(result, "Результат", MessageBoxButton.OK, MessageBoxImage.Warning);
            if (result == "Its ok")
                ButtonCatalog_Click(this, new RoutedEventArgs());

        }

        private string CheckData()
        {
            string name = nameTxt.Text,
                manufacturer = manufacturerTxt.Text,
                type = typeTxt.Text,
                purpose = purposeTxt.Text,
                description = new TextRange(descriptionTxt.Document.ContentStart, descriptionTxt.Document.ContentEnd).Text;
            int quantity, price;
            try
            {
                quantity = Convert.ToInt32(quantityTxt.Text);
                price = Convert.ToInt32(priceTxt.Text);
                if (quantity <= 0 || price <= 0)
                    throw new Exception();
            }
            catch { return "Введены некорректные данные"; }
            List<ShopAddressLink> productShopList = new List<ShopAddressLink>();
            foreach (var qTxt in quantityTxtList)
            {
                if (qTxt.Text != "")
                {
                    try
                    {
                        if (Convert.ToInt32(qTxt.Text) < 0)
                            throw new Exception();
                        else if (Convert.ToInt32(qTxt.Text) == 0)
                            continue;
                        productShopList.Add(new ShopAddressLink() { ProductID = SystemContext.Product.ProductID, ShopID = (qTxt.Tag as Shop).ShopID, ShopAddressLinkAvailability = Convert.ToInt32(qTxt.Text) });
                    }
                    catch
                    {
                        return "Введены некорректные данные";
                    }
                }
            }
            if (name == "" || manufacturer == "" || type == "" || purpose == "" || description == "")
                return "Не все поля заполнены";
            string result;
            if (isCreate)
            {
                using (var db = new Pharmacy_ValeriankaEntities())
                {
                    var product = db.Product.Add(new Product() { ProductName = name, ProductType = type, ProductDescription = description, ProductCount = quantity, ProductManufacturer = manufacturer, ProductPurpose = purpose, ProductPrice = price, ProductImage = imageBytes });
                    db.SaveChanges();
                }
                result = "Товар добавен";
            }
            else
            {
                using (var db = new Pharmacy_ValeriankaEntities())
                {
                    var product = SystemContext.Product;
                    product.ProductName = name;
                    product.ProductType = type;
                    product.ProductCount = quantity;
                    product.ProductDescription = description;
                    product.ProductManufacturer = manufacturer;
                    product.ProductPurpose = purpose;
                    product.ProductPrice = price;
                    product.ProductImage = imageBytes;

                    db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                result = "Товар изменен";
            }
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                foreach (var shop in productShopList)
                {
                    //тут ошибка надо чтобы оно не обновляло а создавало если такого нет
                    var productShop = (from sc in db.ShopAddressLink
                                       where sc.ProductID == shop.ProductID & sc.ShopID == shop.ShopID
                                       select sc).FirstOrDefault();
                    if (productShop != null)
                    {
                        productShop.ShopAddressLinkAvailability = shop.ShopAddressLinkAvailability;
                        db.Entry(productShop).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        db.ShopAddressLink.Add(shop);
                    }
                }
                db.SaveChanges();
            }
            return result;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            SystemContext.Product = null;
            ButtonCatalog_Click(this, new RoutedEventArgs());
        }
    }
}
