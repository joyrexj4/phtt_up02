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
using WpfDemoExam02.Models;

namespace WpfDemoExam02
{
    /// <summary>
    /// Логика взаимодействия для ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        public ManagerWindow()
        {
            InitializeComponent();
        }

        private void logoutBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private void viewProductsBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow()
            {
                Owner = this
            };
            main.ShowDialog();
        }

        private void viewOrdersBtn_Click(object sender, RoutedEventArgs e)
        {
            if (usersDg.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selected = (usersDg.SelectedItem as User).Id;

            CartWindow cart = new CartWindow(selected)
            {
                Owner = this
            };
            cart.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUsersDataGrid();
        }
        private void LoadUsersDataGrid()
        {
            using (DemoExamDBContext db = new DemoExamDBContext())
            {
                usersDg.ItemsSource = db.Users.ToList();
            };
        }
    }
}
