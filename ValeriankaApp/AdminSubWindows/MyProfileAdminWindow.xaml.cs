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

namespace ValeriankaApp.AdminSubWindows
{
    /// <summary>
    /// Логика взаимодействия для MyProfileAdminWindow.xaml
    /// </summary>
    public partial class MyProfileAdminWindow : Window
    {
        public MyProfileAdminWindow()
        {
            InitializeComponent();
            LoadDefault();
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

        private void LoadDefault()
        {
            var user = SystemContext.User;

            txtLogin.Text = user.UserLogin;
            txtEmail.Text = user.UserEmail;
            txtPassword.Password = user.UserPassword;
            txtPasswordConfirm.Password = user.UserPassword;
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
                    }
                }
                SystemContext.User = (from u in db.Users where u.UserID == SystemContext.User.UserID select u).FirstOrDefault();
            }
            AdminWindow adminWindow = new AdminWindow();
            this.Close();
            adminWindow.ShowDialog();
        }

        private void AdminUserListClick_Button(object sender, MouseButtonEventArgs e)
        {
            AdminWindow aw = new AdminWindow();
            this.Close();
            aw.ShowDialog();
        }

        private void AdminUserAddClick_Button(object sender, MouseButtonEventArgs e)
        {
            AddUserWindow auw = new AddUserWindow();
            this.Close();
            auw.ShowDialog();
        }
    }
}
