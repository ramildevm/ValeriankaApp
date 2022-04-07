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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ValeriankaApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var register = new RegisterWindow();
            this.Close();
            register.ShowDialog();
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            string result = LoginMethod(txtLogin.Text, txtPassword.Password);
            MessageBox.Show(result, "Результат", MessageBoxButton.OK, MessageBoxImage.Warning);
            if (result == $"Добро пожаловать, {txtLogin.Text}!")
            {
                ClientMainWindow cmw = new ClientMainWindow();
                //AdminWindow adminWindow = new AdminWindow();
                this.Close();
                //adminWindow.ShowDialog();
                cmw.ShowDialog();
            }
        }

        private string LoginMethod(string login, string password)
        {
            if (login.Length == 0 || password.Length == 0)
                return "Не все поля заполнены!";
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                Users user = (from u in db.Users where u.UserLogin == login select u).FirstOrDefault();
                if (user == null)
                    return "Пользователя с таким логином не существует!";
                if (user.UserPassword != password)
                    return "Неверный пароль!";
                SystemContext.User = user;
            }
            return $"Добро пожаловать, {login}!";
        }
    }
}
