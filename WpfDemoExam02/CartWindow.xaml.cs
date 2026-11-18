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
    /// Логика взаимодействия для CartWindow.xaml
    /// </summary>
    public partial class CartWindow : Window
    {
        private int? userId;
        private bool? isEditing;
        private bool? isDeleting;
        public CartWindow(int? userId = null, bool? isEditing = null, bool? isDeleting = null)
        {
            InitializeComponent();
            this.userId = userId;
            this.isEditing = isEditing;
            this.isDeleting = isDeleting;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            userInfoLb.Content = $"{UserData.Surname} {UserData.Name} {UserData.Patronymic}";
            
            if (Owner is AdminWindow || Owner is ManagerWindow)
            {
                LoadOrderHistoryItems();
            }
            if (isDeleting == true)
            {
                deleteOrderBtn.Visibility = Visibility.Visible;
            }
        }
        private void LoadOrderHistoryItems()
        {
            if (userId == null)
            {
                MessageBox.Show("Не найден id пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            using (DemoExamDBContext db = new DemoExamDBContext())
            {
                var orders = db.Orders.Where(x => x.UserId == userId)
                    .Include(x => x.Product)
                    .Include(x => x.OrderStatus)
                    .Include(x => x.PickupPointAddress)
                        .ThenInclude(y => y.Street)
                    .ToList();

                foreach (var order in orders)
                {   
                    Grid grid = new Grid();
                    grid.Tag = order;

                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });

                    Border mainBorder = new Border();
                    mainBorder.BorderBrush = Brushes.Black;
                    mainBorder.BorderThickness = new Thickness(1);
                    mainBorder.Margin = new Thickness(0, 0, 10, 0);

                    StackPanel sp = new StackPanel();
                    sp.Orientation = Orientation.Vertical;
                    sp.Margin = new Thickness(15, 0, 15, 0);

                    TextBlock article = new TextBlock();
                    article.FontWeight = FontWeights.Bold;
                    article.Text = $"Артикул: {order.Product.Article}";

                    TextBlock status = new TextBlock();
                    status.Text = $"Статус: {order.OrderStatus.Name}";

                    TextBlock address = new TextBlock();
                    address.Text = $"{order.PickupPointAddress.Street.StreetName} {order.PickupPointAddress.HouseNumber}";

                    TextBlock orderDate = new TextBlock();
                    orderDate.Text = order.OrderDate.ToShortDateString();

                    sp.Children.Add(article);
                    sp.Children.Add(status);
                    sp.Children.Add(address);
                    sp.Children.Add(orderDate);

                    mainBorder.Child = sp;
                    Grid.SetColumn(mainBorder, 0);
                    grid.Children.Add(mainBorder);

                    Border dateBorder = new Border();
                    dateBorder.BorderBrush = Brushes.Black;
                    dateBorder.BorderThickness = new Thickness(1);

                    TextBlock deliveryDate = new TextBlock();
                    deliveryDate.HorizontalAlignment = HorizontalAlignment.Center;
                    deliveryDate.Text = $"Дата доставки\n{order.DeliveryDate.ToShortDateString()}";

                    dateBorder.Child = deliveryDate;
                    Grid.SetColumn(dateBorder, 1);
                    grid.Children.Add(dateBorder);

                    cartLv.Items.Add(grid);
                }
            };
        }

        private void deleteOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (cartLv.SelectedItem is not Grid grid || grid.Tag is not Order selectedOrder)
            {
                MessageBox.Show("Выберите заказ, чтобы его удалить", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (DemoExamDBContext db = new DemoExamDBContext())
            {
                db.Orders.Remove(selectedOrder);
                db.SaveChanges();

            };
            cartLv.Items.Clear();
            LoadOrderHistoryItems();
            MessageBox.Show("Заказ удалён");
        }
        private void logoutBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Owner is AdminWindow || Owner is ManagerWindow)
            {
                this.Close();
                return;
            }

            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void cartLv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cartLv.SelectedItem is not Grid grid || grid.Tag is not Order selectedOrder)
                return;

            if (isEditing == true)
            {
                AdminAddOrderWindow order = new AdminAddOrderWindow(userId, selectedOrder);
                order.ShowDialog();

                cartLv.Items.Clear();
                LoadOrderHistoryItems();
            }

        }
    }
}
