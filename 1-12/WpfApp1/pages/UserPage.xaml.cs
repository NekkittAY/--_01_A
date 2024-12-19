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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1.pages
{
    /// <summary>
    /// Логика взаимодействия для UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        public UserPage()
        {
            InitializeComponent();
            CmbSorting.SelectedIndex = 0; // Установка сортировки по умолчанию
            CheckDriver.IsChecked = false; // Сбрасываем фильтр
            UpdateUsers(); // Начальная загрузка данных
        }

        // Метод для обновления данных
        private void UpdateUsers()
        {
            // Загружаем всех пользователей
            var currentUsers = Entities.GetContext().User.ToList();

            // Поиск по Ф.И.О. (без учета регистра)
            if (!string.IsNullOrWhiteSpace(TextBoxSearch.Text))
            {
                currentUsers = currentUsers.Where(x => x.FIO.ToLower()
                    .Contains(TextBoxSearch.Text.ToLower())).ToList();
            }

            // Фильтрация: показываем только пользователей
            if (CheckDriver.IsChecked == true)
            {
                currentUsers = currentUsers.Where(x => x.Role.Contains("Пользователь")).ToList();
            }

            // Сортировка
            if (CmbSorting.SelectedIndex == 0) // По возрастанию
            {
                currentUsers = currentUsers.OrderBy(x => x.FIO).ToList();
            }
            else // По убыванию
            {
                currentUsers = currentUsers.OrderByDescending(x => x.FIO).ToList();
            }

            // Устанавливаем данные в ListView
            ListUser.ItemsSource = currentUsers;
        }

        // Обработчик поиска
        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateUsers();
        }

        // Обработчик изменения сортировки
        private void CmbSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUsers();
        }

        // Обработчик фильтрации
        private void CheckDriver_Checked(object sender, RoutedEventArgs e)
        {
            UpdateUsers();
        }

        private void CheckDriver_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateUsers();
        }

        // Обработчик кнопки "Очистить фильтр"
        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            TextBoxSearch.Clear();
            CmbSorting.SelectedIndex = 0;
            CheckDriver.IsChecked = false;
            UpdateUsers();
        }
    }
}
