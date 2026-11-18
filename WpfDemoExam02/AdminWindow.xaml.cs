using Microsoft.EntityFrameworkCore;
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
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
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

        private void addProductBtn_Click(object sender, RoutedEventArgs e)
        {
            AdminAddProductWindow addWindow = new AdminAddProductWindow();
            addWindow.ShowDialog();

            productsDg.ItemsSource = null;
            LoadProductsDataGrid();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            userInfoLb.Content = $"{UserData.Surname} {UserData.Name} {UserData.Patronymic}";
            LoadProductsDataGrid();
            LoadUsersDataGrid();
        }
        private void LoadProductsDataGrid()
        {
            using (DemoExamDBContext db = new DemoExamDBContext())
            {
                productsDg.ItemsSource = db.ProductLists
                    .Include(x => x.Product)
                    .ToList();
            };
        }

        private void LoadUsersDataGrid()
        {
            using (DemoExamDBContext db = new DemoExamDBContext())
            {
                usersDg.ItemsSource = db.Users.ToList();
            };
        }

        private void editProductBtn_Click(object sender, RoutedEventArgs e)
        {
            if (productsDg.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selected = (productsDg.SelectedItem as ProductList);

            AdminAddProductWindow editWindow = new AdminAddProductWindow(selected)
            {
                Owner = this
            };
            editWindow.ShowDialog();
        }

        private void deleteProductBtn_Click(object sender, RoutedEventArgs e)
        {
            if (productsDg.SelectedItem == null)
            {
                MessageBox.Show("Сначало выберите товар из списка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (DemoExamDBContext db = new DemoExamDBContext())
            {
                var selected = (productsDg.SelectedItem as ProductList);
                var selectedProdSupplier = db.ProductSuppliers.Where(x => x.ProductId == selected.ProductId).ToList();
                var selectedProd = db.Products.Where(x => x.Id == selected.ProductId).FirstOrDefault();

                var productList = db.ProductLists.Where(x => x.Id == selected.Id).FirstOrDefault();
                
                if (selectedProdSupplier == null || selectedProd == null || productList == null)
                {
                    MessageBox.Show("Товар уже удалён");
                    return;
                }

                var productInOrders = db.Orders.Where(x => x.ProductId == selected.ProductId).FirstOrDefault();
                if (productInOrders != null)
                {
                    MessageBox.Show("Нельзя удалить товар, потому что он есть в заказе", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                db.ProductLists.Remove(productList);
                foreach (var supplier in selectedProdSupplier)
                {
                    db.ProductSuppliers.Remove(supplier);
                }
                db.Products.Remove(selectedProd);
                db.SaveChanges();
            };

            productsDg.ItemsSource = null;
            LoadProductsDataGrid();

            MessageBox.Show("Товар удалён");
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

        private void addOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (usersDg.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя, чтобы добавить заказ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selected = (usersDg.SelectedItem as User).Id;

            AdminAddOrderWindow addOrder = new AdminAddOrderWindow(selected);
            addOrder.ShowDialog();
        }

        private void editOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (usersDg.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя, чтобы отредактировать заказ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selected = (usersDg.SelectedItem as User).Id;

            CartWindow cart = new CartWindow(selected, true)
            {
                Owner = this
            };
            cart.ShowDialog();
        }

        private void deleteOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (usersDg.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя, чтобы удалить заказ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selected = (usersDg.SelectedItem as User).Id;

            CartWindow cart = new CartWindow(selected, false, true)
            {
                Owner = this
            };
            cart.ShowDialog();
        }
    }
}
