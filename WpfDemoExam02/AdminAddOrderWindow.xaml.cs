using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AdminAddOrderWindow.xaml
    /// </summary>
    public partial class AdminAddOrderWindow : Window
    {
        private Order? order;
        private int? userId;
        public AdminAddOrderWindow(int? userId = null, Order? order = null)
        {
            InitializeComponent();
            this.order = order;
            this.userId = userId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (DemoExamDBContext db = new DemoExamDBContext())
            {
                articleCb.ItemsSource = db.Products.ToList();
                statusesCb.ItemsSource = db.OrderStatuses.ToList();
                var houseList = db.Houses.Select(
                    x => new
                    {
                        Id = x.Id,
                        FullAddress = x.Street.StreetName + ", " + x.HouseNumber
                    }).ToList();
                addressesCb.ItemsSource = houseList;
            };
            LoadData();
        }
        private void LoadData()
        {
            using (DemoExamDBContext db = new DemoExamDBContext())
            {
                if (order != null)
                {
                    createOrderButton.Content = "Изменить заказ";

                    articleCb.SelectedValue = order.ProductId;
                    statusesCb.SelectedValue = order.OrderStatusId;
                    amountTb.Text = order.Amount.ToString();
                    addressesCb.SelectedValue = order.PickupPointAddressId;
                    orderDateDp.SelectedDate = order.OrderDate;
                    deliveryDateDp.SelectedDate = order.DeliveryDate;
                }
            };
        }
        private bool CheckForEmptyInputs()
        {
            bool article = articleCb.SelectedItem != null;
            bool status = statusesCb.SelectedItem != null;
            bool amount = !string.IsNullOrWhiteSpace(amountTb.Text);
            bool address = addressesCb.SelectedItem != null;
            bool orderDate = orderDateDp.SelectedDate != null;
            bool deliveryDate = deliveryDateDp.SelectedDate != null;

            return article && status && amount && address && orderDate && deliveryDate;
        }
        private void createOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckForEmptyInputs())
            {
                MessageBox.Show("Заполните все поля корректно", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (orderDateDp.SelectedDate > deliveryDateDp.SelectedDate)
            {
                MessageBox.Show("Дата заказа не может быть позднее даты доставки", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (DemoExamDBContext db = new DemoExamDBContext())
            {
                if (order != null)
                {
                    order.ProductId = (articleCb.SelectedItem as Product).Id;
                    order.Amount = Convert.ToInt32(amountTb.Text);
                    order.OrderDate = orderDateDp.SelectedDate.Value;
                    order.DeliveryDate = deliveryDateDp.SelectedDate.Value;
                    order.PickupPointAddressId = Convert.ToInt32(addressesCb.SelectedValue);
                    order.UserId = userId.Value;
                    order.OrderStatusId = (statusesCb.SelectedItem as OrderStatus).Id;

                    db.Entry(order).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();

                    MessageBox.Show("Товар изменён");
                    this.Close();
                    return;
                }
                else
                {
                    var lastCode = db.Orders
                        .Where(x => x.ObtainingCode != 0)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault();

                    Order newOrder = new Order
                    {
                        ProductId = (articleCb.SelectedItem as Product).Id,
                        Amount = Convert.ToInt32(amountTb.Text),
                        OrderDate = orderDateDp.SelectedDate.Value,
                        DeliveryDate = deliveryDateDp.SelectedDate.Value,
                        PickupPointAddressId = Convert.ToInt32(addressesCb.SelectedValue),
                        UserId = userId.Value,
                        ObtainingCode = lastCode.ObtainingCode + 1,
                        OrderStatusId = (statusesCb.SelectedItem as OrderStatus).Id
                    };

                    db.Orders.Add(newOrder);
                    db.SaveChanges();
                }
            };

            MessageBox.Show("Заказ добавлен");
            this.Close();
        }

        private void amountTb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
         
        private void amountTb_LostFocus(object sender, RoutedEventArgs e)
        {
            using (DemoExamDBContext db = new DemoExamDBContext())
            {
                var selectedArticle = (articleCb.SelectedItem as Product);
                if (selectedArticle != null)
                {
                    var selectedProduct = db.Products.Where(x => x.Article == selectedArticle.Article).FirstOrDefault();
                    var productList = db.ProductLists.Where(x => x.ProductId == selectedProduct.Id).FirstOrDefault();

                    if (int.TryParse(amountTb.Text, out int amount))
                    {
                        if (amount > productList.AvailableAmount)
                            amountTb.Text = productList.AvailableAmount.ToString();
                    }
                }
            };
        }
        private void articleCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(amountTb.Text))
                amountTb.Text = "1";
            else
                amountTb_LostFocus(sender, e);
        }
    }
}
