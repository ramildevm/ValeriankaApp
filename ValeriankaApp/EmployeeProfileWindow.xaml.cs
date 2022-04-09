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
    /// Логика взаимодействия для EmployeeProfileWindow.xaml
    /// </summary>
    public partial class EmployeeProfileWindow : Window
    {
        public EmployeeProfileWindow()
        {
            InitializeComponent();
            try
            {
                btnProfileText.Text = SystemContext.User.UserLogin;
                btnProfile.Click += ButtonMyProfile_Click;
            }
            catch { }
        }

        private void ButtonMyProfile_Click(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
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
            }
            AdminWindow adminWindow = new AdminWindow();
            this.Close();
            adminWindow.ShowDialog();
        }

        private void ButtonCatalog_Click(object sender, RoutedEventArgs e)
        {
            EmployeeMainWindow emw = new EmployeeMainWindow();
            this.Close();
            emw.ShowDialog();
        }

        private void ButtonOrders_Click(object sender, RoutedEventArgs e)
        {
            EmployeeOrderListWindow eolw = new EmployeeOrderListWindow();
            this.Close();
            eolw.ShowDialog();
        }
    }
}
