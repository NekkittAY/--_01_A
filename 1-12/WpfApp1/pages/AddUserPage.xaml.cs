using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

namespace WpfApp1.pages
{
    /// <summary>
    /// Логика взаимодействия для AddUserPage.xaml
    /// </summary>
    public partial class AddUserPage : Page
    {
        private User _currentUser;

        public static string GetHash(string input)
        {
            using (var sha1 = SHA1.Create())
            {
                byte[] data = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                return string.Concat(data.Select(b => b.ToString("X2"))); // Возвращает хеш в виде строки в верхнем регистре
            }
        }

        public AddUserPage(User selectedUser = null)
        {
            InitializeComponent();

            // Если передан пользователь для редактирования, используем его, иначе создаем нового
            _currentUser = selectedUser ?? new User();
            DataContext = _currentUser;

            // Заполняем поля при редактировании
            TextBoxLogin.Text = _currentUser.Login; // Заполняем логин
            TextBoxFIO.Text = _currentUser.FIO;     // Заполняем ФИО
            TextBoxPhoto.Text = _currentUser.Photo; // Заполняем путь к фото

            // Установка значения роли в ComboBox
            if (!string.IsNullOrEmpty(_currentUser.Role))
            {
                // Сопоставляем текущую роль с элементами ComboBox
                CmbRole.SelectedItem = CmbRole.Items
                    .Cast<ComboBoxItem>()
                    .FirstOrDefault(item => item.Content.ToString() == _currentUser.Role);
            }
            else
            {
                CmbRole.SelectedIndex = 0; // Значение по умолчанию
            }

            // Поле пароля оставляем пустым для редактирования
            TextBoxPassword.Text = string.Empty;
        }


        // Обработчик кнопки "Сохранить"
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            // Обновляем данные модели вручную перед сохранением
            _currentUser.Login = TextBoxLogin.Text.Trim();
            _currentUser.Password = GetHash(TextBoxPassword.Text.Trim());
            _currentUser.Role = (CmbRole.SelectedItem as ComboBoxItem)?.Content.ToString();
            _currentUser.FIO = TextBoxFIO.Text.Trim();
            _currentUser.Photo = TextBoxPhoto.Text.Trim() ?? string.Empty;

            // Проверка на обязательные поля
            if (string.IsNullOrWhiteSpace(_currentUser.Login))
                errors.AppendLine("Укажите логин!");
            if (string.IsNullOrWhiteSpace(_currentUser.Password))
                errors.AppendLine("Укажите пароль!");
            if (string.IsNullOrWhiteSpace(_currentUser.Role))
                errors.AppendLine("Выберите роль!");
            if (string.IsNullOrWhiteSpace(_currentUser.FIO))
                errors.AppendLine("Укажите ФИО!");

            // Если есть ошибки, выводим их и выходим из метода
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Добавление нового пользователя
            if (_currentUser.ID == 0) // ID == 0 означает новый пользователь
                Entities.GetContext().User.Add(_currentUser);

            // Попытка сохранения данных
            try
            {
                Entities.GetContext().SaveChanges();
                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Обработчик кнопки "Отмена"
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
