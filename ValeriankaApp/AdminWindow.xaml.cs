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
        }

        private void Users_Click(object sender, MouseButtonEventArgs e)
        {
            
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
            MPAW.ShowDialog();
        }
    }
}
