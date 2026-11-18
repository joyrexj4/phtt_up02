using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
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
using WpfDemoExam02.Models;

namespace WpfDemoExam02
{
    /// <summary>
    /// Логика взаимодействия для ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        private ProductList product;
        public ProductWindow(ProductList product)
        {
            InitializeComponent();
            this.product = product;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Owner is AdminWindow || Owner is ManagerWindow)
            {
                addToCartBtn.Visibility = Visibility.Hidden;
                addToCartBtn.Content = "Добавить в заказ";
            }

            if (!UserData.IsSignedIn())
            {
                userInfoLb.Content = "Вы вошли в режиме гостя";
                addToCartBtn.IsEnabled = false;
                addToCartBtn.Width = 250;
                addToCartBtn.Content = "Нельзя заказать товар в режиме гостя";
            }
            else
                userInfoLb.Content = $"{UserData.Surname} {UserData.Name} {UserData.Patronymic}";

            using (DemoExamDBContext db = new DemoExamDBContext())
            {
                var suppliers = db.ProductSuppliers
                    .Where(x => x.ProductId == product.Id)
                    .Include(x => x.Supplier)
                    .ToList();

                string path = Path.Combine(Environment.CurrentDirectory, "images", "picture.png");
                if (product.Image != null)
                {
                    path = Path.Combine(Environment.CurrentDirectory, "images", product.Image);
                }

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.EndInit();

                img.Source = bitmap;

                productNameTb.Text = product.Product.Name;
                productCategoryTb.Text += product.Category.Name;
                productManufacturerTb.Text += product.Manufacturer.Name;
                foreach (var sup in suppliers)
                {
                    productSupplierTb.Text += sup.Supplier.Name + ", ";
                }
                productSupplierTb.Text = productSupplierTb.Text.TrimEnd(',', ' ');

                productPriceTb.Text += product.Price;
                productDiscountTb.Text += $"{product.Discount}%";
                productDiscountPriceTb.Text += $"{product.Price - product.Price / 100 * product.Discount}";
                productUnitTb.Text += product.Unit.Name;
                productAmountTb.Text += product.AvailableAmount.ToString();

                totalPriceLb.Content = $"Итого: {product.Price}";
                
                if (product.AvailableAmount == 0)
                {
                    totalPriceLb.Content = $"Товар закончился";
                    addToCartBtn.IsEnabled = false;
                }
            };
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
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

        private void amountTb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void amountTb_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox amount = (TextBox)sender;
            if (Convert.ToInt32(amount.Text) < 1)
                amountTb.Text = "1";
        }

        private void decreaseBtn_Click(object sender, RoutedEventArgs e)
        {
            int amount = Convert.ToInt32(amountTb.Text);
            if (amount > 1)
            {
                amount--;
                amountTb.Text = amount.ToString();
                UpdatePrice();
            }
        }
        private void increaseBtn_Click(object sender, RoutedEventArgs e)
        {
            int amount = Convert.ToInt32(amountTb.Text);
            if (amount < product.AvailableAmount)
            {
                amount++;
                amountTb.Text = amount.ToString();
                UpdatePrice();
            }
        }
        private void UpdatePrice()
        {
            int amount = Convert.ToInt32(amountTb.Text);
            totalPriceLb.Content = $"Итого: {product.Price * amount}";
        }
        private void addToCartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Owner is AdminWindow)
            {
                MessageBox.Show("Товар добавлен в корзину");
                this.Close();
                return;
            }
        }
    }
}
