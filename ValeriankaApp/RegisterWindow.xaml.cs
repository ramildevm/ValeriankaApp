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
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            var loginPage = new MainWindow();
            this.Close();
            loginPage.ShowDialog();
        }
        string registerMethod(string login, string password, string confirmPas, string email)
        {
            if (email.Length ==0)
                return "Не все поля заполнены!";
            if (login.Length < 5 || login.Length == 0)
                return "Логин должен содержать от 5 символов!";
            if (password.Length < 8 || password.Length == 0)
                return "Минимальный размер пароля: 8 символов!";
            if (password != confirmPas)
                return "Пароли не соответсвуют друг другу!";
            using(var db = new Pharmacy_ValeriankaEntities())
            {
                Users user = (from u in db.Users where u.UserLogin == login select u).FirstOrDefault<Users>();
                if (user != null)
                    return "Пользователь с таким логином уже существует!";

                Users user2 = db.Users.Add(new Users() { UserLogin = login, UserPassword = password, UserEmail = email, UserRole = "User" });
                db.Client.Add(new Client() {ClientID= user2.UserID, UserID =  user2.UserID});
                db.SaveChanges();
            }
            return "Регистрация прошла успешно!";
        }

        private void ButtonReg_Click(object sender, RoutedEventArgs e)
        {
            string result = registerMethod(txtLogin.Text, txtPassword.Password, txtPasswordConfirm.Password, txtEmail.Text);
            MessageBox.Show(result, "Результат", MessageBoxButton.OK, MessageBoxImage.Warning);
            if (result == "Регистрация прошла успешно!")
                ButtonBack_Click(this, new RoutedEventArgs());
        }
    }
}
