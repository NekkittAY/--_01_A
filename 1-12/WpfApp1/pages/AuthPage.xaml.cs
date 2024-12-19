using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.pages;

namespace WpfApp1.Pages
{
    public partial class AuthPage : Page
    {
        // Контекст данных для ADO.NET Entity Model
        private readonly Entities db = new Entities();

        public AuthPage()
        {
            InitializeComponent();
        }

        // Метод для хэширования пароля (SHA-1)
        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password))
                    .Select(x => x.ToString("X2")));
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = TextBoxLogin.Text.Trim();
            string password = PasswordBoxPassword.Password;

            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Хэширование пароля перед сравнением
            string hashedPassword = GetHash(password);

            // Поиск пользователя в базе данных
            var user = db.User.FirstOrDefault(u => u.Login == login && u.Password == hashedPassword);

            if (user != null)
            {
                MessageBox.Show($"Добро пожаловать, {user.FIO}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Переход на страницу в зависимости от роли
                if (user.Role == "Администратор")
                    NavigationService.Navigate(new AdminPage());
                else if (user.Role == "Пользователь")
                    NavigationService.Navigate(new UserPage());
            }
            else
            {
                MessageBox.Show("Неверные логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
        }

    }
}
