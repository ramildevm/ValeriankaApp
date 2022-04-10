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
    /// Логика взаимодействия для UserProfileWindow.xaml
    /// </summary>
    public partial class UserProfileWindow : Window
    {
        public UserProfileWindow()
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
        }

        private void LoadDefault()
        {
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                SystemContext.Client = (from c in db.Client where c.UserID == SystemContext.User.UserID select c).FirstOrDefault();
            }

            var client = SystemContext.Client;
            var user = SystemContext.User;

            txtLogin.Text = user.UserLogin;
            txtEmail.Text = user.UserEmail;
            txtPassword.Password = user.UserPassword;
            txtPasswordConfirm.Password = user.UserPassword;
            txtFIO.Text = client.ClientFIO;
            txtNumber.Text = client.ClientNumber;
            ComBoxBaseAddress.SelectedItem = client.ClientPreferredAddress;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                var user = db.Users.FirstOrDefault(x => x.UserLogin == SystemContext.User.UserLogin);
                if (user == null)
                {
                    return;
                }
                db.Users.Remove(user);
                db.SaveChanges();
            }
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.ShowDialog();
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

        private void OutOfSystem_Click(object sender, MouseButtonEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.ShowDialog();
        }

        private void SaveChanges_Click(object sender, MouseButtonEventArgs e)
        {
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                //SystemContext.Client = (from c in db.Client where c.UserID == SystemContext.User.UserID select c).FirstOrDefault();
                var client = (from c in db.Client where c.UserID == SystemContext.User.UserID select c).FirstOrDefault();
                var user = (from u in db.Users where u.UserID == SystemContext.User.UserID select u).FirstOrDefault();

                if (txtLogin.Text != "" || txtEmail.Text != "" || txtPassword.Password != "")
                {
                    if (txtLogin.Text != "" && txtLogin.Text.Length >= 5 && txtLogin.Text != SystemContext.User.UserLogin)
                    {
                        user.UserLogin = txtLogin.Text;
                    }
                    if (txtEmail.Text != "" && txtEmail.Text != SystemContext.User.UserEmail)
                    {
                        user.UserEmail = txtEmail.Text;
                    }
                    if (txtPassword.Password == txtPasswordConfirm.Password && txtPassword.Password != "" && txtPassword.Password.Length >= 8 && SystemContext.User.UserPassword != txtPassword.Password)
                    {
                        user.UserPassword = txtPassword.Password;
                    }
                    if (SystemContext.User.UserLogin == txtLogin.Text && SystemContext.User.UserEmail == txtEmail.Text && SystemContext.User.UserPassword == txtPassword.Password)
                    {

                    }
                    else
                    {
                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        SystemContext.User = user;
                    }
                }

                if (txtFIO.Text != "" || txtNumber.Text != "" || ComBoxBaseAddress.SelectedItem != null)
                {
                    if (txtFIO.Text != "" && txtFIO.Text != SystemContext.Client.ClientFIO)
                    {
                        client.ClientFIO = txtFIO.Text;
                    }
                    if (txtNumber.Text != "" && txtNumber.Text != SystemContext.Client.ClientNumber)
                    {
                        client.ClientNumber = txtNumber.Text;
                    }
                    if (ComBoxBaseAddress.SelectedItem != null && ComBoxBaseAddress.Text != SystemContext.Client.ClientPreferredAddress)
                    {
                        client.ClientPreferredAddress = ComBoxBaseAddress.Text;
                    }
                    if (SystemContext.Client.ClientFIO == txtFIO.Text && SystemContext.Client.ClientNumber == txtNumber.Text && SystemContext.Client.ClientPreferredAddress == ComBoxBaseAddress.Text)
                    {

                    }
                    else
                    {
                        db.Entry(client).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        SystemContext.Client = client;
                    }
                }
                   
            }
            SystemContext.typeWindow = "Каталог";
            ClientMainWindow cmw = new ClientMainWindow();
            this.Close();
            cmw.ShowDialog();       
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
            LoadAddress();
        }

        private void ClientCardButton_Click(object sender, RoutedEventArgs e)
        {
            SystemContext.typeWindow = "Корзина";
            ClientMainWindow cmw = new ClientMainWindow();
            this.Close();
            cmw.ShowDialog();
        }

        private void MyOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            OrderListWindow olw = new OrderListWindow();
            this.Close();
            olw.ShowDialog();
        }
    }
}
