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
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void SignInBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckForEmptyInputs())
            {
                MessageBox.Show("Заполните поля корректно", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (DemoExamDBContext db = new DemoExamDBContext())
            {
                var user = db.Users.Where(x => x.Login == loginTb.Text).FirstOrDefault();

                if (user != null && user.Password == passwordPb.Password)
                {
                    UserData.Id = user.Id;
                    UserData.RoleId = user.RoleId;
                    UserData.Surname = user.Surname;
                    UserData.Name = user.Name;
                    UserData.Patronymic = user.Patronymic;
                    
                    switch (user.RoleId)
                    {
                        case 1:
                            AdminWindow admin = new AdminWindow();
                            admin.Show();
                            break;
                        case 2:
                            ManagerWindow manager = new ManagerWindow();
                            manager.Show();
                            break;
                        case 3:
                            MainWindow main = new MainWindow();
                            main.Show();
                            break;
                        default:
                            MessageBox.Show("Не найдена роль пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                    }
                    this.Close();
                }
                else
                    MessageBox.Show("Неправильный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            };
        }

        public bool CheckForEmptyInputs()
        {
            bool login = !string.IsNullOrWhiteSpace(loginTb.Text);
            bool password = !string.IsNullOrWhiteSpace(passwordPb.Password);

            return login && password;
        }

        private void GuestModeBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UserData.ClearData();
        }
    }
}
