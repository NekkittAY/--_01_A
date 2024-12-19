using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Entities _context = new Entities();
        public MainWindow()
        {
            InitializeComponent();
            ChartPayments.ChartAreas.Add(new ChartArea("Main"));
            CmbUser.ItemsSource = _context.User.ToList();
            CmbDiagram.ItemsSource = Enum.GetValues(typeof(SeriesChartType));
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            base.OnClosing(e);
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if (CmbUser.SelectedItem is User currentUser && CmbDiagram.SelectedItem is SeriesChartType currentType)
            {
                Series currentSeries = ChartPayments.Series.FirstOrDefault();
                if (currentSeries == null)
                {
                    currentSeries = new Series("Платежи") { IsValueShownAsLabel = true };
                    ChartPayments.Series.Add(currentSeries);
                }

                currentSeries.ChartType = currentType;
                currentSeries.Points.Clear();

                var categoriesList = _context.Category.ToList();
                foreach (var category in categoriesList)
                {
                    currentSeries.Points.AddXY(category.Name,
                        _context.Payment.Where(p => p.UserID == currentUser.ID && p.CategoryID == category.ID)
                                        .Sum(p => p.Price * p.Num));
                }
            }
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            // Реализация экспорта в Excel
        }

        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            // Реализация экспорта в Word
        }

    }
}
