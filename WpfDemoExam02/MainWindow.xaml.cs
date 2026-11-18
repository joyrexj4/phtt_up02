using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WpfDemoExam02.Models;

namespace WpfDemoExam02
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Owner is AdminWindow || Owner is ManagerWindow)
            {
                logoutBtn.Content = "Назад";
                cartBtn.Visibility = Visibility.Collapsed;
            }    

            if (!UserData.IsSignedIn())
            {
                userInfoLb.Content = "Вы зашли в режиме гостя";
                filtersSp.Visibility = Visibility.Collapsed;
            }
            else
            {
                userInfoLb.Content = $"{UserData.Surname} {UserData.Name} {UserData.Patronymic}";
            }

            switch (UserData.RoleId)
            {
                case 1:
                    filtersSp.Visibility = Visibility.Visible;
                    break;
                case 2:
                    filtersSp.Visibility = Visibility.Visible;
                    break;
                case 3:
                    filtersSp.Visibility = Visibility.Collapsed;
                    break;
                default:
                    filtersSp.Visibility = Visibility.Collapsed;
                    break;

            }
            LoadFilters();
            LoadItems();
        }
        private void LoadFilters()
        {
            using (DemoExamDBContext db = new DemoExamDBContext())
            {
                filterCb.ItemsSource = db.Categories.ToList();

                var categories = db.Categories.Select(x => new CategoryOption
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();
                categories.Insert(0, new CategoryOption { Id = null, Name = " " });

                filterCb.ItemsSource = categories;
                filterCb.SelectedIndex = 0;
            };

            sortCb.Items.Add("");
            sortCb.Items.Add("Возрастанию цены");
            sortCb.Items.Add("Убыванию цены");

            discountsCb.Items.Add("");
            discountsCb.Items.Add("0 - 12,99%");
            discountsCb.Items.Add("13 - 16,99%");
            discountsCb.Items.Add("17 - 25%");
            discountsCb.Items.Add("25 - 50%");
        }

        // Загрузка списка товаров. В аргументах задаём параметры фильтрации
        private void LoadItems(int? filterId = null, int sortIndex = 0, string searchText = "", int discountIndex = 0, int? amount = 0)
        {
            using (DemoExamDBContext db = new DemoExamDBContext())
            {
                // Создаём запрос, который будет отредактирован далее
                var query = db.ProductLists
                    .Include(x => x.Manufacturer)
                    .Include(x => x.Product)
                    .Include(x => x.Unit)
                    .Include(x => x.Category)
                    .AsQueryable();

                // Параметры фильтрации
                // Поиск
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    string textResult = searchBarTb.Text.Trim().ToLower();
                    query = query.Where(x => x.Product.Name.ToLower().Contains(textResult));
                }

                // Категория
                if (filterId.HasValue)
                {
                    query = query.Where(x => x.CategoryId == filterId.Value);
                }
                // Сортировка
                query = sortIndex switch
                {
                    1 => query.OrderBy(x => x.Price),
                    2 => query.OrderByDescending(x => x.Price),
                    _ => query.OrderBy(x => x.Id)
                };

                // Диапазон скидок
                if (discountIndex > 0)
                {
                    var (minDiscount, maxDiscount) = discountIndex switch
                    {
                        1 => (0, 12.99),
                        2 => (13, 16.99),
                        3 => (17, 25),
                        4 => (25, 50),
                        _ => (0, 100)
                    };

                    query = query.Where(x => x.Discount >= minDiscount && x.Discount <= maxDiscount);
                }

                // Количество товаров
                if (amount.HasValue)
                {
                    query = query.Where(x => x.AvailableAmount >= amount.Value);
                }

                var products = query.ToList();

                productsLv.SelectedItem = null;
                productsLv.Items.Clear();

                // Программно создаём список товаров 
                foreach (var product in products)
                {
                    string path = Path.Combine(Environment.CurrentDirectory, "images", "picture.png");
                    if (product.Image != null)
                    {
                        path = Path.Combine(Environment.CurrentDirectory, "images", product.Image);
                    }

                    Grid grid = new Grid();
                    grid.Tag = product;

                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });

                    Border imgBorder = new Border();
                    Grid.SetColumn(imgBorder, 0);
                    imgBorder.BorderThickness = new Thickness(1);
                    imgBorder.Width = 200;
                    imgBorder.BorderBrush = Brushes.Black;

                    grid.Children.Add(imgBorder);

                    Image image = new Image();
                    image.Width = 120;

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(path, UriKind.Absolute);
                    bitmap.EndInit();

                    image.Source = bitmap;
                    imgBorder.Child = image;

                    StackPanel textSp = new StackPanel();
                    textSp.Orientation = Orientation.Vertical;
                    textSp.HorizontalAlignment = HorizontalAlignment.Left;
                    textSp.Width = 520;
                    textSp.Height = 220;

                    TextBlock productName = new TextBlock();
                    // Категория товара | Наименование товара
                    productName.Text = $"{product.Category.Name} | {product.Product.Name}";
                    productName.TextWrapping = TextWrapping.Wrap;
                    productName.FontWeight = FontWeights.Bold;

                    textSp.Children.Add(productName);

                    TextBlock description = new TextBlock();
                    description.TextWrapping = TextWrapping.Wrap;
                    description.Text = $"Описание товара: {product.Description}";
                    
                    textSp.Children.Add(description);

                    TextBlock manufacturer = new TextBlock();
                    manufacturer.Text = $"Производитель: {product.Manufacturer.Name}";
                    
                    textSp.Children.Add(manufacturer);

                    var suppliers = db.ProductSuppliers.Where(x => x.ProductId == product.Id)
                        .Include(x => x.Supplier)
                        .ToList();

                    TextBlock supplier = new TextBlock();
                    supplier.Text = $"Поставщик: ";

                    foreach (var sup in suppliers)
                    {
                        supplier.Text += sup.Supplier.Name + ", ";
                    }
                    supplier.Text = supplier.Text.TrimEnd(',', ' ');
                    
                    textSp.Children.Add(supplier);

                    TextBlock price = new TextBlock();
                    if (product.Discount != 0)
                    {
                        price.Text = $"Цена: {product.Price}";
                        price.TextDecorations = TextDecorations.Strikethrough;
                        price.Foreground = Brushes.Red;
                        TextBlock totalPrice = new TextBlock();
                        totalPrice.Text = $"Итоговая цена: {product.Price - product.Price / 100 * product.Discount}";
    
                        textSp.Children.Add(price);
                        textSp.Children.Add(totalPrice);
                    }
                    else
                    {
                        textSp.Children.Add(price);
                        price.Text = $"Цена: {product.Price}";
                    }
                    
                    TextBlock unit = new TextBlock();
                    unit.Text = $"Единица измерения: {product.Unit.Name}";

                    textSp.Children.Add(unit);

                    TextBlock availableAmount = new TextBlock();
                    availableAmount.Text = $"Количество на складе: {product.AvailableAmount}";

                    if (product.AvailableAmount == 0)
                        availableAmount.Foreground = Brushes.DarkGray;
                    
                    textSp.Children.Add(availableAmount);

                    Border spBorder = new Border();
                    spBorder.BorderThickness = new Thickness(1);
                    spBorder.BorderBrush = Brushes.Black;
                    spBorder.Margin = new Thickness(10, 0, 10, 0);
                    spBorder.Padding = new Thickness(5, 10, 5, 0);
                    spBorder.Child = textSp;
                    Grid.SetColumn(spBorder, 1);

                    grid.Children.Add(spBorder);

                    TextBlock discount = new TextBlock();
                    discount.Text = $"Действующая скидка: {product.Discount}%";
                    discount.HorizontalAlignment = HorizontalAlignment.Center;
                    discount.VerticalAlignment = VerticalAlignment.Center;
                    discount.TextWrapping = TextWrapping.Wrap;

                    Border discountBorder = new Border();
                    discountBorder.BorderThickness = new Thickness(1);
                    discountBorder.BorderBrush = Brushes.Black;
                    discountBorder.Margin = new Thickness(10, 0, 10, 0);
                    Grid.SetColumn(discountBorder, 2);
                    discountBorder.Child = discount;

                    if (product.Discount >= 25)
                        grid.Background = new SolidColorBrush(Color.FromRgb(35, 225, 239));

                    grid.Children.Add(discountBorder);

                    productsLv.Items.Add(grid);
                }
            };
        }
        private void logoutBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Owner is AdminWindow || Owner is ManagerWindow)
            {
                this.Close();
                return;
            }
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private void productsLv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productsLv.SelectedItem is not Grid grid || grid.Tag is not ProductList product)
                return;

            if (Owner is AdminWindow || Owner is ManagerWindow)
            {
                ProductWindow adminProdWindow = new ProductWindow(product)
                {
                    Owner = this.Owner
                };
                adminProdWindow.ShowDialog();
                return;
            }

            ProductWindow prodWindow = new ProductWindow(product);
            prodWindow.Show();
            this.Close();
        }

        private void RefreshItems()
        {
            int? categoryId = (filterCb.SelectedItem as CategoryOption)?.Id;
            int? availableAmount = null;
            
            if (int.TryParse(amountTb.Text, out int amount))
                availableAmount = amount;
            else
                availableAmount = 0;
            
            LoadItems(categoryId, sortCb.SelectedIndex, searchBarTb.Text, discountsCb.SelectedIndex, availableAmount);
        }

        private void searchBarTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsInitialized)
                RefreshItems();
        }

        private void filterCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsInitialized)
                RefreshItems();
        }
        private void sortCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsInitialized)
                RefreshItems();
        }
        private void clearFiltersBtn_Click(object sender, RoutedEventArgs e)
        {
            filterCb.SelectedItem = null;
            sortCb.SelectedIndex = 0;
            searchBarTb.Text = "";
            discountsCb.SelectedIndex = 0;
            amountTb.Text = "";
            LoadItems();
        }
        private void discountsCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsInitialized)
                RefreshItems();
        }
        private void amountTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsInitialized)
                RefreshItems();
        }
        private void amountTb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(amountTb.Text);
        }
        private class CategoryOption
        {
            public int? Id { get; set; }
            public string Name { get; set; }
        }

        private void cartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Owner is AdminWindow)
            {
                return;
            }

            CartWindow cart = new CartWindow();
            cart.Show();
            this.Close();
        }
    }
}