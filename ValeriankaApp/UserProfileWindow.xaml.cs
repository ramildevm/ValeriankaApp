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
                Client client = new Client();
                SystemContext.Client = (from c in db.Client where c.UserID == SystemContext.User.UserID select c).FirstOrDefault();
                string user = SystemContext.User.UserLogin;
                if (txtLogin.Text != "" || txtEmail.Text != "" || txtPassword.Password != "")
                {
                    if (txtLogin.Text != "" && txtLogin.Text.Length >= 5 && txtLogin.Text != user)
                    {
                        SystemContext.User.UserLogin = txtLogin.Text;
                    }
                    if (txtEmail.Text != "" && txtEmail.Text != SystemContext.User.UserEmail)
                    {
                        SystemContext.User.UserEmail = txtEmail.Text;
                    }
                    if (txtPassword.Password == txtPasswordConfirm.Password && txtPassword.Password != "" && txtPassword.Password.Length >= 8 && SystemContext.User.UserPassword != txtPassword.Password)
                    {
                        SystemContext.User.UserPassword = txtPassword.Password;
                    }
                    db.Entry(SystemContext.User).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                
                if (SystemContext.Client == null)
                {
                    if (txtFIO.Text != "")
                    {
                        if (txtNumber.Text != "")
                        {
                            if (ComBoxBaseAddress.SelectedItem != null)
                            {
                                client.ClientFIO = txtFIO.Text;
                                client.UserID = SystemContext.User.UserID;
                                client.ClientNumber = txtNumber.Text;
                                client.ClientPreferredAddress = ComBoxBaseAddress.Text;
                                db.Client.Add(client);
                                db.SaveChanges();
                            }
                            else
                            {
                                MessageBox.Show("Нужно заполнить все личные данные!");
                            }
                        }
                    }
                }
                else
                {
                    if (txtFIO.Text != "" || txtNumber.Text != "" || ComBoxBaseAddress.SelectedItem != null)
                    {
                        if (txtFIO.Text != "")
                        {
                            SystemContext.Client.ClientFIO = txtFIO.Text;
                        }
                        if (txtNumber.Text != "")
                        {
                            SystemContext.Client.ClientNumber = txtNumber.Text;
                        }
                        if (ComBoxBaseAddress.SelectedItem != null)
                        {
                            SystemContext.Client.ClientPreferredAddress = ComBoxBaseAddress.Text;
                        }
                        db.Entry(SystemContext.Client).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
           
                    }
                    
                }
                   
            }
            ClientMainWindow cmw = new ClientMainWindow();
            this.Close();
            cmw.ShowDialog();

            
        }
    }
}
