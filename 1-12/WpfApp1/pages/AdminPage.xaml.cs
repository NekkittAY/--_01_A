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
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            DataGridUser.ItemsSource = Entities.GetContext().User.ToList();
        }

        // Обработчик кнопки "Добавить"
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddUserPage());
        }

        // Обработчик кнопки "Удалить"
        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            // Получаем список выделенных пользователей из DataGrid
            var usersForRemoving = DataGridUser.SelectedItems.Cast<User>().ToList();

            if (usersForRemoving.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одного пользователя для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Запрос подтверждения удаления
            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {usersForRemoving.Count()} элементов?",
                                "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    // Удаление записей из базы данных
                    Entities.GetContext().User.RemoveRange(usersForRemoving);
                    Entities.GetContext().SaveChanges();

                    MessageBox.Show("Данные успешно удалены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Обновляем DataGrid с актуальными данными
                    DataGridUser.ItemsSource = Entities.GetContext().User.ToList();
                }
                catch (Exception ex)
                {
                    // Обработка ошибок удаления
                    MessageBox.Show($"Ошибка при удалении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        // Обработчик кнопки "Редактировать"
        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = (sender as Button).DataContext as User;

            if (selectedUser != null)
            {
                NavigationService.Navigate(new AddUserPage(selectedUser)); // Передаём выбранного пользователя
            }
            else
            {
                MessageBox.Show("Выберите пользователя для редактирования!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                // Перезагружаем только сущности, которые уже есть в базе данных
                Entities.GetContext().ChangeTracker.Entries()
                    .Where(x => x.State != System.Data.Entity.EntityState.Added)
                    .ToList()
                    .ForEach(x => x.Reload());

                // Обновляем DataGrid
                DataGridUser.ItemsSource = Entities.GetContext().User.ToList();
            }
        }

    }
}
