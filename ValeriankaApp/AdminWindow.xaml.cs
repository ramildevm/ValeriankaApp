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
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            LoadContent();
        }

        void LoadContent()
        {
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                //List<Users> users = (from u in db.Users select u).ToList<Users>();
                List<Users> users = new List<Users>();
                users.Add(new Users() { UserID = 0, UserLogin = "sssss", UserEmail = "ddd", UserRole = "admin" });
                users.Add(new Users() { UserID = 0, UserLogin = "sssss", UserEmail = "ddd", UserRole = "admin" });
                users.Add(new Users() { UserID = 0, UserLogin = "sssss", UserEmail = "ddd", UserRole = "admin" });
                users.Add(new Users() { UserID = 0, UserLogin = "sssss", UserEmail = "ddd", UserRole = "admin" });
                users.Add(new Users() { UserID = 0, UserLogin = "sssss", UserEmail = "ddd", UserRole = "admin" });
                users.Add(new Users() { UserID = 0, UserLogin = "sssss", UserEmail = "ddd", UserRole = "admin" });
                users.Add(new Users() { UserID = 0, UserLogin = "sssss", UserEmail = "ddd", UserRole = "admin" });
                users.Add(new Users() { UserID = 0, UserLogin = "sssss", UserEmail = "ddd", UserRole = "admin" });
                users.Add(new Users() { UserID = 0, UserLogin = "sssss", UserEmail = "ddd", UserRole = "admin" });
                int i = 0;
                foreach (var user in users)
                {
                    AddNewUser(user.UserLogin, user.UserEmail, user.UserRole);
                    i++;
                }
            }
        }

        void AddNewUser(string login, string email, string role)
        {
            var borderPanel = new Border() { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(2), Style = (Style)UserView.Resources["contentBorderStyle"] };
            var mainGrid = new Grid() { };
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star)});
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength()});

            StackPanel sp = new StackPanel() { };
            Grid.SetColumn(sp, 0);
            TextBlock TxtLogin = new TextBlock() {Text = "Логин: ", Style = (Style)UserView.Resources["Lbl"], Margin = new Thickness(0, 0, 0, 0) };
            TextBlock TxtEmail = new TextBlock() { Text = "Email: ", Style = (Style)UserView.Resources["Lbl"], Margin = new Thickness(0, 5, 0, 0) };
            TextBlock TxtRole = new TextBlock() { Text = "Роль: ", Style = (Style)UserView.Resources["Lbl"], Margin = new Thickness(0, 5, 0, 0) };
            TxtLogin.Inlines.Add(new TextBlock() { Text = $" {login}", Foreground = (Brush)(new BrushConverter().ConvertFrom("Black")), Margin = new Thickness(0) });
            TxtEmail.Inlines.Add(new TextBlock() { Text = $" {email}", Foreground = (Brush)(new BrushConverter().ConvertFrom("Black")), Margin = new Thickness(0) });
            TxtRole.Inlines.Add(new TextBlock() { Text = $" {role}", Foreground = (Brush)(new BrushConverter().ConvertFrom("Black")), Margin = new Thickness(0) });
            WrapPanel wp = new WrapPanel() { };
            Button deleteBtn = new Button() { Width = 81, Height = 23, Content = "Удалить", Foreground = Brushes.White, FontWeight = FontWeights.Bold };
            deleteBtn.Style = (Style)UserView.Resources["RoundedButtonStyle"];
            Grid.SetColumn(deleteBtn, 1);
            sp.Children.Add(TxtLogin);
            sp.Children.Add(TxtEmail);
            sp.Children.Add(TxtRole);
            mainGrid.Children.Add(sp);
            mainGrid.Children.Add(deleteBtn);

            borderPanel.Child = mainGrid;
            UserView.Children.Add(borderPanel);
        }

        private void Users_Click(object sender, MouseButtonEventArgs e)
        {
            LoadContent();
        }

        private void AddUser_Click(object sender, MouseButtonEventArgs e)
        {
            AdminSubWindows.AddUserWindow AUW = new AdminSubWindows.AddUserWindow();
            AUW.ShowDialog();
        }

        private void MyProfile_Click(object sender, MouseButtonEventArgs e)
        {
            AdminSubWindows.MyProfileAdminWindow MPAW = new AdminSubWindows.MyProfileAdminWindow();
            this.Close();
            MPAW.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AdminSubWindows.MyProfileAdminWindow MPAW = new AdminSubWindows.MyProfileAdminWindow();
            this.Close();
            MPAW.ShowDialog();
        }
    }
}
