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
            LoadContent(searchTxt.Text);
        }

        void LoadContent(string searchText)
        {
            using (var db = new Pharmacy_ValeriankaEntities())
            {
                List<Users> users;
                try
                {
                    if (searchText == "")
                    {
                        users = (from u in db.Users select u).ToList<Users>();
                    }
                    else
                    {
                        IEnumerable<Users> userSet = (from u in db.Users select u);
                        users = userSet.Where(user => user.UserLogin.Contains($"{searchText}")).ToList<Users>();
                    }
                    int i = 0;
                    foreach (var user in users)
                    {
                        AddNewUser(i, user.UserLogin, user.UserEmail, user.UserRole);
                        i++;
                    }
                }
                catch
                {

                }
            }
        }

        void AddNewUser(int i, string login, string email, string role)
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
            Button deleteBtn = new Button() { Width = 81, Height = 23, Content = "Удалить", Foreground = Brushes.White, FontWeight = FontWeights.Bold, Cursor = Cursors.Hand };
            deleteBtn.Style = (Style)UserView.Resources["RoundedButtonStyle"];
            Grid.SetColumn(deleteBtn, 1);
            deleteBtn.Name = login;
            deleteBtn.Click += DeleteButtonOnClick;

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
            UserView.Children.Clear();
            LoadContent(searchTxt.Text);
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

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            UserView.Children.Clear();
            LoadContent(searchTxt.Text);
        }

        private void DeleteButtonOnClick(object sender, EventArgs eventArgs)
        {
            var button = (Button)sender;
            if (button != null)
            {
                using (var db = new Pharmacy_ValeriankaEntities())
                {
                    var user = db.Users.FirstOrDefault(x => x.UserLogin == button.Name);
                    if (user == null)
                    {
                        return;
                    }
                    db.Users.Remove(user);
                    db.SaveChanges();
                    UserView.Children.Clear();
                    LoadContent(searchTxt.Text);
                }
            }
        }
    }
}
