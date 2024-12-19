using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Pages;

namespace WpfApp1.pages
{
    /// <summary>
    /// Логика взаимодействия для RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        private readonly Entities db = new Entities();

        public RegPage()
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

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string fio = TextBoxFIO.Text.Trim();
            string login = TextBoxLogin.Text.Trim();
            string password = PasswordBoxPassword.Password;
            string confirmPassword = PasswordBoxConfirm.Password;
            string role = CmbRole.Text;

            // Проверка на пустые поля
            if (string.IsNullOrEmpty(fio) || string.IsNullOrEmpty(login) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Все поля должны быть заполнены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка на существующий логин
            if (db.User.Any(u => u.Login == login))
            {
                MessageBox.Show("Логин уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка формата пароля
            if (password.Length < 6 || !System.Text.RegularExpressions.Regex.IsMatch(password, @"^[a-zA-Z0-9]+$") ||
                !password.Any(char.IsDigit))
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов, английские буквы и хотя бы одну цифру!",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка на совпадение паролей
            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Добавление пользователя с хэшированным паролем
            User newUser = new User
            {
                FIO = fio,
                Login = login,
                Password = GetHash(password), // Хэшируем пароль перед сохранением
                Role = role
            };

            db.User.Add(newUser);
            db.SaveChanges();

            MessageBox.Show("Регистрация успешна!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            // Переход на страницу авторизации
            NavigationService.Navigate(new AuthPage());
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Очистка полей
            TextBoxFIO.Clear();
            TextBoxLogin.Clear();
            PasswordBoxPassword.Clear();
            PasswordBoxConfirm.Clear();
            CmbRole.SelectedIndex = 0;

            // Возврат на страницу авторизации
            NavigationService.Navigate(new AuthPage());
        }
    }
}
