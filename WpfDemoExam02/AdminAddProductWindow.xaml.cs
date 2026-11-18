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
using System.Xml.Linq;
using WpfDemoExam02.Models;

namespace WpfDemoExam02
{
    /// <summary>
    /// Логика взаимодействия для AdminAddProductWindow.xaml
    /// </summary>
    public partial class AdminAddProductWindow : Window
    {
        private ProductList? product = null;
        private string filename = "picture.png";
        public AdminAddProductWindow(ProductList? product = null)
        {
            InitializeComponent();
            this.product = product;
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void selectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".jpg";
            dialog.Filter = "Изображения |*.jpg;*.jpeg;*.png;...";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string filepath = dialog.FileName;
                filename = Path.GetFileName(filepath);
                filePathTb.Text = filepath;


                string destinationPath = Path.Combine(Environment.CurrentDirectory, "images", filename);
                File.Copy(filepath, destinationPath, true);
                
                LoadImage(filename);
            }
            else
            {
                filename = "picture.png";
                LoadImage();
            }    
        }

        private void removeFileBtn_Click(object sender, RoutedEventArgs e)
        {

            filePathTb.Text = "";
            LoadImage();

            if (filename != "picture.png")
            {
                string destinationPath = Path.Combine(Environment.CurrentDirectory, "images", filename);
                File.Delete(destinationPath);
            }
            filename = "picture.png";
        }

        private void LoadImage(string name = "picture.png")
        {
            string filepath = Path.Combine(Environment.CurrentDirectory, "images", name);

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(filepath, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            previewImg.Source = bitmap;
        }
        private bool CheckForEmptyInputs()
        {
            bool name = !string.IsNullOrWhiteSpace(nameTb.Text);
            bool category = categoryCb.SelectedItem != null;
            bool manufacturer = manufacturerCb.SelectedItem != null;
            bool supplier = supplierCb.SelectedItem != null;
            bool price = !string.IsNullOrWhiteSpace(priceTb.Text);
            bool unit = unitCb.SelectedItem != null;
            bool amount = !string.IsNullOrWhiteSpace(amountTb.Text);
            bool discount = !string.IsNullOrWhiteSpace(discountTb.Text);

            return name && category && manufacturer && supplier && supplier && price && unit && amount && discount;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Owner is AdminWindow)
            {
                addProductBtn.Content = "Изменить";
                filename = product.Image;
            }

            userInfoLb.Content = $"{UserData.Surname} {UserData.Name} {UserData.Patronymic}";
            LoadImage();

            using (DemoExamDBContext db = new DemoExamDBContext())
            {
                categoryCb.ItemsSource = db.Categories.ToList();
                manufacturerCb.ItemsSource = db.Manufacturers.ToList();
                supplierCb.ItemsSource = db.Suppliers.ToList();
                unitCb.ItemsSource = db.Units.ToList();

                if (product != null)
                {
                    var supplier = db.ProductSuppliers.Where(x => x.ProductId == product.ProductId).FirstOrDefault();

                    nameTb.Text = product.Product.Name;
                    categoryCb.SelectedItem = categoryCb.ItemsSource
                        .Cast<Category>()
                        .FirstOrDefault(x => x.Id == product.CategoryId);

                    descriptionTb.Text = product.Description;
                    manufacturerCb.SelectedItem = manufacturerCb.ItemsSource
                        .Cast<Manufacturer>()
                        .FirstOrDefault(x => x.Id == product.ManufacturerId);

                    supplierCb.SelectedItem = supplier?.Supplier;
                    priceTb.Text = product.Price.ToString();
                    unitCb.SelectedItem = unitCb.ItemsSource
                        .Cast<Unit>()
                        .FirstOrDefault(x => x.Id == product.UnitId);

                    amountTb.Text = product.AvailableAmount.ToString();
                    discountTb.Text = product.Discount.ToString();

                    LoadImage(product.Image);
                }
            };
        }
        private void addProductBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckForEmptyInputs())
            {
                MessageBox.Show("Заполните все поля корректно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Owner is AdminWindow)
            {
                using (DemoExamDBContext db = new DemoExamDBContext())
                {

                    product.UnitId = (unitCb.SelectedItem as Unit).Id;
                    product.Price = Convert.ToDecimal(priceTb.Text);
                    product.ManufacturerId = (manufacturerCb.SelectedItem as Manufacturer).Id;
                    
                    var supplier = db.ProductSuppliers.Where(x => x.ProductId == product.ProductId).FirstOrDefault();
                    supplier.SupplierId = (supplierCb.SelectedItem as Supplier).Id;

                    product.CategoryId = (categoryCb.SelectedItem as Category).Id;
                    product.Discount = Convert.ToInt32(discountTb.Text);
                    product.AvailableAmount = Convert.ToInt32(amountTb.Text);
                    product.Description = descriptionTb.Text;
                    product.Image = filename;

                    db.Entry(product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.Entry(supplier).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                    MessageBox.Show("Товар изменён");
                    return;
                };
            }

            using (DemoExamDBContext db = new DemoExamDBContext())
            {
                Product newProduct = new Product
                {
                    Article = null,
                    Name = nameTb.Text
                };

                db.Products.Add(newProduct);
                db.SaveChanges();

                ProductSupplier newProdSupplier = new ProductSupplier
                {
                    ProductId = newProduct.Id,
                    SupplierId = (supplierCb.SelectedItem as Supplier).Id
                };

                db.ProductSuppliers.Add(newProdSupplier);
                db.SaveChanges();

                ProductList newProductList = new ProductList
                {
                    ProductId = newProduct.Id,
                    UnitId = (unitCb.SelectedItem as Unit).Id,
                    Price = Convert.ToDecimal(priceTb.Text),
                    ManufacturerId = (manufacturerCb.SelectedItem as Manufacturer).Id,
                    CategoryId = (categoryCb.SelectedItem as Category).Id,
                    Discount = Convert.ToInt32(discountTb.Text),
                    AvailableAmount = Convert.ToInt32(amountTb.Text),
                    Description = descriptionTb.Text,
                    Image = filename
                };

                db.ProductLists.Add(newProductList);
                db.SaveChanges();
            };

            MessageBox.Show("Товар добавлен в базу");
            this.Close();
        }

        private void priceTb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            string fulltext = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength).Insert(tb.SelectionStart, e.Text);
            Regex regex = new Regex(@"^\d{0,8}(,\d{0,3})?$");

            e.Handled = !regex.IsMatch(fulltext);
        }

        private void discountTb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9+]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void amountTb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9+]");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
