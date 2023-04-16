using Wpf.Ui.Common.Interfaces;
using gptask.ViewModels;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows;

namespace gptask.Views.Pages
{
    /// <summary>
    /// Interaction logic for CalendarPage.xaml
    /// </summary>
    public partial class CalendarPage : INavigableView<ViewModels.CalendarViewModel>
    {
        private DateTime _currentMonth;

        public ViewModels.CalendarViewModel ViewModel
        {
            get;
        }

        public CalendarPage(ViewModels.CalendarViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            _currentMonth = DateTime.Now;
            BuildCalendar(_currentMonth);
        }

        private void BuildCalendar(DateTime targetDate)
        {
            CalendarGrid.Children.Clear();
            CalendarGrid.RowDefinitions.Clear();
            CalendarGrid.ColumnDefinitions.Clear();

            MonthYearTextBlock.Text = $"{targetDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture)}";


            for (int i = 0; i < 7; i++)
            {
                CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition());
                var dayTextBlock = new TextBlock { Text = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[i], FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center };
                Grid.SetColumn(dayTextBlock, i);
                CalendarGrid.Children.Add(dayTextBlock);
            }

            int daysInMonth = DateTime.DaysInMonth(targetDate.Year, targetDate.Month);
            DateTime firstDayOfMonth = new DateTime(targetDate.Year, targetDate.Month, 1);
            int startColumn = (int)firstDayOfMonth.DayOfWeek;

            int currentDay = 1;
            int totalRows = (int)Math.Ceiling((daysInMonth + startColumn) / 7.0) + 1; // Updated
            for (int i = 0; i < totalRows; i++)
            {
                CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                for (int j = 0; j < 7; j++)
                {
                    if (i == 0 && j < startColumn || currentDay > daysInMonth)
                    {
                        continue;
                    }

                    var dayBorder = new Border();
                    Grid.SetRow(dayBorder, i + 1);
                    Grid.SetColumn(dayBorder, j);
                    CalendarGrid.Children.Add(dayBorder);

                    var dayGrid = new Grid();
                    dayGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    dayGrid.RowDefinitions.Add(new RowDefinition());
                    dayBorder.Child = dayGrid;

                    var dayTextBlock = new TextBlock { Text = currentDay.ToString(), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(5, 5, 0, 0) };
                    Grid.SetRow(dayTextBlock, 0);
                    dayGrid.Children.Add(dayTextBlock);

                    currentDay++;
                }
            }
        }

        private void PreviousMonthButton_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            BuildCalendar(_currentMonth);
        }

        private void NextMonthButton_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(1);
            BuildCalendar(_currentMonth);
        }
    }
}
